using PerformanceTest.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Methods
{
    public static class StreamingMemoryMappedFileWorkerMethod
    {
        public static TestResult Execute(string filePath)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long initialMemory = GC.GetTotalMemory(true);

            int totalLinesProcessed = 0;
            int totalIoOperations = 0;
            long totalIoTimeMilliseconds = 0;

            try
            {
                using var mmf = MemoryMappedFile.CreateFromFile(filePath, System.IO.FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
                var fileSize = new System.IO.FileInfo(filePath).Length;
                var totalWorkers = Environment.ProcessorCount;
                var chunkSize = DetermineChonkSize(fileSize, totalWorkers);

                var workQueue = new BlockingCollection<ProcessingChunk>(totalWorkers);

                var readerTask = Task.Factory.StartNew(() =>
                {
                    long offset = 0;
                    while (offset < fileSize)
                    {
                        var remaining = fileSize - offset;
                        var length = Math.Min(chunkSize, remaining);

                        var chunk = new ProcessingChunk
                        {
                            Offset = offset,
                            Length = length,
                            MemoryMappedFile = mmf,
                        };

                        workQueue.Add(chunk);
                        offset += length;
                    }
                    workQueue.CompleteAdding();
                });

                var workerTasks = new Task[totalWorkers];
                for (int i = 0; i < totalWorkers; i++)
                {
                    workerTasks[i] = Task.Factory.StartNew(() =>
                    {
                        foreach (var chunk in workQueue.GetConsumingEnumerable())
                        {
                            var metrics = ProcessingChunk(chunk);
                            Interlocked.Add(ref totalLinesProcessed, metrics.LinesProcessed);
                            Interlocked.Add(ref totalIoOperations, metrics.IoOperations);
                            Interlocked.Add(ref totalIoTimeMilliseconds, metrics.IoTime);
                        }
                    });
                }
                Task.WaitAll(workerTasks);
                readerTask.Wait();
            }
            finally
            {

            }

            stopwatch.Stop();
            double troughput = MetricsCalculator.CalculateThroughputMBPerSecond(filePath, stopwatch.ElapsedMilliseconds);
            double linesPerSecond = MetricsCalculator.CalculateLinesPerSecond(totalLinesProcessed, stopwatch.ElapsedMilliseconds);

            return new TestResult(filePath, "Streaming Memory Mapped File Worker Method", failed: false)
            {
                Method = nameof(StreamingMemoryMappedFileWorkerMethod),
                ExecutionTimeMilliseconds = stopwatch.ElapsedMilliseconds,
                IOOperations = totalIoOperations,
                MemoryUsageBytes = GC.GetTotalMemory(true) - initialMemory,
                ThroughputMBPerSecond = troughput,
                LinesProcessed = totalLinesProcessed,
                LinesPerSecond = linesPerSecond,
                IOTimeMilliseconds = totalIoTimeMilliseconds
            };
        }

        private static TaskMetrics ProcessingChunk(ProcessingChunk chunk)
        {
            int ioOperations = 0;
            int linesProcessed = 0;
            long ioTime = 0;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            using (var accessor = chunk.MemoryMappedFile.CreateViewAccessor(chunk.Offset, chunk.Length, MemoryMappedFileAccess.Read))
            {
                byte[] buffer = new byte[chunk.Length];
                accessor.ReadArray(0, buffer, 0, buffer.Length);
                ioOperations++;
                ioTime += stopwatch.ElapsedMilliseconds;

                char previousChar = '\0';
                var sb = new StringBuilder();

                foreach (var b in buffer) 
                {
                    char currentChar = (char)b;
                    if (previousChar != '\r' && currentChar == '\n')
                    {
                        sb.Append(' ');
                    }
                    else if (previousChar != '\r' && currentChar == '\n')
                    {
                        linesProcessed++;
                        sb.Clear();
                    }
                    else if (currentChar != '\r')
                    {
                        sb.Append(currentChar);
                    }
                    previousChar = currentChar;
                }

                if (sb.Length > 0)
                {
                    linesProcessed++;
                }

                return new TaskMetrics
                {
                    LinesProcessed = linesProcessed,
                    IoOperations = ioOperations,
                    IoTime = ioTime
                };
            }
        }

        private static long DetermineChonkSize(long fileSize, int totalWorkers)
        {
            const long baseCunkSize = 10L * 1024 * 1024; // 10 MB
            const long maxChunkSize = 100L * 1024 * 1024; // 100 MB

            return Math.Min(baseCunkSize * totalWorkers, maxChunkSize);
        }
    }

    public class ProcessingChunk
    {
        public long Offset { get; set; }
        public long Length { get; set; }
        public MemoryMappedFile MemoryMappedFile { get; set; }
    }

    public class TaskMetrics
    {
        public int LinesProcessed { get; set; }
        public int IoOperations { get; set; }
        public long IoTime { get; set; }
    }
}
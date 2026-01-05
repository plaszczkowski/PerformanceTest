using PerformanceTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Methods
{
    public static class BatchMethod
    {
        public static TestResult Execute(string filePath)
        {
            const int batchSize = 1024; // Number of lines to process in each batch

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long initialMemory = GC.GetTotalMemory(true);

            int ioOperations = 0;
            int linesProcessed = 0;
            long ioTimeMilliseconds = 0;

            try
            {
                using var streamReader = new StreamReader(filePath, Encoding.UTF8, false, bufferSize: 8192);

                char[] buffer = new char[batchSize];

                while ((ioOperations = streamReader.Read(buffer, 0, batchSize)) > 0) // Read a batch of lines
                {
                    var stopwatchBatch = System.Diagnostics.Stopwatch.StartNew();
                    ioTimeMilliseconds = stopwatchBatch.ElapsedMilliseconds;

                    var content = new string(buffer, 0, ioOperations);
                    var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    linesProcessed += lines.Length;

                    foreach (var line in lines)
                    {
                        // Simulate processing each line (e.g., parsing, computations)
                        var processedLine = line.ToUpperInvariant(); // Dummy processing
                    }
                }
            }
            finally
            {

            }

            stopwatch.Stop();

            return new TestResult(filePath, "Batch Method", failed: false)
            {
                ExecutionTimeMilliseconds = stopwatch.ElapsedMilliseconds,
                ThroughputMBPerSecond = MetricsCalculator.CalculateThroughputMBPerSecond(filePath, stopwatch.ElapsedMilliseconds),
                LinesProcessed = linesProcessed,
                LinesPerSecond = MetricsCalculator.CalculateLinesPerSecond(linesProcessed, stopwatch.ElapsedMilliseconds),
                IOTimeMilliseconds = ioTimeMilliseconds,
            };
        }
    }
}

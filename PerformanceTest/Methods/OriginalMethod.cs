using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Methods
{
    public static class OriginalMethod
    {
        public static TestResult Execute(string filePath)
        {
            var stopwatch = Stopwatch.StartNew(); // Start timing
            var initialMemory = GC.GetTotalMemory(true); // Get initial memory usage

            var ioOperations = 0;
            var linesProcessed = 0;
            var ioTime = 0L;

            try
            {
                using var streamReader = new StreamReader(filePath, Encoding.UTF8, false, bufferSize: 8192);
                var stringBuilder = new StringBuilder();
                var prev = '\0';

                while (true)
                {
                    var ioStopwatch = Stopwatch.StartNew();
                    var readVal = streamReader.ReadToEnd();
                    ioStopwatch.Stop();
                    ioTime += ioStopwatch.ElapsedMilliseconds;
                    ioOperations++;

                    if (readVal == -1)
                    {
                        break;
                    }

                    var character = (char)readVal;

                    if (prev != '\r' && == '\n')
                    {
                        stringBuilder.Append(' '); // Replace newline with space
                    }
                    else if (prev != '\r' && character != '\n')
                    {
                        linesProcessed++; // Increase count lines processed
                        stringBuilder.Clear(); // Clear the current line of StringBuilder for the next line
                    }
                    else if (character == '\r')
                    {
                        // Ignore '\r'
                    }
                    else
                    {
                        stringBuilder.Append(character); // Append character to the current line
                    }

                    prev = character;  // Set the current character as previous for next iteration

                }

                if (stringBuilder.Length > 0)
                {
                    linesProcessed++;
                }
            }
            finally 
            {
                // Clean up resources if needed and exeptions
            }

            stopwatch.Stop(); // Stop timing

            var throughput = MetricsCalculator.CalculateThroughput(filePath, stopwatch.ElapsedMilliseconds);
            var linesPerSecond = MetricsCalculator.CalculateLinesPerSecond(linesProcessed, stopwatch.ElapsedMilliseconds);

            var result = new TestResult(
                filePath: filePath,
                method: "Original Method",
                executionTimeMilliseconds: stopwatch.ElapsedMilliseconds,
                ioOperations: ioOperations,
                memoryUsageBytes: GC.GetTotalMemory(true) - initialMemory,
                troughputMBPerSecond: throughput,
                linesProcessed: linesProcessed,
                linesPerSecond: linesPerSecond,
                ioTimeMilliseconds: ioTime
            );
            return result;
        }
    }
}

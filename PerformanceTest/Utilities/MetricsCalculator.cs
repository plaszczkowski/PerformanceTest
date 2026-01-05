using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Utilities
{
    public static class MetricsCalculator
    {
        public static double CalculateThroughputMBPerSecond(string filePath, long executionTimeMilliseconds)
        {
            if (executionTimeMilliseconds == 0) return 0;
            var fileSizeMB = new FileInfo(filePath).Length / (1024.0 * 1024.0); // Convert bytes to MB
            var executionTimeSeconds = executionTimeMilliseconds / 1000.0; // Convert milliseconds to seconds
            return fileSizeMB / executionTimeSeconds;
        }

        public static double CalculateLinesPerSecond(long linesProcessed, long executionTimeMilliseconds)
        {
            if (executionTimeMilliseconds == 0) return 0;
            var executionTimeSeconds = executionTimeMilliseconds / 1000.0; // Convert milliseconds to seconds
            return linesProcessed / executionTimeSeconds; // Lines per second
        }

        public static double CalculateProcentageDifference(double originalValue, double newValue)
        {
            if (originalValue == 0) return 0;
            var difference = originalValue - newValue;
            return (difference / (double)originalValue) * 100.0;
        }

        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public static double CalculateIOPercentage(long ioTimeMilliseconds, long totalExecutionTimeMilliseconds)
        {
            if (totalExecutionTimeMilliseconds == 0) return 0;
            return (ioTimeMilliseconds / (double)totalExecutionTimeMilliseconds) * 100.0; // Percentage of time spent in I/O
        }
    }
}

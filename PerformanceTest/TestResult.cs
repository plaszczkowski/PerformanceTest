using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class TestResult
    {
        public string FilePath { get; set; }
        public string Method { get; set; }
        public long ExecutionTimeMilliseconds { get; set; }
        public BigInteger IOOperations { get; set; }
        public long MemoryUsageBytes { get; set; }
        public double TroughputMBPerSecond { get; set; }
        public long LinesProcessed { get; set; }
        public double LinesPerSecond { get; set; }
        public long IOTimeMilliseconds { get; set; }
        public bool Failed { get; set; }

    public TestResult(string filePath, string method, long executionTimeMilliseconds = 0,
        BigInteger ioOperations = default, long memoryUsageBytes = 0, double troughputMBPerSecond = 0,
        long linesProcessed = 0, double linesPerSecond = 0, long ioTimeMilliseconds = 0, bool failed = false)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            ExecutionTimeMilliseconds = executionTimeMilliseconds;
            IOOperations = ioOperations;
            MemoryUsageBytes = memoryUsageBytes;
            TroughputMBPerSecond = troughputMBPerSecond;
            LinesProcessed = linesProcessed;
            LinesPerSecond = linesPerSecond;
            IOTimeMilliseconds = ioTimeMilliseconds;
            Failed = failed;
        }

        public override string ToString()
        {
            return $"File: {FilePath}, Method: {Method}, Execution Time: {ExecutionTimeMilliseconds} ms, " +
                   $"IO Operations: {IOOperations}, Memory Usage: {MemoryUsageBytes} bytes, " +
                   $"Throughput: {TroughputMBPerSecond} MB/s, Lines Processed: {LinesProcessed}, " +
                   $"Lines per Second: {LinesPerSecond}, IO Time: {IOTimeMilliseconds} ms, " +
                   $"Failed: {Failed}";
        }
    }
}
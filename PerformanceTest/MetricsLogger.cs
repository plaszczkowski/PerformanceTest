using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PerformanceTest.Utilities;

namespace PerformanceTest
{
    public static class MetricsLogger
    {
        public static void WriteResultsToFile(List<TestResult> results, string filePath)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            WriteEnvirormentInfo(writer);

            var resultsGroupByFile = results.GroupBy(r => r.FilePath);

            foreach (var group in resultsGroupByFile)
            {
                var fileName = Path.GetFileName(group.Key);
                var fileSize = FormatBytes(new FileInfo(group.Key).Length);

                writer.WriteLine();
                writer.WriteLine($"Results for file: {group.Key}");

                foreach (var result in group)
                {
                    writer.WriteLine($"Method: {result.MethodName}");
                    writer.WriteLine($"File Size: {fileSize}");
                    writer.WriteLine($"Execution Time (ms): {result.ExecutionTimeMilliseconds}");
                    writer.WriteLine($"Throughput (MB/s): {result.ThroughputMBPerSecond}");
                    writer.WriteLine($"Lines Processed: {result.LinesProcessed}");
                    writer.WriteLine($"Lines per Second: {result.LinesPerSecond}");
                    writer.WriteLine($"IO Time (ms): {result.IOTimeMilliseconds}");
                    writer.WriteLine($"IO Percentage (%): {result.IOPercentage}");
                    writer.WriteLine($"Failed: {result.Failed}");
                    writer.WriteLine();
                }
                writer.WriteLine(); // Extra line between different files
            }

            // Write header
            writer.WriteLine("File Path,Method Name,Execution Time (ms),Throughput (MB/s),Lines Processed,Lines per Second,IO Time (ms),IO Percentage (%),Failed");
            foreach (var result in results)
            {
                writer.WriteLine($"{result.FilePath},{result.MethodName},{result.ExecutionTimeMilliseconds},{result.ThroughputMBPerSecond},{result.LinesProcessed},{result.LinesPerSecond},{result.IOTimeMilliseconds},{result.IOPercentage},{result.Failed}");
            }
        }

        private static void WriteEnvirormentInfo(StreamWriter writer) {
            {
                writer.WriteLine();

                // RAM information
                try
                {
                    var totalMemory = GetTotalMemoryInBytes();

                }
                catch (Exception)
                {

                    throw                }
            }
        }
        public static long GetTotalMemoryInBytes()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetTotalMemoryOnWindows()
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetTotalMemoryInBytesOnLinux();
            }
            else
            {
                throw new PlatformNotSupportedException($"The current platform is not supported {RuntimeInformation.OSDescription}");
            }
        }

        private static long GetTotalMemoryOnWindows()
        {
            var memoryStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memoryStatus))
            {
                return (long)memoryStatus.ullTotalPhys;
            }
            else
            {
                throw new InvalidOperationException("Unable to retrive memory information via GlobalMemoryStatusEx.");
            }
        }

        private static long GetTotalMemoryInBytesOnLinux()
        {
            try
            {
                if (File.Exists("/proc/meminfo"))
                {
                    var lines = File.ReadAllLines("/proc/meminfo");
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("MemTotal:"))
                        {
                            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2 && long.TryParse(parts[1], out long memKb))
                            {
                                return memKb * 1024; // Convert from KB to Bytes
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore reade errors and fallback to sysinfo
            }
            throw new InvalidOperationException("Unable to retrive memory information from /proc/meminfo.");
        }

        [DllImport('kernel32.dll', CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys; // Total availble memory RAM
            public ulong ullAvailPhys; // Availbible RAM
            public ulong ulTotalPageFile;
            public ulong ulAvailPageFile;
            public ulong usTotalVirtual;
            public ulong usAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        private static string FormatBytes(long bytes)
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
    }
}

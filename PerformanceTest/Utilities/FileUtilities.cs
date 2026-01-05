using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Utilities
{
    public static class FileUtilities
    {
        public static void CreateTestFile(string[] filePaths)
        {

            foreach (var filePath in filePaths)
            {
                long fileSize = DetermineFileSize(filePath);
                CreateTestFile(filePath, fileSize);
            }
        }

        public static void CleanupTestFiles(string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"Deleted test file: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
                }
            }
        }

        private static void CreateTestFile(string filePath, long sizeInBytes)
        {
            const string sampleLine = "This is a sample line for performance testing purposes.\r\n";
            var totalWrittenBytes = 0L;

            try
            {
                using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

                while (totalWrittenBytes < sizeInBytes)
                {
                    writer.Write(sampleLine);
                    totalWrittenBytes += Encoding.UTF8.GetBytes(sampleLine).Length;
                }

                Console.WriteLine($"Created test file: {filePath} ({FormatBytes(totalWrittenBytes)})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create file {filePath}: {ex.Message}");
                throw;
            }
        }

        private static long DetermineFileSize(string filePath)
        {
            var fileName = Path.GetFileName(filePath).ToLower();
            if (fileName.Contains("small")) return 1L * 1024 * 1024; // 1 MB
            if (fileName.Contains("medium")) return 10L * 1024 * 1024; // 10 MB

            throw new ArgumentException($"Unknown file type for path: {filePath}");
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
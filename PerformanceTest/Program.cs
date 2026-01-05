using PerformanceTest.Utilities;

namespace PerformanceTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Performance Tests - Comparing Processing Methods\n");

            // Directories of test files
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var resultsFilePath = Path.Combine(baseDirectory, "PerformanceTestResults.txt");

            string[] testFiles =
            {
                Path.Combine(baseDirectory, "small_test_file.txt"),
                Path.Combine(baseDirectory, "medium_test_file.txt"),
            };

            Console.WriteLine("Creating test files...");
            FileUtilities.CreateTestFile(testFiles);

            Console.WriteLine("Running tests...");
            var testRunner = new TestRunner();
            var results = testRunner.RunTests(testFiles);

            Console.WriteLine("Writing results to a file...");
            MetricsLogger.WriteResultsToFile(results, resultsFilePath);

            FileUtilities.CleanupTestFiles(testFiles);
        }
    }
}
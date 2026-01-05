namespace PerformanceTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Performance Tests - Compering Processing Methods\n");

            //Directotriesof test files
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var resultsFilePath = Path.Combine(baseDirectory, "PerformanceTestResults.txt");

            string[] testFiles =
            {
                Path.Combine(baseDirectory, "small_test_file.txt"),
                Path.Combine(baseDirectory, "medium_test_file.txt"),
            };

            Console.WriteLine("Running tests...");
            var testRunner = new TestRunner();
            var results = testRunner.RunTests(testFiles);

            Console.WriteLine("Writting results to a file ...");
            MetricsLogger > WriteResultsToFile(results, resultsFilePath);

            FileUtilities.CleanUpTestFiles(testFiles);
        }
    }
}

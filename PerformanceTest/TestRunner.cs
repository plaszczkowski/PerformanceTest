using PerformanceTest.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class TestRunner
    {
        public List<TestResult> RunTests(string[] filePaths)
        {
            var results = new List<TestResult>();
            foreach (var file in filePaths)
            {
                Console.WriteLine($"Running tests for file: {file}");

                results.Add(RunSingleTest(file, "Original Method", OriginalMethod.Execute));

            }
            return results;
        }

        private TestResult RunSingleTest(string filePath, string methodName, Func<string, TestResult> method)
        {
            try
            {
                return method(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed for file: {filePath}, Method: {methodName}. Error: {ex.Message}");
                return new TestResult(filePath, methodName, failed: true);
            }
        }
    }
}

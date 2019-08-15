using System.IO;
using TestGenerator.Models;

namespace TestGenerator.Generation
{
    public interface ITestWriter
    {
        string ScaffoldTest(TestClassDefinition testClassDefinition, string testProject, string testPath);
    }

    public class TestWriter : ITestWriter
    {
        public string ScaffoldTest(TestClassDefinition testClassDefinition, string testProject, string testPath)
        {
            var template = new UnitTestTemplate(testClassDefinition);
            var content = template.TransformText();

            var directory = DetermineDirectory(testProject, testPath);
            var fileName = Path.Combine(directory, $"{testClassDefinition.ClassName}.cs");
            File.WriteAllText(fileName, content);

            return fileName;
        }

        private static string DetermineDirectory(string testProject, string testPath)
        {
            var testProjectDirectory = Directory.GetParent(testProject);
            var testDirectory = Path.Combine(testProjectDirectory.FullName, testPath);

            if (!Directory.Exists(testDirectory))
                Directory.CreateDirectory(testDirectory);

            return testDirectory;
        }
    }
}

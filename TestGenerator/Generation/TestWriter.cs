using System.IO;
using EnvDTE;
using TestGenerator.Models;

namespace TestGenerator.Generation
{
    public interface ITestWriter
    {
        void ScaffoldTest(TestClassDefinition testClassDefinition, Project testProject, string testPath);
    }

    public class TestWriter : ITestWriter
    {
        public void ScaffoldTest(TestClassDefinition testClassDefinition, Project testProject, string testPath)
        {
            var template = new UnitTestTemplate(testClassDefinition);
            var content = template.TransformText();

            var directory = DetermineDirectory(testProject.FullName, testPath);
            var fileName = Path.Combine(directory, $"{testClassDefinition.ClassName}.cs");
            File.WriteAllText(fileName, content);

            testProject.ProjectItems.AddFromFile(fileName);
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

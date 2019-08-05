using System.IO;
using TestGenerator.Models;

namespace TestGenerator.Generation
{
    public interface ITestWriter
    {
        void ScaffoldTest(TestClassDefinition testClassDefinition);
    }

    public class TestWriter : ITestWriter
    {
        public void ScaffoldTest(TestClassDefinition testClassDefinition)
        {
            var template = new UnitTestTemplate(testClassDefinition);
            var content = template.TransformText();
            File.WriteAllText($"{testClassDefinition.ClassName}.cs", content);
        }
    }
}

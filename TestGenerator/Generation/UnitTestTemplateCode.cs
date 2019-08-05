using TestGenerator.Models;

namespace TestGenerator.Generation
{
    public partial class UnitTestTemplate
    {
        private readonly TestClassDefinition _testClassDefinition;

        public UnitTestTemplate(TestClassDefinition testClassDefinition)
        {
            _testClassDefinition = testClassDefinition;
        }
    }
}

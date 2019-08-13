using System.Collections.Generic;

namespace TestGenerator.Models
{
    public class TestClassDefinition
    {
        public string Namespace { get; set; }
        public string ClassName => $"{TargetClassName}Tests";
        public string TargetClassName { get; set; }
        public string TargetBaseType { get; set; }
        public IList<string> Dependencies { get; } = new List<string>();
        public IList<string> Methods { get; } = new List<string>();
    }
}

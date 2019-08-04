using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGenerator.Models
{
    public class ClassDefinition
    {
        public NamespaceDeclarationSyntax Namespace { get; set; }
        public ClassDeclarationSyntax Class { get; set; }
        public IList<ConstructorDeclarationSyntax> Constructors { get; set; }
    }
}

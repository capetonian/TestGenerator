using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGenerator.Models;

namespace TestGenerator.Parsing
{
    public interface IClassParser
    {
        IEnumerable<ClassDefinition> LoadClass(SyntaxTree tree);
    }

    public class ClassParser : IClassParser
    {
        public IEnumerable<ClassDefinition> LoadClass(SyntaxTree tree)
        {
            if (!(tree.GetRoot() is CompilationUnitSyntax)) yield return null;

            var root = tree.GetRoot() as CompilationUnitSyntax;

            if (root.Members.Count != 1 && !(root.Members.First() is NamespaceDeclarationSyntax)) yield return null;

            var nameSpace = root.Members.First() as NamespaceDeclarationSyntax;

            // Search for interfaces and classes
            var interfaces = nameSpace.Members.OfType<InterfaceDeclarationSyntax>();
            var classes = nameSpace.Members.OfType<ClassDeclarationSyntax>();

            // Search for constructors and methods
            foreach (var classDeclaration in classes)
            {
                var constructors = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().ToList();

                yield return new ClassDefinition
                {
                    Namespace = nameSpace,
                    Class = classDeclaration,
                    Constructors = constructors
                };
            }
        }
    }
}

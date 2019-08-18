using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGenerator.Models;

namespace TestGenerator.Parsing
{
    public interface IClassParser
    {
        IEnumerable<TestClassDefinition> LoadClass(SyntaxTree tree);
    }

    public class ClassParser : IClassParser
    {
        public IEnumerable<TestClassDefinition> LoadClass(SyntaxTree tree)
        {
            if (!(tree.GetRoot() is CompilationUnitSyntax)) yield break;

            var root = tree.GetRoot() as CompilationUnitSyntax;

            if (root.Members.Count != 1 && !(root.Members.First() is NamespaceDeclarationSyntax)) yield break;

            var baseNamespace = root.Members.First() as NamespaceDeclarationSyntax;

            // Search for interfaces and classes
            var interfaces = baseNamespace.Members.OfType<InterfaceDeclarationSyntax>();
            var classes = baseNamespace.Members.OfType<ClassDeclarationSyntax>();

            // Search for constructors and methods
            foreach (var classDeclaration in classes)
            {
                var testClass = new TestClassDefinition
                {
                    Namespace = $"{RemoveNewLine(baseNamespace.Name.GetText().ToString())}.Tests",
                    TargetClassName = classDeclaration.Identifier.Text,
                    TargetBaseType = GetTargetBaseType(classDeclaration)
                };

                var constructors = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().ToList();

                var constructorWithDependencies = constructors.OrderByDescending(_ => _.ParameterList.Parameters.Count).FirstOrDefault();

                if (constructorWithDependencies != null)
                {
                    foreach (var dependency in constructorWithDependencies.ParameterList.Parameters)
                    {
                        testClass.Dependencies.Add(RemoveNewLine(dependency.Type.GetText().ToString()));
                    }
                }

                var publicMethods = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Modifiers.Any(modifier => modifier.Text == "public"))
                    .ToList();

                publicMethods.ForEach(method => testClass.Methods.Add(method.Identifier.Text));

                yield return testClass;
            }
        }

        private static string GetTargetBaseType(BaseTypeDeclarationSyntax targetType)
        {
            if (targetType.BaseList == null || targetType.BaseList.Types.Count != 1)
                return targetType.Identifier.Text;

            var baseType = targetType.BaseList.Types.First().Type.GetText();

            return RemoveNewLine(baseType.ToString());
        }

        private static string RemoveNewLine(string text) => Regex.Replace(text, @"\s+", string.Empty);
    }
}

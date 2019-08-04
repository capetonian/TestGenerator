using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestGenerator.Parsing
{
    public interface ISyntaxTreeFactory
    {
        SyntaxTree LoadSyntaxTreeFromDocument(Document document);
    }

    public class SyntaxTreeFactory : ISyntaxTreeFactory
    {
        public SyntaxTree LoadSyntaxTreeFromDocument(Document document)
        {
            var text = LoadDocumentText(document);

            var tree = CSharpSyntaxTree.ParseText(text);

            return tree;
        }

        private static string LoadDocumentText(Document document)
        {
            var currentDocument = document.Object() as TextDocument;
            var editPoint = currentDocument.StartPoint.CreateEditPoint();

            var text = editPoint.GetText(currentDocument.EndPoint);

            return text;
        }
    }
}

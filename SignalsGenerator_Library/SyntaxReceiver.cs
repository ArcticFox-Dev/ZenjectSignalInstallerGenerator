using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pixions.SignalsGenerator.Utils;

namespace ArcticFox.Zenject.SignalsGenerator
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<string> SignalsToInstall = new List<string>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is StructDeclarationSyntax interfaceDeclarationSyntax)) return;

            if (IsSignalDefinition(syntaxNode))
            {
                var domainFullName = GetBaseTypeFullName(interfaceDeclarationSyntax);
                SignalsToInstall.Add(domainFullName);
            }
        }

        private static bool IsSignalDefinition(SyntaxNode syntaxNode)
        {
            return syntaxNode.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Any(AttributeUtils.IsSignalAttribute);
        }

        private static string GetBaseTypeFullName(BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
        {
            if (baseTypeDeclarationSyntax.Parent is null) return baseTypeDeclarationSyntax.Identifier.Text;
            return $"{GetNodeFullName(baseTypeDeclarationSyntax.Parent)}.{baseTypeDeclarationSyntax.Identifier.Text}";
        }

        private static string GetNodeFullName(SyntaxNode node)
        {
            var name = "";
            if (node.Parent != null)
            {
                var parentName = GetNodeFullName(node.Parent);
                name = string.IsNullOrEmpty(parentName) ? name : name + ".";
            }
            if (node is ClassDeclarationSyntax classNode)
            {
                name += classNode.Identifier.Text;
            }

            if (node is NamespaceDeclarationSyntax namespaceNode)
            {
                name += namespaceNode.Name;
            }

            return name;
        }
    }
}
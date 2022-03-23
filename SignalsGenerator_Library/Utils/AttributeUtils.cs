using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pixions.SignalsGenerator.Utils
{
    public class AttributeUtils
    {
        #region Attribute Names

        private const string SignalDefinitionFullName = "SignalAttribute";
        private static readonly string SignalDefinitionShortName = "Signal";
        
        #endregion
        
        public static bool IsSignalAttribute(AttributeSyntax attributeSyntax)
        {
            return attributeSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(ins => ins.DescendantTokens()
                    .Any(st => st.Kind() == SyntaxKind.IdentifierToken
                               && (st.ValueText == SignalDefinitionFullName
                                   || st.ValueText == SignalDefinitionShortName)));
        }
        
        public static bool IsSignalAttribute(AttributeData attributeData)
        {
            return attributeData.AttributeClass != null && attributeData.AttributeClass.Name == SignalDefinitionFullName;
        }

    }
}
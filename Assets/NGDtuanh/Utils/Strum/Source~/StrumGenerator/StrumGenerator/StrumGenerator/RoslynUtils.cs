using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NGDtuanh.Utils {
    public static class RoslynHelpers {
        public static string GlobalName(this ISymbol symbol) =>
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        public static string NameFromGlobalName(this string fullName) =>
            fullName.Split([".", "::"], StringSplitOptions.None).Last();

        public static (
            string nspace
          , List<TypeDeclarationSyntax> types
            ) GetSyntaxPath(this TypeDeclarationSyntax target) {
            (string nspace, List<TypeDeclarationSyntax> types) result = (null, []);

            SyntaxNode ite = target;

            do
                if (ite is NamespaceDeclarationSyntax nspace)
                    result.nspace = nspace.Name + (result.nspace != null ? "." : "") + result.nspace;
                else if (ite is TypeDeclarationSyntax type)
                    result.types.Add(type);
            while ((ite = ite.Parent) != null);

            result.types.Reverse();

            return result;
        }
    }
}
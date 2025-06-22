using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NGDtuanh.Utils;

namespace StrumGenerator;

[Generator]
public class Main : IIncrementalGenerator {
    private const string Namespace          = "NGDtuanh.Utils";
    private const string AttrName           = $"Strum{nameof(Attribute)}";
    private const string AttrNameGlobalName = $"global::{Namespace}.{AttrName}";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Where(static item => item.ok)
            .Select(static (item, _) => item.input);

        context.RegisterSourceOutput(provider.Collect(), Generate);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken _) {
        // Is type declaration
        if (node is not TypeDeclarationSyntax strum) return false;

        // Have attribute
        if (strum.AttributeLists.Count == 0) return false;

        // All partial
        do
            if (!strum.Modifiers.Any(SyntaxKind.PartialKeyword))
                return false;
        while ((strum = strum.Parent as TypeDeclarationSyntax) != null);

        return true;
    }

    private static (bool ok, GenInput input) Transform(GeneratorSyntaxContext context, CancellationToken _) {
        var strum = (TypeDeclarationSyntax)context.Node;

        foreach (var attr in strum.AttributeLists.SelectMany(static attrList => attrList.Attributes)) {
            if (attr.ArgumentList?.Arguments is not { } paras
                // Point to at least one enum
             || paras.Count < 1
             || context.SemanticModel.GetSymbolInfo(attr).Symbol is not IMethodSymbol attrSymbol
                // This is StrumAttribute
             || attrSymbol.ContainingType.GlobalName() != AttrNameGlobalName)
                continue;

            var enumGlobalNames = new List<string>();
            var enumFields      = new List<List<string>>();

            foreach (var para in paras)
                if (para.Expression is TypeOfExpressionSyntax typeOf
                 && context.SemanticModel.GetTypeInfo(typeOf.Type).Type is { TypeKind: TypeKind.Enum } enumSymbol) {
                    enumGlobalNames.Add(enumSymbol.GlobalName());
                    enumFields.Add(enumSymbol.GetMembers()
                        .Where(member => member is IFieldSymbol { IsStatic: true })
                        .Select(RoslynHelpers.GlobalName).ToList());
                } else return (false, null);

            return (true, new GenInput(
                Strum: strum
              , EnumGlobalNames: enumGlobalNames
              , EnumFields: enumFields));
        }

        return (false, null);
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<GenInput> inputs) {
        foreach (var (strum, enumGlobalNames, enumFields) in inputs) {
            var builder = new StringBuilder();

            var flatEnumFields = enumFields.SelectMany(static item => item).ToList();
            var isSingle    = enumFields.Count == 1;
            var syntaxPath     = strum.GetSyntaxPath();
            var strumFullName = syntaxPath.nspace + (syntaxPath.nspace != null ? "." : "")
              + string.Join(".", syntaxPath.types.Select(static type => type.Identifier.Text));

            // ----------HEADER----------
            // namespace
            if (syntaxPath.nspace != null)
                builder.Append($"namespace {syntaxPath.nspace} {{ ");
            // types declaration
            builder.Append(string.Join(" ", syntaxPath.types.Select(static type =>
                $"partial {type.Keyword.Text} {type.Identifier.Text}{type.TypeParameterList?.ToString()} {{")));

            // ----------FIELDS----------
            builder.Append($@"
public partial struct Fields<T> {{
{string.Join("\n", flatEnumFields.Select(static field => $"\tpublic T {field};"))}

    public T this[int index] {{
        get => this.ValueRO(index);
        set => this.ValueRW(index) = value;
    }}        

{string.Join("\n\n", enumGlobalNames.Select(static enumGlobalName => $@"    public T this[{enumGlobalName} index] {{
        get => this.ValueRO(index);
        set => this.ValueRW(index) = value;
    }}"))}

    public Enumerator GetEnumerator() => new Enumerator(this);
    
    public struct Enumerator {{
        private int index;

        private readonly Fields<T> strum;
        
        public Enumerator(Fields<T> _strum) {{
            index = -1; 
            strum = _strum;
        }}

        public bool MoveNext() => ++index < Count{(isSingle ? "" : ".All")};

        public T Current => strum[index];
    }}
}}");

            // ----------FIRST----------
            builder.Append(flatEnumFields.Count == 0 ? "" : "\n\n" + (isSingle
                ? enumFields[0].Count == 0 ? "" : $"public const {enumGlobalNames[0]} First = {enumGlobalNames[0]}.{enumFields[0].First()};"
                : $@"public static partial class First {{
    public const int All = 0;
{string.Join("\n", enumGlobalNames
    .Zip(enumFields, static (enumGlobalName, fields) => (enumGlobalName, fields))
    .Where(item => item.fields.Count != 0).Select(item => 
        $"\tpublic const {item.enumGlobalName} {item.enumGlobalName.NameFromGlobalName()} = {item.enumGlobalName}.{item.fields.First()};"))}
}}"));
            
            // ----------LAST----------
            builder.Append(flatEnumFields.Count == 0 ? "" : "\n\n" + (isSingle
                ? enumFields[0].Count == 0 ? "" : $"public const {enumGlobalNames[0]} Last = {enumGlobalNames[0]}.{enumFields[0].Last()};"
                : $@"public static partial class Last {{
    public const int All = {flatEnumFields.Count - 1};
{string.Join("\n", enumGlobalNames
    .Zip(enumFields, static (enumGlobalName, fields) => (enumGlobalName, fields))
    .Where(item => item.fields.Count != 0).Select(item => 
        $"\tpublic const {item.enumGlobalName} {item.enumGlobalName.NameFromGlobalName()} = {item.enumGlobalName}.{item.fields.Last()};"))}
}}"));
            
            // ----------COUNT----------
            builder.Append("\n\n" + (isSingle
                ? $"public const int Count = {enumFields[0].Count};"
                : $@"public static partial class Count {{
    public const int All = {flatEnumFields.Count};
{string.Join("\n", enumGlobalNames.Zip(enumFields, static (enumGlobalName, fields) => 
        $"\tpublic const int {enumGlobalName.NameFromGlobalName()} = {fields.Count};"))}
}}"));
            
            // ----------INDEX OF----------
            int indexOfBuffer = 0;
            builder.Append(isSingle
                ? ""
                : $@"

{string.Join("\n\n", enumGlobalNames.Zip(enumFields, (enumGlobalName, fields) => {
    int prev = indexOfBuffer;
    indexOfBuffer += fields.Count;
    return $"public static int IndexOf({enumGlobalName} index) => {prev} + (int)index;";
}))}");
            
            // ----------INDEX ENUMERATOR----------
            builder.Append(flatEnumFields.Count == 0 ? "" : "\n\n" + (isSingle
                ? $"public static readonly z__IndexEnumerable Indexes = default;"
                : $@"public static partial class Indexes {{
    public static readonly z__IndexEnumerable.All All = default;
{string.Join("\n", enumGlobalNames.Zip(enumFields, (enumGlobalName, fields) => (enumGlobalName, fields))
    .Where(item => item.fields.Count != 0).Select(item => 
    $"\tpublic static readonly z__IndexEnumerable.{item.enumGlobalName.NameFromGlobalName()} {item.enumGlobalName.NameFromGlobalName()} = default;"))}
}}"));
            
            // ----------INDEX ENUMERABLE----------
            builder.Append(flatEnumFields.Count == 0 ? "" : isSingle 
                ? $@"

public struct z__IndexEnumerable {{
    public Enumerator GetEnumerator() => Enumerator.New; 
    
    public struct Enumerator {{
        public static readonly Enumerator New = new Enumerator {{ index = First - 1}};
        
        private {enumGlobalNames[0]} index;

        public bool MoveNext() => ++index <= Last;

        public {enumGlobalNames[0]} Current => index;
    }} 
}}"
                : $@"

public static partial class z__IndexEnumerable {{
    public struct All {{
        public Enumerator GetEnumerator() => Enumerator.New; 
        
        public struct Enumerator {{
            public static readonly Enumerator New = new Enumerator {{ index = First.All - 1}};
            
            private int index;

            public bool MoveNext() => ++index <= Last.All;

            public int Current => index;
        }} 
    }}    

    {string.Join("\n\n\t", enumGlobalNames.Zip(enumFields, (enumGlobalName, fields) => (enumGlobalName, fields))
        .Where(item => item.fields.Count != 0).Select(item =>
        $@"public struct {item.enumGlobalName.NameFromGlobalName()} {{
        public Enumerator GetEnumerator() => Enumerator.New; 
        
        public struct Enumerator {{
            public static readonly Enumerator New = new Enumerator {{ index = First.{item.enumGlobalName.NameFromGlobalName()} - 1}};
            
            private {item.enumGlobalName} index;

            public bool MoveNext() => ++index <= Last.{item.enumGlobalName.NameFromGlobalName()};

            public {item.enumGlobalName} Current => index;
        }} 
    }}"))}
}}");

            // ----------CLOSE BRACES----------
            builder.Append(new string('}', (syntaxPath.nspace != null ? 1 : 0) + syntaxPath.types.Count));
            
            // ----------EXTENSIONS----------
            builder.Append($@"

public static partial class StrumExtensions {{
    public static ref T ValueRW<T>(this ref {strumFullName}.Fields<T> strum, int index) {{
        switch (index) {{
{string.Join("\n", flatEnumFields.Select((field, index) => 
    $"\t\t\tcase {index}: return ref strum.{field};"))}

            default: throw new System.Exception($""NGDtuanh wrong strum index, founded: {{index}} {{(int)index}}"");
        }}
    }}

    public static ref readonly T ValueRO<T>(this in {strumFullName}.Fields<T> strum, int index) {{
        switch (index) {{
{string.Join("\n", flatEnumFields.Select((field, index) => 
    $"\t\t\tcase {index}: return ref strum.{field};"))}

            default: throw new System.Exception($""NGDtuanh wrong strum index, founded: {{index}} {{(int)index}}"");
        }}
    }}

{string.Join("\n\n", enumGlobalNames.Zip(enumFields, (enumGlobalName, fields) => 
    $@"{"\t"}public static ref T ValueRW<T>(this ref {strumFullName}.Fields<T> strum, {enumGlobalName} index) {{
        switch (index) {{
{string.Join("\n", fields.Select(field => $"\t\t\tcase {enumGlobalName}.{field}: return ref strum.{field};"))}

            default: throw new System.Exception($""NGDtuanh wrong strum index, founded: {{index}} {{(int)index}}"");
        }}
    }}"))}

{string.Join("\n\n", enumGlobalNames.Zip(enumFields, (enumGlobalName, fields) => 
    $@"{"\t"}public static ref readonly T ValueRO<T>(this in {strumFullName}.Fields<T> strum, {enumGlobalName} index) {{
        switch (index) {{
{string.Join("\n", fields.Select(field => $"\t\t\tcase {enumGlobalName}.{field}: return ref strum.{field};"))}

            default: throw new System.Exception($""NGDtuanh wrong strum index, founded: {{index}} {{(int)index}}"");
        }}
    }}"))}
}}");
            
            // ----------ADD SOURCE----------
            context.AddSource(strumFullName + ".g.cs", builder.ToString());
        }
    }
    
    private record GenInput(
        TypeDeclarationSyntax Strum
      , List<string>          EnumGlobalNames
      , List<List<string>>    EnumFields) {
        public TypeDeclarationSyntax Strum           { get; } = Strum;
        public List<string>          EnumGlobalNames { get; } = EnumGlobalNames;
        public List<List<string>>    EnumFields      { get; } = EnumFields;
    }
}
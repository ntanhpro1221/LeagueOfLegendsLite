using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace NGDtuanh.BubleAsset.Generator {
    internal class ScriptGenSource {
        public static class KeyWord {
            public const string Using     = "using";
            public const string Enum      = "Enum";
            public const string Struct    = "struct";
            public const string Unmanaged = "unmanaged";
            public const string Namespace = "namespace";
            public const string Public    = "public";
            public const string Value     = "Value";
            public const string Ref       = "ref";
            public const string Where     = "where";
            public const string Void      = "void";
        }

        public List<GenericData> GenericDatas      = new();
        public List<string>      elementRealNames  = new();
        public List<string>      elementShortNames = new();
        public TypeData          TypeData          = new();
        public bool              useSourceInLast;
        public string            lastResultGenericName = null;
        public string            lastSourceGenericName = null;

        public string GenerateFileContent(string spaceName, string[] usings, string[] inherits, out string fileName) {
            BuildAllGenericsName();

            int tabNumber = 0;

            string result                                    = GenUsings(usings);
            if (usings != null && usings.Length != 0) result += "\n\n";

            // NAMESPACE OPEN {
            if (spaceName != null) {
                result += $"{KeyWord.Namespace} {spaceName} {{\n";
                ++tabNumber;
            }

            string thisType = GenThisTypeName(out fileName, TabToString(tabNumber));
            result += $"{TabToString(tabNumber)}{KeyWord.Public} {KeyWord.Struct}"
              + $" {thisType}"
              + $"{GenInherits(inherits, TabToString(tabNumber), thisType)}"
              + $"{GenConstraints(tabNumber + 1)}"
              + $" {{\n";

            // STRUCT OPEN {
            ++tabNumber;

            result +=
                $"{TabToString(tabNumber)}{KeyWord.Public} {GenerateTypeStr(tabNumber)} {KeyWord.Value};"
              + $"\n\n{GenBuildBlobFunc(TabToString(tabNumber))}"
              + $"\n\n{GenBuildBlobSelfFunc(TabToString(tabNumber), thisType)}";

            // STRUCT CLOSE }
            --tabNumber;
            result += "\n" + TabToString(tabNumber) + "}";

            // NAMESPACE CLOSE }
            if (spaceName != null) {
                result += "\n}";
                --tabNumber;
            }

            return result;
        }

        private void BuildAllGenericsName() {
            Dictionary<GenericType, int> counter = new();
            foreach (var gene in GenericDatas) {
                counter.TryAdd(gene.GenericType, 0);
                ++counter[gene.GenericType];
                gene.Name = "T" + gene.GenericType;
                if (gene.GenericType != GenericType.ValueResult
                 && gene.GenericType != GenericType.ValueSource)
                    gene.Name += counter[gene.GenericType].ToString();
            }

            if (useSourceInLast) {
                lastResultGenericName = GenericDatas[^2].Name;
                lastSourceGenericName = GenericDatas[^1].Name;
            }
            else {
                lastResultGenericName = GenericDatas[^1].Name;
            }
        }

        private string GenUsings(string[] usings) {
            string result = "";

            if (usings != null && usings.Length != 0)
                result = $"{KeyWord.Using} {string.Join($";\n{KeyWord.Using} ", usings)};";

            return result;
        }

        private string GenThisTypeName(out string fileName, string tab) {
            string name    = "";
            string generic = "";

            //NAME
            name     = "Buble_" + string.Join("_", elementShortNames);
            fileName = name;

            // generic
            generic = string.Join(", ", GenericDatas.Select(item => item.Name));

            if (GenericDatas.Count != 0)
                name = $"{name}<{generic}>";

            return name;
        }

        private string GenInherits(string[] inherits, string tab, string thisType) {
            string inheritStr = "";

            // INHERIT
            string[] inheritWithBuildable = new string[inherits.Length + 2];
            inherits.CopyTo(inheritWithBuildable, 2);
            inheritWithBuildable[0] =
                nameof(IBlobBuildable<int>)
              + '<'
              + TypeData.SourceType.ToString(
                    tab + '\t'
                  , useSourceInLast
                  , lastResultGenericName
                  , lastSourceGenericName)
              + '>';
            inheritWithBuildable[1] =
                nameof(IBlobBuildableSelf<int>)
              + '<'
              + thisType
              + '>';
            inheritStr = string.Join('\n' + tab + "  , ", inheritWithBuildable);

            if (!string.IsNullOrEmpty(inheritStr))
                inheritStr = " :\n" + tab + '\t' + inheritStr;

            return inheritStr;
        }

        private string GenConstraints(int tabNumber) {
            if (GenericDatas.Sum(item => item.Constraints.Count) == 0) return "";

            string result = "";
            string tab    = TabToString(tabNumber);

            result = "\n" + string.Join("\n", GenericDatas
                .Where(generic => generic.Constraints.Count != 0)
                .Select(generic =>
                    tab
                  + KeyWord.Where + ' '
                  + generic.Name
                  + " : "
                  + string.Join(", ", generic.Constraints.Select(item => ConstraintToString(item, generic.Name)))));

            return result;
        }

        private string GenerateTypeStr(int tabNumber) {
            return TypeData.ToString(
                TabToString(tabNumber)
              , useSourceInLast
              , lastResultGenericName
              , lastSourceGenericName);
        }

        private string TabToString(int tabNumber) => new('\t', tabNumber);

        private string ConstraintToString(ConstraintType constraint, string thisName) {
            return constraint switch {
                ConstraintType.Equatable         => $"{nameof(IEquatable<int>)}<{thisName}>"
              , ConstraintType.Enum              => KeyWord.Enum
              , ConstraintType.Struct            => KeyWord.Struct
              , ConstraintType.Unmanaged         => KeyWord.Unmanaged
              , ConstraintType.BlobBuildable     => $"{nameof(IBlobBuildable<int>)}<{GenericDatas.Last().Name}>"
              , ConstraintType.BlobBuildableSelf => $"{nameof(IBlobBuildableSelf<int>)}<{thisName}>"
              , _                                => throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null)
            };
        }

        private string GenBuildBlobFunc(string tab) {
            string result = "";

            string sourceType = TypeData.SourceType.ToString(
                tab
              , useSourceInLast
              , lastResultGenericName
              , lastSourceGenericName
              , false);

            result += $"{tab}{KeyWord.Public} {KeyWord.Void} {nameof(IBlobBuildable<int>.BuildBlob)}(" +
                $"\n{tab}    {KeyWord.Ref} {nameof(BlobBuilder)} builder"                              +
                $"\n{tab}  , {sourceType} source) {{\n";

            result += $"{tab}\t{KeyWord.Value}.{nameof(IBlobBuildable<int>.BuildBlob)}" +
                $"({KeyWord.Ref} builder, source);\n";

            result += tab + '}';

            return result;
        }

        private string GenBuildBlobSelfFunc(string tab, string thisType) {
            string result = "";

            result += $"{tab}{KeyWord.Public} {KeyWord.Void} {nameof(IBlobBuildable<int>.BuildBlob)}(" +
                $"\n{tab}    {KeyWord.Ref} {nameof(BlobBuilder)} builder"                              +
                $"\n{tab}  , {KeyWord.Ref} {thisType} source) {{\n";

            result += $"{tab}\t{KeyWord.Value}.{nameof(IBlobBuildable<int>.BuildBlob)}" +
                $"({KeyWord.Ref} builder, {KeyWord.Ref} source.{KeyWord.Value});\n";

            result += tab + '}';

            return result;
        }
    }
}
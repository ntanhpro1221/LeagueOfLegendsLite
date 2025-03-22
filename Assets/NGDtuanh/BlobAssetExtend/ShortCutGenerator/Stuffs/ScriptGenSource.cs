using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.BlobAssetExtend;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.BlobAssetExtend.Generator {
    internal class ScriptGenSource {
        public static class KeyWord {
            public const string Using     = "using";
            public const string Enum      = "Enum";
            public const string Struct    = "struct";
            public const string Namespace = "namespace";
            public const string Public    = "public";
            public const string _Ref      = "Ref";
            public const string Value     = "Value";
            public const string Ref       = "ref";
            public const string Where     = "where";
            public const string Void     = "void";
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
            
            int    tabNumber                                 = 0;
            
            string result                                    = GenUsings(usings);
            if (usings != null && usings.Length != 0) result += "\n\n";

            // NAMESPACE OPEN {
            if (spaceName != null) {
                result += $"{KeyWord.Namespace} {spaceName} {{\n";
                ++tabNumber;
            }

            result += $"{TabToString(tabNumber)}{KeyWord.Public} {KeyWord.Struct} {GenThisTypeName(inherits, out fileName, TabToString(tabNumber))}";

            result += GenConstraints(tabNumber + 1); 
            result += $" {{\n";
            
            // STRUCT OPEN {
            ++tabNumber;

            string typeStr = GenerateTypeStr(tabNumber);

            result +=
                $"{TabToString(tabNumber)}{KeyWord.Public} {GenerateTypeStr(tabNumber)} {KeyWord.Value};"
              + $"\n\n{GenBuildBlobFunc(TabToString(tabNumber))}";
            
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
            string result  = "";
            
            if (usings != null && usings.Length != 0) 
                result = $"{KeyWord.Using} {string.Join($";\n{KeyWord.Using} ", usings)};";
            
            return result;
        }

        private string GenThisTypeName(string[] inherits, out string fileName, string tab) {
            string name       = "";
            string generic    = "";
            string inheritStr = "";

            //NAME
            name     = "Buble_" + string.Join("_", elementShortNames);
            fileName = name;

            // generic
            generic = string.Join(", ", GenericDatas.Select(item => item.Name));

            // INHERIT
            string[] inheritWithBuildable = new string[inherits.Length + 1];
            inherits.CopyTo(inheritWithBuildable, 1);
            inheritWithBuildable[0] =
                nameof(IBlobBuildable<int>)
              + '<'
              + TypeData.SourceType.ToString(
                    tab + '\t'
                  , useSourceInLast
                  , lastResultGenericName
                  , lastSourceGenericName)
              + '>';
            inheritStr = string.Join('\n' + tab + "  , ", inheritWithBuildable);

            if (GenericDatas.Count != 0)
                name = $"{name}<{generic}>";
            name = name + " :\n" + tab + '\t' + inheritStr;
            return name;
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
                ConstraintType.Equatable     => $"{nameof(IEquatable<int>)}<{thisName}>"
                , ConstraintType.Enum        => KeyWord.Enum
              , ConstraintType.Struct        => KeyWord.Struct
              , ConstraintType.BlobBuildable => $"{nameof(IBlobBuildable<int>)}<{GenericDatas.Last().Name}>"
              , _                            => throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null)
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
                $"\n{tab}    {KeyWord.Ref} {nameof(BlobBuilder)} builder"                                         +
                $"\n{tab}  , {sourceType} source) {{\n";
            
            result += $"{tab}\t {KeyWord.Value}.{nameof(IBlobBuildable<int>.BuildBlob)}({KeyWord.Ref} builder, source);\n";
            
            result += tab + '}';
            
            return result;
        }
    }
}
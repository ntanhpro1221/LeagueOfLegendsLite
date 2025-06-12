using System.Collections.Generic;
using System.IO;
using System.Linq;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using Unity.Entities;
using UnityEditor;

namespace NGDtuanh.BubleAsset.Generator {
    [InitializeOnLoad]
    internal static class BubleShortCutGenerator {
        public static readonly string path;

        public static readonly string SpaceName = $"{nameof(NGDtuanh)}.{nameof(NGDtuanh.BubleAsset)}.ShortCut";

        public static readonly string[] Usings = new string[] {
            nameof(System)
          , nameof(System)   + '.' + nameof(System.Collections) + '.' + nameof(System.Collections.Generic)
          , nameof(Unity)    + '.' + nameof(Unity.Entities)
          , nameof(NGDtuanh) + '.' + nameof(NGDtuanh.BubleAsset)
          , nameof(NGDtuanh) + '.' + nameof(NGDtuanh.Utils)
          , nameof(NGDtuanh) + '.' + nameof(NGDtuanh.Collections)
        };

        public static readonly string[] Inherits = new string[] {
            nameof(IComponentData)
        };

        public const int DEPTH = 3;
        
        public static readonly TypeData[] AllTypes = new TypeData[] {
            new() {
                Name       = nameof(BubleMap<int, int>)
              , ShortName  = "Map"
              , SourceName = nameof(ICovKVPCollection<int, int>)
              , Generics = new List<GenericData> {
                    new GenericData {
                        Name = "Init_Key"
                      , Constraints = new List<ConstraintType> {
                            ConstraintType.Unmanaged
                          , ConstraintType.Equatable
                        }
                      , GenericType = GenericType.Key
                    }
                }
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                      , ConstraintType.BlobBuildable
                      , ConstraintType.BlobBuildableSelf
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
          , new() {
                Name       = nameof(BubleEnMap<GenericType, int>)
              , ShortName  = "EnMap"
              , SourceName = nameof(ICovKVPCollection<int, int>)
              , Generics = new List<GenericData> {
                    new GenericData {
                        Name = "Init_Key"
                      , Constraints = new List<ConstraintType> {
                            ConstraintType.Unmanaged
                          , ConstraintType.Enum
                        }
                      , GenericType = GenericType.Key
                    }
                }
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                      , ConstraintType.BlobBuildable
                      , ConstraintType.BlobBuildableSelf
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
           ,new() {
                Name       = nameof(BubleArray<int>)
              , ShortName  = "Array"
              , SourceName = nameof(IReadOnlyCollection<int>)
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                      , ConstraintType.BlobBuildable
                      , ConstraintType.BlobBuildableSelf
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
        };
        
        public static readonly TypeData[] AllTypesLite = new TypeData[] {
            new() {
                Name       = nameof(BubleMap<int, int>)
              , ShortName  = "Map"
              , SourceName = nameof(ICovKVPCollection<int, int>)
              , Generics = new List<GenericData> {
                    new GenericData {
                        Name = "Init_Key"
                      , Constraints = new List<ConstraintType> {
                            ConstraintType.Unmanaged
                          , ConstraintType.Equatable
                        }
                      , GenericType = GenericType.Key
                    }
                }
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
          , new() {
                Name       = nameof(BubleEnMap<GenericType, int>)
              , ShortName  = "EnMap"
              , SourceName = nameof(ICovKVPCollection<int, int>)
              , Generics = new List<GenericData> {
                    new GenericData {
                        Name = "Init_Key"
                      , Constraints = new List<ConstraintType> {
                            ConstraintType.Unmanaged
                          , ConstraintType.Enum
                        }
                      , GenericType = GenericType.Key
                    }
                }
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
           ,new() {
                Name       = nameof(BubleArray<int>)
              , ShortName  = "Array"
              , SourceName = nameof(IReadOnlyCollection<int>)
              , ChildGeneric = new GenericData {
                    Name        = "Init_ValueResult"
                  , GenericType = GenericType.ValueResult
                  , Constraints = new List<ConstraintType> {
                        ConstraintType.Struct
                    }
                }
              , SourceGeneric = new GenericData {
                    Name        = "Init_ValueSource"
                  , GenericType = GenericType.ValueSource
                  , Constraints = new List<ConstraintType> { }
                }
            }
        };
        
        static BubleShortCutGenerator() {
            path = Path.Combine(AssetHelper.GetScriptPathWithoutFileName(nameof(BubleShortCutGenerator)), "Generated");
        }

        [MenuItem("Tools/Generate Buble ShortCut")]
        public static void Generate() {
            if (Directory.Exists(path)) {
                foreach (var sourceFile in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
                    File.Delete(sourceFile);
            }

            // TEST
            // string content = BuildSource(
            //     new() { AllTypes[2], AllTypes[2] }
            //   , false
            //   , SpaceName
            //   , Usings
            //   , Inherits
            //   , out string fileName);
            // AssetHelper.WriteToFile(Path.Combine(path, fileName + ".cs"), content, false);
            
            // RELEASE
            Directory.CreateDirectory(path);
            RecursiveGenerate(new());
            
            AssetHelper.ImportFolder(path);
        }

        public static void RecursiveGenerate(List<TypeData> seeds, int curDepth = 1) {
            if (curDepth > 2) {
                string content = BuildSource(
                    seeds
                  , true
                  , SpaceName
                  , Usings
                  , Inherits
                  , out string fileName);
                AssetHelper.WriteToFile(Path.Combine(path, fileName + ".cs"), content, false);

                for (int i = 0; i < AllTypes.Length; ++i) {
                    if (seeds[^1] != AllTypes[i]) continue;

                    seeds[^1] = AllTypesLite[i];
                    
                    content = BuildSource(
                        seeds
                      , false
                      , SpaceName
                      , Usings
                      , Inherits
                      , out fileName);
                    AssetHelper.WriteToFile(Path.Combine(path, fileName + "Lite.cs"), content, false);
                    
                    seeds[^1] = AllTypes[i];
                    
                    break;
                }
            }

            if (curDepth > DEPTH) return;

            foreach (var seed in AllTypes) {
                seeds.Add(seed);
                RecursiveGenerate(seeds, curDepth + 1);
                seeds.PopBack();
            }
        }
        
        public static string BuildSource(
            List<TypeData> bubleDatas
          , bool           useSourceInLast
          , string         spaceName
          , string[]       usings
          , string[]       inherits
            , out string fileName) {
            ScriptGenSource source = new() {
                useSourceInLast = useSourceInLast
            };

            TypeData child = null;
            for (int i = bubleDatas.Count - 1; i >= 0; --i) {
                TypeData cur = new();
                cur.Build(bubleDatas[i], child);
                cur.AddToSource(source, useSourceInLast);
                child = cur;
            }
            
            return source.GenerateFileContent(spaceName, usings, inherits, out fileName);
        }
    }
}
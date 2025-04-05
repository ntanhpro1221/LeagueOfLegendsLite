using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NGDtuanh.BubleAsset.Generator {
    internal class TypeData {
        public string            Name;
        public string            ShortName;
        public string            SourceName;
        public List<GenericData> Generics;
        public GenericData       ChildGeneric;
        public GenericData       SourceGeneric;
        public TypeData          ChildType;
        public TypeData          SourceType;
        public bool              IsSource;
        
        public bool IsLast   => ChildType  == null; 
        
        public string ToString(string tab, bool useSourceInLast, string lastResultGenericName, string lastSourceGenericName, bool breakLine = true) {
            string result         = "";
            bool   haveSource     = !IsSource && !(IsLast && !useSourceInLast);
            
            result += Name + '<';

            if (Generics != null && Generics.Count != 0)
                result += string.Join(", ", Generics.Select(item => item.Name)) + ", ";

            if (IsLast) {
                if (IsSource && useSourceInLast) result += lastSourceGenericName;
                else result += lastResultGenericName;
            }
            else result        += ChildType.ToString(tab + '\t', useSourceInLast, lastResultGenericName, lastSourceGenericName, breakLine);
            if (haveSource) result += ", ";

            if (haveSource) {
                if (IsLast)
                    result += lastSourceGenericName;
                else {
                    if (breakLine) result += '\n';
                    result += tab + "\t"
                      + ChildType.SourceType.ToString(tab, useSourceInLast, lastResultGenericName, lastSourceGenericName, breakLine);
                }
            }

            result += ">";
            return result;
        }

        public void Build(TypeData rootType, TypeData childType) {
            Name       = rootType.Name;
            ShortName  = rootType.ShortName;
            SourceName = rootType.SourceName;
            // not use ref of root's type because we will change it later
            Generics      = rootType.Generics?.Select(item => item.Clone()).ToList();
            ChildGeneric  = rootType.ChildGeneric;
            SourceGeneric = rootType.SourceGeneric;

            ChildType = childType;
            SourceType = new() {
                Name       = SourceName
              , SourceName = null
              , Generics   = Generics
              , ChildType  = ChildType?.SourceType
              , SourceType = null
              , IsSource   = true
            };
        }

        public void AddToSource(ScriptGenSource source, bool useLastSource) {
            if (IsLast) {
                if (useLastSource) source.GenericDatas.Insert(0, SourceGeneric);
                source.GenericDatas.Insert(0, ChildGeneric);
            }
            if (Generics != null) source.GenericDatas.InsertRange(0, Generics);
            source.elementRealNames.Insert(0, Name);
            source.elementShortNames.Insert(0, ShortName);
            source.TypeData = this;
        }
    }
}
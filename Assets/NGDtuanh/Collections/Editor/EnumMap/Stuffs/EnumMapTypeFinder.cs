using System;
using System.Collections.Generic;
using System.Reflection;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    public class EnumMapTypeFinder {
        public readonly Type ThisType;
        public readonly Type KeyType;
        public readonly Type ValueType;

        private static readonly Type PureWrapperType = typeof(WrapperBase<>);
        private static readonly Type PureListType    = typeof(List<>);

        public EnumMapTypeFinder(FieldInfo fieldInfo) {
            try {
                var pureType = typeof(EnumMap<,>);
                ThisType = fieldInfo.FieldType;
                while (!ThisType.EqualsWithoutGeneric(pureType)) {
                    if (ThisType!.IsArray) // is array
                        ThisType = ThisType.GetElementType();
                    else if (ThisType.EqualsWithoutGeneric(PureListType)) // is list
                        ThisType  = ThisType.GenericTypeArguments[0];
                    else ThisType = ThisType.BaseType; // is children
                }

                KeyType   = ThisType!.GenericTypeArguments[0];
                ValueType = ThisType!.GenericTypeArguments[1];
                while (ValueType.EqualsWithoutGeneric(PureWrapperType))
                    ValueType = ValueType.GenericTypeArguments[0];
            }
            catch {
                Debug.LogError("Cannot find EnumMap type, the type are: " + fieldInfo.FieldType.FullName);
            }
        }
    }
}
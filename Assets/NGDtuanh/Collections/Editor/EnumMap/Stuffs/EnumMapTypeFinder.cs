using System;
using System.Reflection;
using NGDtuanh.Collections;
using NGDtuanh.Collections.PropertyWrapper;
using NGDtuanh.Utils;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    public class EnumMapTypeFinder {
        public readonly Type ThisType;
        public readonly Type KeyType;
        public readonly Type ValueType;

        private static readonly Type PureWrapperType = typeof(WrapperBase<>);

        public EnumMapTypeFinder(FieldInfo fieldInfo) {
            try {
                var pureType = typeof(EnumMap<,>);
                ThisType = fieldInfo.FieldType;
                while (!ThisType.EqualsWithoutGeneric(pureType))
                    ThisType = ThisType!.IsArray
                        ? ThisType.GetElementType()
                        : ThisType.BaseType;
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
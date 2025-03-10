using System;

namespace NGDtuanh.Utils {
    public static class TypeExtensions {
        public static Type WithoutGeneric(this Type type)
            => type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type;

        public static bool EqualsWithoutGeneric(this Type alice, Type bob)
            => alice.WithoutGeneric() == bob.WithoutGeneric();
    }
}
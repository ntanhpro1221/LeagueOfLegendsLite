using NGDtuanh.Collections;

namespace MyCustomPatterns.Collections.Editor {
    public static class EnumMapPropertyName {
        private enum TmpEnum { }

        public static readonly string Keys              = nameof(EnumMap<TmpEnum, int>._Keys);
        public static readonly string Values            = nameof(EnumMap<TmpEnum, int>._Values);
        public static readonly string KeyNames          = nameof(EnumMap<TmpEnum, int>._KeyNames);
        public static readonly string EditorSessionCode = nameof(EnumMap<TmpEnum, int>._EditorSessionCode);
        public static readonly string KeySynced         = nameof(EnumMap<TmpEnum, int>._KeySynced);
    }
}
namespace NGDtuanh.Utils.Editor {
    internal static class ScriptReloadDetectorHub {
        public static long GetCodeSkipDefault() {
            if (IsCodeDefault) ChangeCode();
            return _SessionCode;
        }

        private static long _SessionCode;
        private static bool IsCodeDefault => _SessionCode == default;
        private static void ChangeCode()  => ++_SessionCode;

        private static void ChangeCodeSkipDefault() {
            do {
                ChangeCode();
            } while (IsCodeDefault);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptReload() => ChangeCodeSkipDefault();
    }
}
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Utils.Editor {
    public static class ScriptReloadDetector {
        /// <summary>
        /// check your editor session code to see if it is different from current editor session code
        /// (in that case editor was reloaded). After the checking you need to call <see cref="SyncSessionCode"/>
        /// method to sync your session code to newest.
        /// </summary>
        /// <param name="localEditorSessionCode">store it in your code</param>
        public static bool IsReloaded(in Hash128 localEditorSessionCode) {
            return localEditorSessionCode != EditorSessionCode;
        }

        public static bool IsReloaded(SerializedProperty localEditorSessionCode) {
            return localEditorSessionCode.hash128Value != EditorSessionCode;
        }

        /// <summary>
        /// Sync your editor session code to the newest value
        /// </summary>
        public static void SyncMySessionCode(ref Hash128 localEditorSessionCode) {
            localEditorSessionCode = EditorSessionCode;
        }

        public static void SyncMySessionCode(SerializedProperty localEditorSessionCode) {
            localEditorSessionCode.hash128Value = EditorSessionCode;
        }

        #region PRIVATE

        private static UnityEngine.Hash128 _EditorSessionCode;

        /// <summary>
        /// Always not equals default value
        /// </summary>
        private static UnityEngine.Hash128 EditorSessionCode {
            get {
                if (_EditorSessionCode == default) RollNewCodeSkipDefault();
                return _EditorSessionCode;
            }
        }

        /// <summary>
        /// Always avoid default value
        /// </summary>
        private static void RollNewCodeSkipDefault() {
            do {
                _EditorSessionCode.Random();
            } while (_EditorSessionCode == default);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptReload() => RollNewCodeSkipDefault();

        #endregion
    }
}
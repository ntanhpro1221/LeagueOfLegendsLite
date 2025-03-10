namespace NGDtuanh.Utils.Editor {
    public struct ScriptReloadDetector {
        private long _SessionCode;

        public bool IsReloaded_Read() {
            return _SessionCode != ScriptReloadDetectorHub.GetCodeSkipDefault();
        }

        public bool IsReloaded_Update() {
            var result = IsReloaded_Read();
            _SessionCode = ScriptReloadDetectorHub.GetCodeSkipDefault();
            return result;
        }
    }
}
using System.IO;
using UnityEditor;

namespace NGDtuanh.Utils.Editor {
    public static class AssetHelper {
        /// <param name="fileName">File name without extensions</param>
        public static string GetScriptPath(string fileName) {
            var guids        = AssetDatabase.FindAssets($"t:Script {fileName}");
            var fullFileName = fileName + ".cs";
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path).Equals(fullFileName))
                    return path;
            }
            
            throw new FileNotFoundException();
        }

        public static bool IsExistSameFile(string path, string content) {
            if (!File.Exists(path)) return false;
            if (!File.ReadAllText(path).Equals(content)) return false;
            
            return true;
        }
        
        public static string GetScriptPathWithoutFileName(string fileName)
            => Path.GetDirectoryName(GetScriptPath(fileName));

        public static void WriteToFile(string path, string content, bool importNow = true) {
            File.WriteAllText(path, content, System.Text.Encoding.Unicode);
            if (importNow) AssetDatabase.ImportAsset(path);
        }

        public static void SafeWriteToFile(string path, string content) {
            if (!IsExistSameFile(path, content))
                WriteToFile(path, content);
        }

        public static void ImportFolder(string path)
            => AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
    }
}
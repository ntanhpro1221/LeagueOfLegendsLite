using System.IO;
using UnityEditor;

namespace NGDtuanh.Utils.Editors {
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

        public static string GetScriptPathWithoutFileName(string fileName)
            => Path.GetDirectoryName(GetScriptPath(fileName));

        public static void WriteToFile(string path, string content, bool importNow = true) {
            File.WriteAllText(path, content, System.Text.Encoding.Unicode);
            AssetDatabase.ImportAsset(path);
        }
    }
}
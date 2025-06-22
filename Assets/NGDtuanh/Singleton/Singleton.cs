using System.Reflection;
using UnityEngine;

namespace NGDtuanh.Singleton {
    public class Singleton<TSelf> : MonoBehaviour where TSelf : MonoBehaviour {
        private static TSelf _Instance;

        public static TSelf Instance {
            get {
                if (_Instance != null) return _Instance;

                _Instance ??= FindFirstObjectByType<TSelf>(FindObjectsInactive.Include);
                if (_Instance == null) return _Instance;

                typeof(TSelf)
                    .GetMethod("OnTouched", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(Instance, null);

                return _Instance;
            }
        }

        protected virtual void OnTouched() { }

        protected virtual void Awake() => DontDestroyOnLoad(gameObject);

        protected virtual void OnDestroy() { }
    }
}
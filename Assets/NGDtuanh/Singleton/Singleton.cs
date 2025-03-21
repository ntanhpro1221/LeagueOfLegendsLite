using System.Reflection;
using UnityEngine;

namespace NGDtuanh.Singleton {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _Instance;

        public static T Instance {
            get {
                if (_Instance != null) return _Instance;

                _Instance ??= FindFirstObjectByType<T>();
                if (_Instance == null) return _Instance;

                typeof(T)
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

using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGDtuanh.Utils.Editor {
    [InitializeOnLoad]
    public static class DelayInvoker {
        private static Queue<(double, Action)> ActionQueue;

        static DelayInvoker() {
            ActionQueue              =  new();
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate() {
            double curTime = EditorApplication.timeSinceStartup;
            while (ActionQueue.Count != 0 && ActionQueue.Peek().Item1 <= curTime)
                ActionQueue.Dequeue().Item2?.Invoke();
        }

        public static void Invoke(Action callback, double delay) {
            if (callback == null) return;
            ActionQueue.Enqueue((EditorApplication.timeSinceStartup + delay, callback));
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGDtuanh.Utils.Editor {
    public static class SerializedArrayExtensions {
        /// <summary>
        /// You should use getter in <see cref="SerializedPropertyExtensions"/> to avoid addtional allocation
        /// </summary>
        public static int IndexOf<T>(this SerializedProperty     array
                                   , T                           value
                                   , Func<SerializedProperty, T> getValue)
            where T : IEquatable<T> {
            return IndexOf(array, (item, _) => getValue(item).Equals(value));
        }

        public static int IndexOf(this SerializedProperty             array
                                , Func<SerializedProperty, int, bool> predict) {
            int id = array.arraySize - 1;
            for (; id >= 0; --id)
                if (predict(array.GetArrayElementAtIndex(id), id))
                    break;
            return id;
        }

        /// <summary>
        /// You should use getter in <see cref="SerializedPropertyExtensions"/> to avoid addtional allocation
        /// </summary>
        public static List<T> ToList<T>(this SerializedProperty     array
                                      , Func<SerializedProperty, T> getValue) {
            List<T> result = new(array.arraySize);
            for (int i = 0; i < array.arraySize; ++i) result.Add(getValue(array.GetArrayElementAtIndex(i)));
            return result;
        }

        /// <summary>
        /// You should use getter in <see cref="SerializedPropertyExtensions"/> to avoid addtional allocation
        /// </summary>
        public static T[] ToArray<T>(this SerializedProperty     array
                                   , Func<SerializedProperty, T> getValue) {
            T[] result                                          = new T[array.arraySize];
            for (int i = 0; i < array.arraySize; ++i) result[i] = getValue(array.GetArrayElementAtIndex(i));
            return result;
        }

        public static void Swap(this SerializedProperty array, int leftId, int rightId) {
            if (leftId > rightId) Swapper.Swap(ref leftId, ref rightId);
            array.MoveArrayElement(leftId,      rightId);
            array.MoveArrayElement(rightId - 1, leftId);
        }

        public static SerializedProperty Back(this SerializedProperty array)
            => array.arraySize != 0
                ? array.GetArrayElementAtIndex(array.arraySize - 1)
                : null;

        public static SerializedProperty Front(this SerializedProperty array)
            => array.arraySize != 0
                ? array.GetArrayElementAtIndex(0)
                : null;

        public static void PushBack(this SerializedProperty array)
            => ++array.arraySize;

        public static void PushFront(this SerializedProperty array)
            => array.InsertArrayElementAtIndex(0);

        public static SerializedProperty PopBack(this SerializedProperty array) {
            if (array.arraySize == 0) return null;
            var value = array.Back();
            --array.arraySize;
            return value;
        }

        public static SerializedProperty PopFront(this SerializedProperty array) {
            if (array.arraySize == 0) return null;
            var value = array.Front();
            array.DeleteArrayElementAtIndex(0);
            return value;
        }
    }
}
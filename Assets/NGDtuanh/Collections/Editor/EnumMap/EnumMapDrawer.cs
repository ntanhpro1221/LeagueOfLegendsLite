using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Collections.Editor {
    [CustomPropertyDrawer(typeof(EnumMap<,>), true)]
    public class EnumMapDrawer : IDrawer<EnumMapInstanceDrawer> { }
}
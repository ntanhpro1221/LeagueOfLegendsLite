using System;
using System.Linq;
using UnityEngine;

namespace NGDtuanh.Utils.Editor {
    public class EnumData {
        public string[] Names  { get; private set; }
        public int[]    Values { get; private set; }
        public int      Count  { get; private set; }

        public EnumData SetData(Type enumType) {
            Names  = Enum.GetNames(enumType);
            Values = Enum.GetValues(enumType).Cast<int>().ToArray();
            Count  = Names.Length;

            return this;
        }
    }
}
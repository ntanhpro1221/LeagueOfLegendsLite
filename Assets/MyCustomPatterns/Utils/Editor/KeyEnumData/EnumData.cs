namespace NGDtuanh.Utils.Editor {
    public class EnumData {
        public string[] Names  { get; private set; }
        public int[]    Values { get; private set; }
        public int      Count  { get; private set; }

        public EnumData SetData(string[] names, int[] values) {
            Names  = names;
            Values = values;
            Count  = names.Length;

            return this;
        }
    }
}
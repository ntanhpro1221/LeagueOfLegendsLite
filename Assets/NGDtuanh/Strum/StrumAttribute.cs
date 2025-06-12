using System;

namespace NGDtuanh.Strum {
    [AttributeUsage(AttributeTargets.Enum)]
    public class StrumAttribute : Attribute {
        public string StrumName { get; set; }

        public StrumAttribute(string StrumName) {
            this.StrumName = StrumName;
        }
    }
}
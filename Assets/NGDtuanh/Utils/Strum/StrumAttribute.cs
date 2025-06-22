using System;

namespace NGDtuanh.Utils {
    [AttributeUsage(
        AttributeTargets.Struct
      | AttributeTargets.Class
      | AttributeTargets.Enum)]
    public class StrumAttribute : Attribute {
        public StrumAttribute(params Type[] Enums) { }
    }
}
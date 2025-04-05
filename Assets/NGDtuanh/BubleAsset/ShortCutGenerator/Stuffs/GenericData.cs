using System.Collections.Generic;

namespace NGDtuanh.BubleAsset.Generator {
    internal class GenericData : ICloneable<GenericData> {
        public string               Name;
        public List<ConstraintType> Constraints = new();
        public GenericType          GenericType;

        public GenericData Clone() => new() {
            Name        = Name
          , Constraints = new(Constraints)
          , GenericType = GenericType
        };
    }
}
using System;

public static partial class Strum {
    [NGDtuanh.Utils.Strum(typeof(CC_DisableId))]
    public static partial class CC_Disable {
        [Serializable]
        partial struct Fields<T> { }
    }
}

public enum CC_DisableId {
    Move
  , Attack
  , ActiveItem
}
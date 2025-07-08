public static partial class Strum {
    [NGDtuanh.Utils.Strum(typeof(InputRequestId))]
    public static partial class InputRequest { }
}

public enum InputRequestId {
    Move
  , CancelMove
  , UpgradeSkill
  , BuyItem
  , SellItem
  , MoveItem
}
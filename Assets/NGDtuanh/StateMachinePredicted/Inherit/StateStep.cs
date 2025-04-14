using System;

namespace NGDtuanh.Entities.StateMachine {
    [Flags]
    public enum StateStep {
        Exit        = 1 << 0
      , Enter       = 1 << 1
      , Update      = 1 << 2
      , FixedUpdate = 1 << 3
    }
}
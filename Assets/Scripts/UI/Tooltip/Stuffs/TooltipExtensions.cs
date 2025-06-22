using System;

public static class TooltipExtensions {
    public static TooltipWindowType GetWindowType(this Type type) {
        if (type == typeof(TooltipWindow_EffectBar)) return TooltipWindowType.EffectBar;
        if (type == typeof(TooltipWindow_Item)) return TooltipWindowType.Item;
        if (type == typeof(TooltipWindow_Simple)) return TooltipWindowType.Simple;
        if (type == typeof(TooltipWindow_Skill)) return TooltipWindowType.Skill;
        
        throw new Exception($"NGDtuanh: fail to get window type from {type.Name}");
    }
}
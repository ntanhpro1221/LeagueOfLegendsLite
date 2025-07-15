using ActiveCondition = Strum.ItemActiveCond.Fields<bool>;

public static class ItemActiveConditionExtensions {
    public static void UpdateFrom(this ref ActiveCondition cond, in InputCastData castData) {
        cond.PointToWalkable = castData.isHitWalkableGround;
        cond.PointToTarget   = castData.isHitEnemy;
    }

    public static bool CheckCondOf(this in ActiveCondition patternCond, in ActiveCondition curCond) {
        foreach (var index in Strum.ItemActiveCond.Indexes)
            if (patternCond[index] && !curCond[index])
                return false;
        return true;
    }
}
using System;

[Serializable]
public struct ItemActiveCondition {
    public bool pointToTarget;
    public bool pointToWalkable;

    public void UpdateAll(in InputCastData castData) {
        pointToWalkable = castData.isHitWalkableGround;
        pointToTarget   = castData.isHitEnemy;
    }

    public readonly bool CheckOK(in ItemActiveCondition curCondition) {
        if (pointToTarget   && !curCondition.pointToTarget) return false;
        if (pointToWalkable && !curCondition.pointToWalkable) return false;

        return true;
    }
}
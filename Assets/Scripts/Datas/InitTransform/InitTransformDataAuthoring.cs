using System.Collections.Generic;
using BlobAssetExtend;
using NGDtuanh.Collections.EnumMap;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class InitTransformDataAuthoring : MonoBehaviour {
    public EnumMap<TransformKeys.BigMonster, Transform>        bigMonster;
    public List<Transform>                                     champion_Blue;
    public List<Transform>                                     champion_Red;
    public EnumMap<TransformKeys.MinionPath, List<Transform>>  minionPath_Blue;
    public EnumMap<TransformKeys.MinionPath, List<Transform>>  minionPath_Red;
    public EnumMap<TransformKeys.Monster, Transform>           monster_Blue;
    public EnumMap<TransformKeys.Monster, Transform>           monster_Red;
    public EnumMap<TransformKeys.ScuttlePath, List<Transform>> scuttlePath;
    public EnumMap<TransformKeys.Tower, Transform>             tower_Blue;
    public EnumMap<TransformKeys.Tower, Transform>             tower_Red;

    private class Baker : Baker<InitTransformDataAuthoring> {
        public override void Bake(InitTransformDataAuthoring authoring) {
            using var builder = new BlobBuilder(Allocator.Temp);

            var dataRef = new InitTransformData_Reference();
            builder.CreateReferenceInBaker(this, authoring.bigMonster,      ref dataRef._BigMonster);
            builder.CreateReferenceInBaker(this, authoring.champion_Blue,   ref dataRef._Champion_Blue);
            builder.CreateReferenceInBaker(this, authoring.champion_Red,    ref dataRef._Champion_Red);
            builder.CreateReferenceInBaker(this, authoring.minionPath_Blue, ref dataRef._MinionPath_Blue);
            builder.CreateReferenceInBaker(this, authoring.minionPath_Red,  ref dataRef._MinionPath_Red);
            builder.CreateReferenceInBaker(this, authoring.monster_Blue,    ref dataRef._Monster_Blue);
            builder.CreateReferenceInBaker(this, authoring.monster_Red,     ref dataRef._Monster_Red);
            builder.CreateReferenceInBaker(this, authoring.scuttlePath,     ref dataRef._ScuttlePath);
            builder.CreateReferenceInBaker(this, authoring.tower_Blue,      ref dataRef._Tower_Blue);
            builder.CreateReferenceInBaker(this, authoring.tower_Red,       ref dataRef._Tower_Red);

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, dataRef);
        }
    }
}
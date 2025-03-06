using System;
using BlobAssetExtend;
using PropertySetUtil;
using Unity.Entities;
using UnityEngine;

public struct ChampionData {
    // just store necessary info to run, some data such as name and description just to show to user.
    public ChampionId                                           id;
    public BlobHashMap<EquatableEnum<ChampionStatsType>, float> stats;
    public Entity                                               prefab;


    [Serializable]
    public class Managed : IComponentData {
        public ChampionId                            id;
        public int                                   num;
        public PropertySet<ChampionStatsType, float> stats;
        public GameObject                            prefab;
        public string                                name;
        public string                                description;
        public Sprite                                avatar;
        public Sprite                                passiveAvatar;
        public Sprite[]                              skillAvatars;
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class MinionWaveDataManaged {
    public List<MinionId> minions;
    public bool           isFixedSpawn;
    public float          firstWaveTime;
    public float          waveInterval;
}
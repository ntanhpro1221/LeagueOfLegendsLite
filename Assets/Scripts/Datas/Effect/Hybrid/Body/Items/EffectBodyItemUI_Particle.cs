using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectBodyItemUI_Particle : IEffectBodyItemUI {
    private ParticleSystem _PS;

    public ParticleSystem PS {
        get {
            if (_PS == null) _PS = GetComponent<ParticleSystem>();
            return _PS;
        }
    }

    public override void Stop() {
        PS.Stop(withChildren: true);
    }

    public override void Play() {
        PS.Play(withChildren: true);
    }
}
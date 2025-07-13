using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class NumberPopup : MonoBehaviour {
    #region COMPONENTS

    public enum Id {
        NomDmg
      , Gold
      , Exp
    }

    [Serializable] public class Curves {
        [SerializeField] private ParticleSystem.MinMaxCurve _LifeTimeFromValue;
        [SerializeField] private ParticleSystem.MinMaxCurve _AlphaOverTime;
        [SerializeField] private ParticleSystem.MinMaxCurve _FontSizeOverTime;
        [SerializeField] private MinMaxCurve3               _PosOverTime;

        public void SetupFor(NumberPopup target, int value) {
            target._Curves = this;
            target._LifeTime = _LifeTimeFromValue.Evaluate(value
              , target._CurveFactor = UnityEngine.Random.value);
            target._Timer = 0;
            UpdateFor(target, 0);
        }

        private void UpdateFor(NumberPopup target, float time) {
            var leftFactor = target._CurveFactor;

            target._Text.alpha          = _AlphaOverTime.Evaluate(time, leftFactor);
            target._Text.fontSize       = _FontSizeOverTime.Evaluate(time, leftFactor);
            target.Trans.localPosition = _PosOverTime.Evaluate(time, leftFactor);
        }

        /// <returns>True when this popup is expired.</returns>
        public bool UpdateFor(NumberPopup target) {
            if ((target._Timer += Time.deltaTime) > target._LifeTime) return true;

            UpdateFor(target, target._Timer / target._LifeTime);
            return false;
        }
    }

    [Serializable] public class Style {
        [SerializeField] private Color  _Color;
        [SerializeField] private string _Prefix;
        [SerializeField] private string _Suffix;
        [SerializeField] private Curves _Curves;

        public void SetupFor(NumberPopup target, int value) {
            target._Text.color = _Color;
            target._Text.text  = $"{_Prefix}{value}{_Suffix}";

            _Curves.SetupFor(target, value);
        }
    }

    #endregion

    #region FIELDS

    private TextMeshPro _Text;
    public  Transform   Trans { get; private set; }

    private Curves _Curves;
    private float  _CurveFactor;
    private float  _LifeTime;
    private float  _Timer;

    #endregion

    private void Awake() {
        _Text  = GetComponent<TextMeshPro>();
        Trans = transform;
    }

    private void Start() => Trans.rotation = Camera.main!.transform.rotation;

    public void Setup(Id id, int value) => GameSO.NumPopupStyle[id].SetupFor(this, value);

    private void Update() {
        if (_Curves.UpdateFor(this)) NumberPopupPool.Instance.ReleaseItem(this);
    }
}
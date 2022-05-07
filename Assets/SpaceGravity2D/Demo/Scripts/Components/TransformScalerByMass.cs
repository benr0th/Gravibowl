using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for scaling transform, depending on mass of target CelestialBody.
    /// Scale is changing in Update only if target's mass is changing.
    /// </summary>
    public class TransformScalerByMass : MonoBehaviour
    {
        public enum ScaleFunctionType
        {
            /// <summary>
            /// Scale dependency function: scale = Mass ^ (1/2).
            /// </summary>
            SqrPow2,

            /// <summary>
            /// Scale dependency function: scale = Mass ^ (1/3).
            /// </summary>
            SqrPow3
        }

        public ScaleFunctionType ScaleType = ScaleFunctionType.SqrPow3;

        public CelestialBody CelestialBodyRef;

        [Range(0.1f, 5f)]
        public float ScaleMultiplier = 1f;

        private float _scale = 1;

        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            if (CelestialBodyRef != null)
            {
                var scale = GetCurrentScale((float)CelestialBodyRef.Mass);
                if (scale != this._scale)
                {
                    this._scale = scale;
                    transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }

        private float GetCurrentScale(float Mass)
        {
            switch (ScaleType)
            {
                case ScaleFunctionType.SqrPow2:
                    if (Mass < 0)
                    {
                        Mass = 0;
                    }
                    return Mathf.Sqrt(Mass) * ScaleMultiplier;
                case ScaleFunctionType.SqrPow3:
                    return CelestialBodyUtils.GetThirdPowerRootSafe(Mass) * ScaleMultiplier;
            }
            return ScaleMultiplier;
        }
    }
}
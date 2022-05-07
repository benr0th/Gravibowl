using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for creating and updating handle collider for target body velocity.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class VelocityHandle : MonoBehaviour
    {
        public CelestialBody CelestialBodyRef;
        private Vector3 _lastHandlePosition;

        private void OnEnable()
        {
            if (CelestialBodyRef == null)
            {
                CelestialBodyRef = GetComponentInParent<CelestialBody>();
            }
            if (CelestialBodyRef != null)
            {
                _lastHandlePosition = GetVelocityPoint();
                transform.localPosition = _lastHandlePosition;
            }
        }

        private Vector3 GetVelocityPoint()
        {
            if (CelestialBodyRef != null)
            {
                var v = (Vector3)CelestialBodyRef.Velocity;
                var minDist = CelestialBodyUtils.GetThirdPowerRootSafe((float)CelestialBodyRef.Mass);
                return v.normalized * (minDist + v.magnitude);
            }
            return transform.localPosition;
        }


        private void LateUpdate()
        {
            if (transform.localPosition != _lastHandlePosition)
            {
                if (CelestialBodyRef != null)
                {
                    CelestialBodyRef.Velocity += new Vector3d(transform.localPosition - _lastHandlePosition);
                }
            }
            _lastHandlePosition = GetVelocityPoint();
            transform.localPosition = _lastHandlePosition;
        }
    }
}
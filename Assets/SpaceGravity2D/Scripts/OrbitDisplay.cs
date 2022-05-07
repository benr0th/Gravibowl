using UnityEngine;

namespace SpaceGravity2D
{
    /// <summary>
    /// Component for displaying current orbit of CelestialBody.
    /// CelestialBody should be attached to same Game Object.
    /// </summary>
    [AddComponentMenu("SpaceGravity2D/OrbitDisplay")]
    public class OrbitDisplay : MonoBehaviour
    {
        /// <summary>
        /// Material for LineRenderer.
        /// </summary>
        public Material OrbitLineMaterial;

        /// <summary>
        /// Line width.
        /// </summary>
        public float Width = 0.1f;

        /// <summary>
        /// Path points count.
        /// More points - better precision.
        /// </summary>
        public int OrbitPointsCount = 50;

        /// <summary>
        /// Max distance for orbit display in world units.
        /// </summary>
        public float MaxOrbitPointsDistance = 100;

        /// <summary>
        /// Reference to line renderer.
        /// </summary>
        public LineRenderer LineRenderer;

        private CelestialBody _body;

        private Vector3[] points = null;

        private void OnEnable()
        {
            if (LineRenderer == null)
            {
                CreateLineRend();
            }
            if (_body == null)
            {
                _body = GetComponentInParent<CelestialBody>();
                if (_body == null)
                {
                    Debug.Log("SpaceGravity2D: Orbit Display can't find celestial body on " + name);
                    enabled = false;
                }
            }
            LineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            LineRenderer.enabled = false;
        }

        private void LateUpdate()
        {
            DrawOrbit();
        }

        private void DrawOrbit()
        {
            if (!LineRenderer)
            {
                CreateLineRend();
            }
            if (LineRenderer.enabled)
            {
                LineRenderer.startWidth = Width;
                LineRenderer.endWidth = Width;
                _body.GetOrbitPointsNoAlloc(ref points, OrbitPointsCount, false, MaxOrbitPointsDistance);

                LineRenderer.positionCount = points.Length;
                for (int i = 0; i < points.Length; i++)
                {
                    LineRenderer.SetPosition(i, points[i]);
                }

                LineRenderer.loop = _body.OrbitData.Eccentricity < 1.0;
            }
        }

        [System.Obsolete("Use LineRenderer.enabled = false instead.")]
        public void HideOrbit()
        {
            if (LineRenderer)
            {
                LineRenderer.enabled = false;
            }
        }

        private void CreateLineRend()
        {
            GameObject lineRendObj = new GameObject("OrbitLineRenderer");
            lineRendObj.transform.SetParent(transform);
            lineRendObj.transform.position = Vector3.zero;
            LineRenderer = lineRendObj.AddComponent<LineRenderer>();
            LineRenderer.material = OrbitLineMaterial;
        }
    }
}
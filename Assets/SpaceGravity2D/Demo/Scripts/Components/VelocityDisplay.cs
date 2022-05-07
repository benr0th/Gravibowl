using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for displaying velocity vector of attached CelestialBody instance.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(CelestialBody))]
    public class VelocityDisplay : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float ArrowHeadRatio = 0.2f;
        public float ArrowHeadMaxSize = 1f;
        public float ArrowHeadWidth = 3f;

        public float VelocityWidth = 1f;
        public float CircleWidth = 1f;

        public float VelocityScale = 1f;

        public float MinimalRadius = 0.01f;
        public int CirclePointsCount = 60;

        private CelestialBody _celestialBody;
        private LineRenderer _velocityLine;
        private LineRenderer _arrowLine;
        private LineRenderer _circleLine;
        public Material LineMaterial;

        private void Awake()
        {
            _celestialBody = GetComponent<CelestialBody>();

            var velocityLineObj = new GameObject("VelocityLine");
            velocityLineObj.transform.SetParent(transform);
            _velocityLine = velocityLineObj.AddComponent<LineRenderer>();
            _velocityLine.material = LineMaterial;
            _velocityLine.useWorldSpace = true;

            var arrowLineObj = new GameObject("ArrowLine");
            arrowLineObj.transform.SetParent(transform);
            _arrowLine = arrowLineObj.AddComponent<LineRenderer>();
            _arrowLine.material = LineMaterial;
            _arrowLine.useWorldSpace = true;

            var circleLineObj = new GameObject("CircleLine");
            circleLineObj.transform.SetParent(transform);
            _circleLine = circleLineObj.AddComponent<LineRenderer>();
            _circleLine.loop = true;
            _circleLine.material = LineMaterial;
            _circleLine.useWorldSpace = true;
        }

        private void OnEnable()
        {
            if (_arrowLine != null)
            {
                _arrowLine.enabled = true;
            }
            if (_circleLine != null)
            {
                _circleLine.enabled = true;
            }
            if (_velocityLine != null)
            {
                _velocityLine.enabled = true;
            }
        }

        private void OnDisable()
        {
            _arrowLine.enabled = false;
            _circleLine.enabled = false;
            _velocityLine.enabled = false;
        }

        private void LateUpdate()
        {
            var minDist = CelestialBodyUtils.GetThirdPowerRootSafe((float)_celestialBody.Mass) * 0.8f;
            DrawVelocity(minDist);
            DrawCircle(minDist);
        }

        private void DrawVelocity(float minDist)
        {
            var velocity = _celestialBody.UseKeplerMotion ? _celestialBody.RelativeVelocity : _celestialBody.Velocity;
            var velocityScalar = velocity.magnitude;
            if (velocityScalar > 1e-3f)
            {
                var normalizedDirection = velocity / velocityScalar;

                DrawArrow(
                    startPoint: transform.position + (Vector3)(normalizedDirection * minDist),
                    direction: (Vector3)normalizedDirection,
                    distance: (float)velocityScalar * VelocityScale,
                    width: VelocityWidth,
                    headWidth: ArrowHeadWidth,
                    headRatio: ArrowHeadRatio,
                    maxHeadSize: ArrowHeadMaxSize
                );
            }
            else
            {
                _velocityLine.enabled = false;
                _arrowLine.enabled = false;
            }
        }

        private void DrawArrow(Vector3 startPoint, Vector3 direction, float distance, float width, float headWidth, float headRatio, float maxHeadSize)
        {
            float headSize = distance * headRatio;
            if (headSize > maxHeadSize)
            {
                headSize = maxHeadSize;
            }

            _velocityLine.enabled = true;
            _velocityLine.endWidth = VelocityWidth;
            _velocityLine.startWidth = VelocityWidth;
            _velocityLine.positionCount = 2;
            _velocityLine.SetPosition(0, startPoint);
            _velocityLine.SetPosition(1, startPoint + direction * (distance - headSize));

            _arrowLine.enabled = true;
            _arrowLine.startWidth = ArrowHeadWidth;
            _arrowLine.endWidth = 0.001f;
            _arrowLine.positionCount = 2;
            _arrowLine.SetPosition(0, startPoint + direction * (distance - headSize));
            _arrowLine.SetPosition(1, startPoint + direction * distance);

        }

        private void DrawCircle(float radius)
        {
            if (radius > MinimalRadius)
            {
                _circleLine.enabled = true;
                _circleLine.startWidth = CircleWidth;
                _circleLine.endWidth = CircleWidth;
                Vector3 axysX;
                Vector3 axysY;
                if (_celestialBody.AttractorRef != null && _celestialBody.OrbitData.IsValidOrbit)
                {
                    axysX = (Vector3)_celestialBody.OrbitData.SemiMinorAxisBasis;
                    axysY = (Vector3)_celestialBody.OrbitData.SemiMajorAxisBasis;
                }
                else
                {
                    axysX = _celestialBody.Velocity.sqrMagnitude > 0 ? (Vector3)_celestialBody.Velocity.normalized : (Vector3)_celestialBody.SimControlRef.EclipticUp;
                    axysY = (Vector3)CelestialBodyUtils.CrossProduct(axysX, (Vector3)_celestialBody.SimControlRef.EclipticNormal);
                }
                _circleLine.positionCount = CirclePointsCount;
                for (int i = 0; i < CirclePointsCount; i++)
                {
                    float angle = i * (2f * Mathf.PI / CirclePointsCount);
                    var pointPosition = transform.position + axysX * Mathf.Sin(angle) * radius + axysY * Mathf.Cos(angle) * radius;
                    _circleLine.SetPosition(i, pointPosition);
                }
            }
            else
            {
                _circleLine.enabled = false;
            }
        }
    }
}
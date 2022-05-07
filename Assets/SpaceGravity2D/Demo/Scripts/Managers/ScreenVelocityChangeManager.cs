using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Singleton, which handles mouse input, and allows to move colliders with VelocityHandle component on scene by mousedrag.
    /// </summary>
    public class ScreenVelocityChangeManager : MonoBehaviour
    {
        private VelocityHandle _currentTarget;

        private void Start()
        {
            InputProvider.Instance.OnPointerDownEvent += OnPointerDown;
            InputProvider.Instance.OnPointerUpEvent += OnPointerUp;
            InputProvider.Instance.OnPointerStayDownEvent += OnPointerInput;
        }

        private void OnPointerDown(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(pointerScreenPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    var handle = hit.collider.GetComponent<VelocityHandle>();
                    if (handle != null && handle.CelestialBodyRef != null)
                    {
                        _currentTarget = handle;
                    }
                }
            }
        }

        private void OnPointerUp(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex)
        {
            _currentTarget = null;
        }

        private void OnPointerInput(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex)
        {
            if (_currentTarget != null)
            {
                if (_currentTarget.CelestialBodyRef != null)
                {
                    var ray = Camera.main.ScreenPointToRay(pointerScreenPosition);
                    Vector3 orbitNormal = _currentTarget.CelestialBodyRef.GetVelocityPlaneNormal();
                    var currentRayPlanarHitPoint = CelestialBodyUtils.GetRayPlaneIntersectionPoint(_currentTarget.transform.position, orbitNormal, ray.origin, ray.direction);
                    if (!float.IsNaN(currentRayPlanarHitPoint.x) && !float.IsInfinity(currentRayPlanarHitPoint.x))
                    {
                        var lastRay = Camera.main.ScreenPointToRay(lastPointerPosition);
                        var lastRayPlanarHitPoint = CelestialBodyUtils.GetRayPlaneIntersectionPoint(_currentTarget.transform.position, orbitNormal, lastRay.origin, lastRay.direction);
                        if (!float.IsNaN(currentRayPlanarHitPoint.x) && !float.IsInfinity(currentRayPlanarHitPoint.x))
                        {
                            var delta = currentRayPlanarHitPoint - lastRayPlanarHitPoint;
                            _currentTarget.transform.position += delta;
                        }
                    }
                }
            }
        }
    }
}
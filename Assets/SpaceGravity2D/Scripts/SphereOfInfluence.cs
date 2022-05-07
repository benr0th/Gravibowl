using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceGravity2D
{
    /// <summary>
    /// Basic static Sphere of Influence component script, alternative to Dynamic Attractor Changing. 
    /// If attached to gameobject whith no colliders, new collider will be created.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("SpaceGravity2D/SphereOfInfluence")]
    public class SphereOfInfluence : MonoBehaviour
    {
        /// <summary>
        /// Reference to collider.
        /// </summary>
        [FormerlySerializedAs("detector")]
        public SphereCollider Detector;

        /// <summary>
        /// Reference to celestial body.
        /// </summary>
        [FormerlySerializedAs("body")]
        public CelestialBody Target;

        /// <summary>
        /// Radius of collider detector in world units.
        /// </summary>
        [Header("Range of ingluence:")]
        [FormerlySerializedAs("TriggerRadius")]
        public float TriggerRadius;

        /// <summary>
        /// If true and attractor is not null, range of influence will be calculated automaticaly. 
        /// Useful for making first approach.
        /// </summary>
        [Header("Calculate radius value based on orbit data:")]
        [FormerlySerializedAs("useAutoROI")]
        public bool UseAutoROI = false;

        /// <summary>
        /// Dynamic attractor changing of celestial body is full alternative to this component.
        /// </summary>
        [Space(15)]
        [FormerlySerializedAs("ignoreBodiesWithDynamicAttrChanging")]
        public bool IgnoreBodiesWithDynamicAttrChanging = true;

        /// <summary>
        /// Dont affect transform scale on trigger radius.
        /// </summary>
        [FormerlySerializedAs("ignoreTransformsScale")]
        public bool IgnoreTransformsScale = true;

        /// <summary>
        /// Don't trigger by other colliders.
        /// </summary>
        [FormerlySerializedAs("ignoreOtherSpheresOfInfluences")]
        public bool IgnoreOtherSpheresOfInfluences = true;

        /// <summary>
        /// Draw sphere in editor.
        /// </summary>
        [FormerlySerializedAs("drawGizmo")]
        public bool DrawGizmo;

        private void Awake()
        {
            GetTriggerCollider();
            Target = GetComponentInParent<CelestialBody>();
            if (!Detector || !Target)
            {
                enabled = false;
            }
            TriggerRadius = Mathf.Abs(TriggerRadius);
        }

        private void GetTriggerCollider()
        {
            var colliders = GetComponentsInChildren<SphereCollider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].isTrigger)
                {
                    Detector = colliders[i];
                }
            }
            if (!Detector)
            {
                Detector = gameObject.AddComponent<SphereCollider>();
                Debug.Log("SpaceGravity2D: Sphere Of Influence autocreate trigger for " + name);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (UseAutoROI)
            {
                if (Target && Target.AttractorRef && !double.IsNaN(Target.OrbitData.SemiMajorAxis))
                {
                    Target.AttractorRef.FindReferences();
                    Target.FindReferences();
                    TriggerRadius = (float)Target.OrbitData.SemiMajorAxis * Mathf.Pow((float)(Target.Mass / Target.AttractorRef.Mass), 2f / 5f);
                }
            }

            float parentScale = 1f;
            float scale = 1f;
            if (IgnoreTransformsScale)
            {
                parentScale = transform.parent == null ? 1 : (transform.parent.localScale.x + transform.parent.localScale.y) / 2f;
                scale = (transform.localScale.x + transform.localScale.y) / 2f;
            }
            Detector.radius = TriggerRadius / scale / parentScale;
        }
#endif

        private void OnTriggerEnter(Collider col)
        {
            if (col.transform != transform.parent)
            {
                if (IgnoreOtherSpheresOfInfluences && col.GetComponentInChildren<SphereOfInfluence>() != null)
                {
                    return;
                }
                var cBody = col.GetComponentInParent<CelestialBody>();
                if (cBody && cBody.AttractorRef != Target && cBody.Mass < Target.Mass && (!IgnoreBodiesWithDynamicAttrChanging || !cBody.IsAttractorSearchActive))
                {
                    if (cBody.AttractorRef != null)
                    {
                        // Check if body is already attracted by child of current _body.
                        if (cBody.AttractorRef.AttractorRef == Target)
                        {
                            return;
                        }
                    }
                    cBody.SetAttractor(Target);
                }
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.transform != transform.parent)
            {
                var colBody = col.GetComponentInParent<CelestialBody>();
                if (colBody && colBody.AttractorRef == Target && (!IgnoreBodiesWithDynamicAttrChanging || !colBody.IsAttractorSearchActive))
                {
                    colBody.SetAttractor(Target.AttractorRef);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (enabled && DrawGizmo)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                Gizmos.DrawSphere(transform.position, TriggerRadius);
            }
        }
    }
}
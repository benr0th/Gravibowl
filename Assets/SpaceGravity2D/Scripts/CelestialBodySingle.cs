using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceGravity2D
{
    /// <summary>
    /// Component for standalone static orbiting body, 
    /// which is independent from SimulationControl.
    /// </summary>
    /// <remarks>
    /// This component is designed for situations, 
    /// when only static orbit motion is required, 
    /// and no any interactions with other bodies.
    /// Attractor parameters placed inside this component, 
    /// so any gameobject can play role of attractor.
    /// </remarks>
    [AddComponentMenu("SpaceGravity2D/CelestialBodySingle")]
    [SelectionBase]
    public class CelestialBodySingle : MonoBehaviour
    {
        /// <summary>
        /// Reference to attractor transform.
        /// </summary>
        [Obsolete("Use AttractorObjectRef instead")]
        public Transform attractorObject
        {
            get
            {
                return AttractorObjectRef;
            }
        }

        /// <summary>
        /// Reference to attractor transform.
        /// </summary>
        [FormerlySerializedAs("attractorObject")]
        [Tooltip("Reference to attractor transform.")]
        public Transform AttractorObjectRef;
     
        /// <summary>
        /// Attractor's mass. Should be bigger than 1.
        /// </summary>
        [Obsolete("Use AttractorMass instead.")]
        public float attractorMass
        {
            get
            {
                return AttractorMass;
            }
        }

        /// <summary>
        /// Attractor's mass. Should be bigger than 1.
        /// </summary>
        [FormerlySerializedAs("attractorMass")]
        [Tooltip("Attractor's mass. Should be bigger than 1.")]
        public float AttractorMass = 1000;

        /// <summary>
        /// Max distance for display orbit.
        /// </summary>
        [Obsolete("Use MaxOrbitWorldUnitsDistance instead.")]
        public float maxDistForHyperbolicCase
        {
            get
            {
                return MaxOrbitWorldUnitsDistance;
            }
        }

        /// <summary>
        /// Max distance for display orbit in world units.
        /// </summary>
        [FormerlySerializedAs("maxDistForHyperbolicCase")]
        [Tooltip("Max distance for display orbit in world units.")]
        public float MaxOrbitWorldUnitsDistance = 100f;

        /// <summary>
        /// Gravitational constant.
        /// </summary>
        [Obsolete("Use GravitationalConstant instead.")]
        public float G
        {
            get
            {
                return GravitationalConstant;
            }
        }

        /// <summary>
        /// Gravitational constant.
        /// </summary>
        /// <remarks>
        /// F = G * (m1 * m2 / distance)
        /// </remarks>
        [FormerlySerializedAs("G")]
        [Tooltip("Classic Gravitational Constant.")]
        public float GravitationalConstant = 0.001f;

        /// <summary>
        /// Reference to velocity handle object.
        /// Assign object and use it as velocity control handle in scene view.
        /// </summary>
        [Obsolete("Use VelocityHandleRef instead.")]
        public Transform velocityHandle
        {
            get
            {
                return VelocityHandleRef;
            }
        }

        /// <summary>
        /// Reference to velocity handle object.
        /// Assign object and use it as velocity control handle in scene view.
        /// </summary>
        [Tooltip("The velocity handle object. Assign object and use it as velocity control handle in scene view.")]
        [FormerlySerializedAs("velocityHandle")]
        public Transform VelocityHandleRef;

        /// <summary>
        /// Multiplier for velocity;
        /// </summary>
        [Obsolete("Use VelocityMlt instead.")]
        public float velocityMlt
        {
            get
            {
                return VelocityMlt;
            }
        }

        /// <summary>
        /// Multiplier for velocity;
        /// </summary>
        [FormerlySerializedAs("velocityMlt")]
        public float VelocityMlt = 1f;

        /// <summary>
        /// Motion total speed setting.
        /// </summary>
        [Tooltip("Motion total speed setting.")]
        public float TimeScale = 1f;

        /// <summary>
        /// The orbit data.
        /// Internal state of orbit.
        /// </summary>
        [Obsolete("Use OrbitData instead.")]
        public OrbitData orbitData
        {
            get
            {
                return OrbitData;
            }
        }

        /// <summary>
        /// The orbit data.
        /// Internal state of orbit.
        /// </summary>
        [FormerlySerializedAs("orbitData")]
        [Tooltip("Internal state of orbit.")]
        public OrbitData OrbitData = new OrbitData();

        [Obsolete("Use OrbitPointsCount instead.")]
        public int orbitPointsCount
        {
            get
            {
                return OrbitPointsCount;
            }
        }

        /// <summary>
        /// Max display orbit points count.
        /// More points - better precision.
        /// </summary>
        [FormerlySerializedAs("orbitPointsCount")]
        [Tooltip("More points - better precision.")]
        public int OrbitPointsCount = 50;

        /// <summary>
        /// Reference to orbit linerenderer.
        /// </summary>
        [Obsolete("Use LineRendererRef instead.")]
        public LineRenderer linerend
        {
            get
            {
                return LineRendererRef;
            }
        }

        /// <summary>
        /// Reference to orbit linerenderer.
        /// Required only if orbit display is used.
        /// </summary>
        [FormerlySerializedAs("linerend")]
        [Tooltip("Required only if orbit display is used.")]
        public LineRenderer LineRendererRef;

        /// <summary>
        /// Disable continious editing orbit in update loop, if you don't need it.
        /// </summary>
        [Tooltip("Disable continious editing orbit in update loop, if you don't need it.")]
        public bool LockOrbitEditing = false;

#if UNITY_EDITOR      
        /// <summary>
        /// Toggle for editor view.
        /// </summary>
        [Header("Gizmo display options:")]
        [Tooltip("Toggle for editor view.")]
        public bool ShowOrbitGizmoInEditor = true;

        /// <summary>
        /// Toggle for editor view.
        /// </summary>
        [Tooltip("Toggle for editor view.")]
        public bool ShowOrbitGizmoWhileInPlayMode = true;

        /// <summary>
        /// The debug error displayed flag.
        /// Used to avoid errors spamming.
        /// </summary>
        private bool _debugErrorDisplayed = false;
#endif

        /// <summary>
        /// Cached state of current orbit line.
        /// </summary>
        private Vector3[] _orbitPoints;

        private void OnEnable()
        {
            ForceUpdateOrbitData();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            StartCoroutine(OrbitUpdateLoop());
        }

        /// <summary>
        /// Late update for updating line display after internal state was updated in this frame.
        /// </summary>
        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (LineRendererRef != null)
            {
                OrbitData.GetOrbitPointsNoAlloc(ref _orbitPoints, OrbitPointsCount, AttractorObjectRef.position, MaxOrbitWorldUnitsDistance);
                LineRendererRef.positionCount = _orbitPoints.Length;
                for (int i = 0; i < _orbitPoints.Length; i++)
                {
                    LineRendererRef.SetPosition(i, _orbitPoints[i]);
                }
                LineRendererRef.loop = OrbitData.Eccentricity < 1.0;
            }
        }

        /// <summary>
        /// Updates orbit internal data if necessary.
        /// </summary>
        /// <remarks>
        /// In this method orbit data is updating from view state:
        /// If you change body position, attractor mass or any other vital orbit parameter, 
        /// this change will be noticed and applyed to internal OrbitData state in this method.
        /// If you need to change orbitData state directly, by script, you need to change OrbitData state and then call ForceUpdateOrbitData
        /// </remarks>
        private void Update()
        {
            if (AttractorObjectRef!=null)
            {
#if UNITY_EDITOR
                _debugErrorDisplayed = false;
#endif
                if (!LockOrbitEditing)
                {
                    var position = transform.position - AttractorObjectRef.position;

                    var velocity = VelocityHandleRef == null ? new Vector3d() : new Vector3d(VelocityHandleRef.position - transform.position);
                    if (position != (Vector3)OrbitData.Position ||
                        (VelocityHandleRef != null && (Vector3)velocity != (Vector3)OrbitData.Velocity) ||
                        OrbitData.GravitationalConstant != GravitationalConstant ||
                        OrbitData.AttractorMass != AttractorMass)
                    {
                        ForceUpdateOrbitData();
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
                if (!_debugErrorDisplayed)
                {
                    _debugErrorDisplayed = true;
                    if (Application.isPlaying)
                    {
                        Debug.LogError("SpaceGravity2D: Attractor reference not asigned", context: gameObject);
                    }
                    else
                    {
                        Debug.Log("SpaceGravity2D: Attractor reference not asigned", context: gameObject);
                    }
                }
#endif
            }
        }


        /// <summary>
        /// Progress orbit path motion.
        /// Actual kepler orbiting is processed here.
        /// </summary>
        /// <remarks>
        /// Orbit motion progress calculations must be placed after Update, so orbit parameters changes can be applyed,
        /// but before LateUpdate, so orbit can be displayed in same frame.
        /// Coroutine loop is best candidate for achieving this.
        /// </remarks>
        private IEnumerator OrbitUpdateLoop()
        {
            while (true)
            {
                if (AttractorObjectRef!=null)
                {
                    if (!OrbitData.IsValidOrbit)
                    {
                        //try to fix orbit if we can.
                        OrbitData.CalculateNewOrbitData();
                    }

                    if (OrbitData.IsValidOrbit)
                    {
                        OrbitData.UpdateOrbitDataByTime(Time.deltaTime * TimeScale);

                        transform.position = AttractorObjectRef.position + (Vector3)OrbitData.Position;
                        if (VelocityHandleRef != null)
                        {
                            VelocityHandleRef.position = transform.position + (Vector3)OrbitData.Velocity;
                        }
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// Updates OrbitData from new body position and velocity vectors.
        /// </summary>
        /// <param name="relativePosition">The relative position.</param>
        /// <param name="velocity">The relative velocity.</param>
        /// <remarks>
        /// This method can be useful to assign new position of body by script.
        /// Or you can directly change OrbitData state and then manually update view.
        /// </remarks>
        public void CreateNewOrbitFromPositionAndVelocity(Vector3 relativePosition, Vector3 velocity)
        {
            if (AttractorObjectRef != null)
            {
                OrbitData.Position = new Vector3d(relativePosition);
                OrbitData.Velocity = new Vector3d(velocity);
                OrbitData.CalculateNewOrbitData();
                ForceUpdateViewFromInternalState();
            }
        }

        /// <summary>
        /// Forces the update of body position, and velocity handler from OrbitData.
        /// Call this method after any direct changing of OrbitData.
        /// </summary>
        public void ForceUpdateViewFromInternalState()
        {
            transform.position = AttractorObjectRef.position + (Vector3)OrbitData.Position;
            if (VelocityHandleRef != null)
            {
                VelocityHandleRef.position = transform.position + (Vector3)OrbitData.Velocity;
            }
        }

        /// <summary>
        /// Forces the update of internal orbit data from current world positions of body, attractor settings and velocityHandle.
        /// </summary>
        /// <remarks>
        /// This method must be called after any manual changing of body position, velocity handler position or attractor settings.
        /// It will update internal OrbitData state from view state.
        /// </remarks>
        public void ForceUpdateOrbitData()
        {
            if (AttractorObjectRef != null)
            {
                OrbitData.AttractorMass = AttractorMass;
                OrbitData.GravitationalConstant = GravitationalConstant;
                OrbitData.Position = new Vector3d(transform.position - AttractorObjectRef.position);
                if (VelocityHandleRef != null)
                {
                    OrbitData.Velocity = new Vector3d((VelocityHandleRef.position - transform.position));
                }
                OrbitData.CalculateNewOrbitData();
            }
        }

        /// <summary>
        /// Change orbit velocity vector to match circular orbit.
        /// </summary>
        [ContextMenu("Circulize orbit")]
        public void SetAutoCircleOrbit()
        {
            if (AttractorObjectRef != null)
            {
                OrbitData.Velocity = CelestialBodyUtils.CalcCircleOrbitVelocity(Vector3d.zero, OrbitData.Position, OrbitData.AttractorMass, 1f, OrbitData.OrbitNormal, OrbitData.GravitationalConstant);
                OrbitData.CalculateNewOrbitData();
                if (VelocityHandleRef != null)
                {
                    VelocityHandleRef.position = transform.position + (Vector3)OrbitData.Velocity;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (ShowOrbitGizmoInEditor)
            {
                if (!Application.isPlaying || ShowOrbitGizmoWhileInPlayMode)
                {
                    //if (!Application.isPlaying)
                    //{
                    //    _moverReference.ForceUpdateOrbitData();
                    //}
                    if (AttractorObjectRef != null)
                    {
                        ShowVelocity();
                        ShowOrbit();
                        ShowNodes();
                    }
                }
            }
        }

        private void ShowVelocity()
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)OrbitData.GetVelocityAtEccentricAnomaly(OrbitData.EccentricAnomaly));
        }

        private void ShowOrbit()
        {
            OrbitData.GetOrbitPointsNoAlloc(ref _orbitPoints, OrbitPointsCount, AttractorObjectRef.position, MaxOrbitWorldUnitsDistance);
            Gizmos.color = new Color(1, 1, 1, 0.3f);
            for (int i = 0; i < _orbitPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(_orbitPoints[i], _orbitPoints[i + 1]);
            }
        }

        private void ShowNodes()
        {
            Vector3 asc;
            if (OrbitData.GetAscendingNode(out asc))
            {
                Gizmos.color = new Color(0.9f, 0.4f, 0.2f, 0.5f);
                Gizmos.DrawLine(AttractorObjectRef.position, AttractorObjectRef.position + asc);
            }
            Vector3 desc;
            if (OrbitData.GetDescendingNode(out desc))
            {
                Gizmos.color = new Color(0.2f, 0.4f, 0.78f, 0.5f);
                Gizmos.DrawLine(AttractorObjectRef.position, AttractorObjectRef.position + desc);
            }
        }

        [ContextMenu("AutoFind LineRenderer")]
        private void AutoFindLineRenderer()
        {
            if (LineRendererRef == null)
            {
                LineRendererRef = GetComponent<LineRenderer>();
            }
        }
#endif
    }
}
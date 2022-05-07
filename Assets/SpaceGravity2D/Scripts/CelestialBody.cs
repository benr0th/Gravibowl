using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Orbital motion system.
/// </summary>
namespace SpaceGravity2D
{
    /// <summary>
    /// Component, which allows gameobject to attract other celestial bodies and be attracted by them.
    /// </summary>
	[AddComponentMenu("SpaceGravity2D/CelestialBody")]
    [ExecuteInEditMode]
    [SelectionBase]
    public class CelestialBody : MonoBehaviour
    {
        /// <summary>
        /// Static event, which was used to register creation of celestial body in SimulationControl.
        /// </summary>
        [Obsolete()]
        public static event Action<CelestialBody> OnBodyCreatedEvent;

        /// <summary>
        /// Static event, which was used to register creation of celestial body in SimulationControl.
        /// </summary>
        [Obsolete()]
        public static event Action<CelestialBody> OnBodyDestroyedEvent;
        
        /// <summary>
        /// Occuring when body was destroyed.
        /// </summary>
        public event Action OnDestroyedEvent;

        /// <summary>
        /// Occuring when body was created or enabled.
        /// </summary>
        public event Action OnEnabledEvent;

        /// <summary>
        /// Occuring when body was destroyed or disabled.
        /// </summary>
        public event Action OnDisabledEvent;

        #region fields and properties

        /// <summary>
        /// Cached transform reference.
        /// </summary>
        private Transform _transformRef;

        /// <summary>
        /// Reference to main controller. Should never be Null.
        /// </summary>
        [Obsolete("Use SimControlRef instead", error: true)]
        public SimulationControl simControlRef
        {
            get
            {
                return SimControlRef;
            }
            set
            {
                SimControlRef = value;
            }
        }

        /// <summary>
        /// Reference to main controller. Should never be Null.
        /// </summary>
        /// <remarks>
        /// If celestial body is created on scene, where Simulation control is not exist, 
        /// then new default simulation control will be created.
        /// Otherwise existing simulation control will be found and placed here as reference automatically.
        /// </remarks>
        [FormerlySerializedAs("simControlRef")]
        [Tooltip("Reference to scene sim control. Never should be null.")]
        public SimulationControl SimControlRef;

        /// <summary>
        /// World position with double precision.
        /// </summary>
        [SerializeField]
        internal Vector3d _position;

        /// <summary>
        /// World position vector with double precision.
        /// </summary>
        [Obsolete("User Position insetad")]
        public Vector3d position
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
            }
        }

        /// <summary>
        /// World position vector with double precision.
        /// </summary>
        public Vector3d Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                _transformRef.position = (Vector3)value;
            }
        }

        /// <summary>
        /// Position relative to attractor.
        /// </summary>
        [Obsolete("Use FocalPosition instead.")]
        public Vector3d focalPosition
        {
            get
            {
                return FocalPosition;
            }
            set
            {
                FocalPosition = value;
            }
        }

        /// <summary>
        /// Position relative to attractor.
        /// </summary>
        public Vector3d FocalPosition
        {
            get
            {
                if (AttractorRef != null)
                {
                    return OrbitData.Position;
                }
                return new Vector3d();
            }
            set
            {
                if (AttractorRef != null)
                {
                    Position = AttractorRef.Position + value;
                }
            }
        }

        /// <summary>
        /// Position relative to orbit center.
        /// </summary>
        [Obsolete("Use CentralPosition instead.")]
        public Vector3d centralPosition
        {
            get
            {
                return CentralPosition;
            }
        }

        /// <summary>
        /// Position relative to orbit center.
        /// </summary>
        /// <remarks>
        /// Note: orbit center is not equal to attractor position (if Eccentricity > 0)
        /// </remarks>
        public Vector3d CentralPosition
        {
            get
            {
                if (AttractorRef != null)
                {
                    return OrbitData.Position - OrbitData.CenterPoint;
                }
                return new Vector3d();
            }
        }

        /// <summary>
        /// Body mass value.
        /// Should not be less than 1.
        /// </summary>
        [Obsolete("Use Mass instead.")]
        public double mass
        {
            get
            {
                return Mass;
            }
            set
            {
                Mass = value;
            }
        }

        /// <summary>
        /// Body mass value.
        /// Should not be less than 1.
        /// </summary>
        /// <remarks>
        /// If mass is bigger than attractor mass treshold, this body will become attractor.
        /// Mass value should always be larger than 1, because of division by zero 
        /// (floating point values between 0 and 1 are ok, but it is easier to set and maintain min value 1).
        /// </remarks>
        [FormerlySerializedAs("mass")]
        [Tooltip("Should not be less than 1.")]
        public double Mass = 1f;

        /// <summary>
        /// Gravitational parameter of body [Mass * GravConst].
        /// </summary>
        public double MG
        {
            get
            {
                return Mass * SimControlRef.GravitationalConstant;
            }
        }

        /// <summary>
        /// Maximum range of attraction force in world units.
        /// </summary>
        [Obsolete("Use MaxAttractorRange instead", true)]
        public double maxAttractorRange
        {
            get
            {
                return MaxAttractionRange;
            }
            set
            {
                MaxAttractionRange = value;
            }
        }

        /// <summary>
        /// Maximum range of attraction force in world units.
        /// </summary>
        [FormerlySerializedAs("maxAttractionRange")]
        [Tooltip("In world units.")]
        public double MaxAttractionRange = double.PositiveInfinity;

        /// <summary>
        /// Attractor body reference.
        /// </summary>
        [Obsolete("Use AttractorRef instead.")]
        public CelestialBody attractor
        {
            get
            {
                return AttractorRef;
            }
            set
            {
                AttractorRef = value;
            }
        }

        /// <summary>
        /// Attractor body reference.
        /// </summary>
        /// <remarks>
        /// Not required only if motion type is N-body.
        /// Used for calculating orbit state in OrbitData.
        /// If OrbitData can't be calculated, Kepler motion type and orbit display is not possible.
        /// </remarks>
        [FormerlySerializedAs("attractor")]
        [Tooltip("Not required only if motion type is N-body.")]
        public CelestialBody AttractorRef;

        /// <summary>
        /// World space velocity vector of the body.
        /// </summary>
        [Obsolete("Use Velocity instead.")]
        public Vector3d velocity
        {
            get
            {
                return Velocity;
            }
            set
            {
                Velocity = value;
            }
        }

        /// <summary>
        /// World space velocity vector of the body.
        /// </summary>
        [FormerlySerializedAs("velocity")]
        public Vector3d Velocity;

        /// <summary>
        /// World space velocity, relative to current attractor.
        /// </summary>
        [Obsolete("Use RelativeVelocity instead.")]
        public Vector3d relativeVelocity
        {
            get
            {
                return RelativeVelocity;
            }
            set
            {
                RelativeVelocity = value;
            }
        }

        /// <summary>
        /// World space velocity, relative to current attractor.
        /// </summary>
        public Vector3d RelativeVelocity
        {
            get
            {
                if (AttractorRef)
                {
                    return Velocity - AttractorRef.Velocity;
                }
                else
                {
                    return Velocity;
                }
            }
            set
            {
                if (AttractorRef)
                {
                    Velocity = AttractorRef.Velocity + value;
                }
                else
                {
                    Velocity = value;
                }
            }
        }

        /// <summary>
        /// Position, relative to current attractor, with double precision.
        /// </summary>
        [Obsolete("Use RelativePosition instead")]
        public Vector3d relativePosition
        {
            get
            {
                return RelativePosition;
            }
            set
            {
                RelativePosition = value;
            }
        }

        /// <summary>
        /// Position, relative to current attractor, with double precision.
        /// </summary>
        public Vector3d RelativePosition
        {
            get
            {
                if (AttractorRef)
                {
                    return _position - AttractorRef._position;
                }
                else
                {
                    return _position;
                }
            }
            set
            {
                if (AttractorRef)
                {
                    _position = value + AttractorRef._position;
                    _transformRef.position = (Vector3)_position;
                }
                else
                {
                    _position = value;
                    _transformRef.position = (Vector3)_position;
                }
            }
        }

        /// <summary>
        /// Is currently position fixed in place (relative to current attractor).
        /// </summary>
        [Obsolete("Use IsFixedPosition instead.")]
        public bool isFixedPosition
        {
            get
            {
                return IsFixedPosition;
            }
            set
            {
                IsFixedPosition = value;
            }
        }

        /// <summary>
        /// Is currently position fixed in place (relative to current attractor).
        /// </summary>
        [FormerlySerializedAs("isFixedPosition")]
        public bool IsFixedPosition;

#if UNITY_EDITOR
        /// <summary>
        /// Draw orbit in editor view
        /// </summary>
        [FormerlySerializedAs("isDrawOrbit")]
        public bool IsDrawOrbit = true;
#endif

        /// <summary>
        /// Is rail motion type active at this frame. If false, then N-body motion type will be active.
        /// </summary>
        [Obsolete("Use IsKeplerMotion instead.")]
        public bool isKeplerMotion
        {
            get
            {
                return IsKeplerMotion;
            }
            set
            {
                IsKeplerMotion = value;
            }
        }

        /// <summary>
        /// Is rail motion type active at this frame. If false, then N-body motion type will be active.
        /// Don't change this manually. modify UseKeplerMotion instead.
        /// </summary>
        [FormerlySerializedAs("isKeplerMotion")]
        [Tooltip("Don't change this manually, this is read-only flag.")]
        public bool IsKeplerMotion = true;

        /// <summary>
        /// Motion type switch.
        /// Switch kepler and N-body motion type.
        /// </summary>
        [Obsolete("Use UseKeplerMotion instead.")]
        public bool useKeplerMotion
        {
            get
            {
                return UseKeplerMotion;
            }
            set
            {
                UseKeplerMotion = value;
            }
        }

        /// <summary>
        /// Motion type switch.
        /// Switch kepler and N-body motion type.
        /// </summary>
        [FormerlySerializedAs("useKeplerMotion")]
        [Tooltip("Switch kepler and N-body motion type.")]
        public bool UseKeplerMotion;

        /// <summary>
        /// Reference to currently running attractor search coroutine.
        /// Used to stop coroutine when needed.
        /// </summary>
        private Coroutine _attrSearch;

        /// <summary>
        /// Runtime state of attractor search loop.
        /// </summary>
        private bool _isAttractorSearchActive = false;

        /// <summary>
        /// Dynamic search of most proper attractor toggle.
        /// </summary>
        [Obsolete("Use IsAttractorSearchActiveInstead")]
        public bool isAttractorSearchActive
        {
            get
            {
                return IsAttractorSearchActive;
            }
            set
            {
                IsAttractorSearchActive = value;
            }
        }

        /// <summary>
        /// Dynamic search of most proper attractor toggle.
        /// Gets the current state of attractor search process.
        /// Sets attractor search state (enabled or disabled).
        /// </summary>
        public bool IsAttractorSearchActive
        {
            get
            {
                return _isAttractorSearchActive;
            }
            set
            {
                if (_isAttractorSearchActive != value)
                {
                    _isAttractorSearchActive = value;
                    if (gameObject.activeInHierarchy)
                    {
                        if (_isAttractorSearchActive)
                        {
                            _attrSearch = StartCoroutine(ContiniousAttractorSearch());
                        }
                        else
                        {
                            if (_attrSearch != null)
                            {
                                StopCoroutine(_attrSearch);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Interval for continious attractor search process in seconds.
        /// </summary>
        [Obsolete("Use SearchAttractorInterval instead.")]
        public float searchAttractorInterval
        {
            get
            {
                return SearchAttractorInterval;
            }
            set
            {
                SearchAttractorInterval = value;
            }
        }

        /// <summary>
        /// Interval for continious attractor search process in seconds.
        /// </summary>
        /// <remarks>
        /// Most proper attractor search process is quite expensive for performance, 
        /// so it should not be executed every frame. 
        /// This interval determines how often it should be performed.
        /// </remarks>
        [FormerlySerializedAs("searchAttractorInterval")]
        public float SearchAttractorInterval = 1.0f;

        /// <summary>
        /// Temporary list of attractors, which were added via SetAttractor.
        /// List is processed at the end of frame.
        /// </summary>
        private Queue<CelestialBody> _newAttractorsBuffer = new Queue<CelestialBody>(4);

        /// <summary>
        /// The additional velocity, which was added in current frame.
        /// Value of velocity will be added to main velocity once per frame.
        /// </summary>
        [Obsolete("Use AdditionalVelocity instead.")]
        public Vector3d additionalVelocity
        {
            get
            {
                return AdditionalVelocity;
            }
            set
            {
                AdditionalVelocity = value;
            }
        }

        /// <summary>
        /// The additional velocity, which was added in current frame.
        /// Value of velocity will be added to main velocity once per frame.
        /// </summary>
        /// <remarks>
        /// Used to bufferize external velocity change, so orbit can be recalculated 
        /// only once, if multiple external changes occured during single frame.
        /// </remarks>
        [NonSerialized]
        public Vector3d AdditionalVelocity;

        /// <summary>
        /// Current internal orbit state data.
        /// </summary>
        [Obsolete("Use OrbitData instead.")]
        public OrbitData orbitData
        {
            get
            {
                return OrbitData;
            }
            set
            {
                OrbitData = value;
            }
        }

        /// <summary>
        /// Current internal orbit state data.
        /// </summary>
        /// <remarks>
        /// OrbitData contains all orbit parameters for Kepler motion type and
        /// also is used to display current orbit.
        /// N-body motion type doesn't require OrbitData.
        /// OrbitData will be updated every frame only if needed.
        /// So, if current motion type is N-body, and orbit path display is turned on, 
        /// then OrbitData still will be updated every frame.
        /// </remarks>
        [FormerlySerializedAs("orbitData")]
        public OrbitData OrbitData = new OrbitData();

        /// <summary>
        /// Current world position of orbit focus (attractor position).
        /// </summary>
        [Obsolete("Use OrbitFocusPoint instead.")]
        public Vector3d orbitFocusPoint
        {
            get
            {
                return orbitFocusPoint;
            }
        }

        /// <summary>
        /// Current world position of orbit focus (attractor position).
        /// </summary>
        public Vector3d OrbitFocusPoint
        {
            get
            {
                if (AttractorRef != null)
                {
                    return AttractorRef.Position;
                }
                return Position;
            }
        }

        /// <summary>
        /// World position of orbit center.
        /// </summary>
        [Obsolete("Use OrbitCenterPoint instead")]
        public Vector3d orbitCenterPoint
        {
            get
            {
                return OrbitCenterPoint;
            }
        }

        /// <summary>
        /// World position of orbit center.
        /// </summary>
        public Vector3d OrbitCenterPoint
        {
            get
            {
                if (AttractorRef != null)
                {
                    return AttractorRef.Position + OrbitData.CenterPoint;
                }
                return Position;
            }
        }

        /// <summary>
        /// World position of periapsis orbit point.
        /// </summary>
        [Obsolete("Ise OrbitPeriapsisPoint instead.")]
        public Vector3d orbitPeriapsisPoint
        {
            get
            {
                return OrbitPeriapsisPoint;
            }
        }

        /// <summary>
        /// World position of lowest orbit point.
        /// </summary>
        public Vector3d OrbitPeriapsisPoint
        {
            get
            {
                if (AttractorRef != null)
                {
                    return AttractorRef.Position + OrbitData.Periapsis;
                }
                return Position;
            }
        }

        /// <summary>
        /// World position of highest orbit point.
        /// </summary>
        [Obsolete("Use OrbitApoapsisPoint instead.")]
        public Vector3d orbitApoapsisPoint
        {
            get
            {
                return OrbitApoapsisPoint;
            }
        }

        /// <summary>
        /// World position of highest orbit point.
        /// </summary>
        public Vector3d OrbitApoapsisPoint
        {
            get
            {
                if (AttractorRef != null)
                {
                    return AttractorRef.Position + OrbitData.Apoapsis;
                }
                return Position;
            }
        }

        /// <summary>
        /// Is current state of orbit errorless.
        /// </summary>
        [Obsolete("Use IsValidOrbit instead.")]
        public bool isValidOrbit
        {
            get
            {
                return isValidOrbit;
            }
        }

        /// <summary>
        /// Is current state of orbit errorless.
        /// </summary>
        public bool IsValidOrbit
        {
            get
            {
                return AttractorRef != null && OrbitData.IsValidOrbit;
            }
        }

        /// <summary>
        /// World position of center of mass of body and current attractor.
        /// </summary>
        [Obsolete("Use CenterOfMass instead.")]
        public Vector3d centerOfMass
        {
            get
            {
                return CenterOfMass;
            }
        }

        /// <summary>
        /// World position of center of mass of body and current attractor.
        /// </summary>
        public Vector3d CenterOfMass
        {
            get
            {
                if (AttractorRef != null)
                {
                    return CelestialBodyUtils.CalcCenterOfMass(_position, Mass, AttractorRef._position, AttractorRef.Mass);
                }
                else
                {
                    return _position;
                }
            }
        }

        /// <summary>
        /// Eccentricity of current orbit.
        /// </summary>
        [Obsolete("Use Eccentricity instead")]
        public double eccentricity
        {
            get
            {
                return Eccentricity;
            }
            set
            {
                Eccentricity = value;
            }
        }

        /// <summary>
        /// Eccentricity of current orbit.
        /// </summary>
        public double Eccentricity
        {
            get
            {
                return OrbitData.Eccentricity;
            }
            set
            {
                OrbitData.SetEccentricity(value);
                RefreshCurrentPositionAndVelocityFromOrbitData();
            }
        }

        /// <summary>
        /// True anomaly of current orbit.
        /// </summary>
        [Obsolete("Use TrueAnomaly instead")]
        public double trueAnomaly
        {
            get
            {
                return TrueAnomaly;
            }
            set
            {
                TrueAnomaly = value;
            }
        }

        /// <summary>
        /// True anomaly of current orbit.
        /// </summary>
        public double TrueAnomaly
        {
            get
            {
                return OrbitData.TrueAnomaly;
            }
            set
            {
                OrbitData.SetTrueAnomaly(value);
                RefreshCurrentPositionAndVelocityFromOrbitData();
            }
        }

        /// <summary>
        /// Eccentric anomaly of current orbit in radians.
        /// </summary>
        [Obsolete("Use EccentricAnomaly instead.")]
        public double eccentricAnomaly
        {
            get
            {
                return EccentricAnomaly;
            }
            set
            {
                EccentricAnomaly = value;
            }
        }

        /// <summary>
        /// Eccentric anomaly of current orbit in radians.
        /// </summary>
        public double EccentricAnomaly
        {
            get
            {
                return OrbitData.EccentricAnomaly;
            }
            set
            {
                OrbitData.SetEccentricAnomaly(value);
                RefreshCurrentPositionAndVelocityFromOrbitData();
            }
        }

        /// <summary>
        /// Mean anomaly of current orbit in radians.
        /// </summary>
        [Obsolete("Use MeanAnomaly instead.")]
        public double meanAnomaly
        {
            get
            {
                return MeanAnomaly;
            }
            set
            {
                MeanAnomaly = value;
            }
        }

        /// <summary>
        /// Mean anomaly of current orbit in radians.
        /// </summary>
        public double MeanAnomaly
        {
            get
            {
                return OrbitData.MeanAnomaly;
            }
            set
            {
                OrbitData.SetMeanAnomaly(value);
                RefreshCurrentPositionAndVelocityFromOrbitData();
            }
        }

        #endregion

        /// <summary>
        /// Called immediatly when object created.
        /// </summary>
        private void Awake()
        {
            if (OnBodyCreatedEvent != null)
            {
                OnBodyCreatedEvent(this);
            }
        }
        
        /// <summary>
        /// Called when object enabled.
        /// </summary>
        private void OnEnable()
        {
            FindReferences();
            SimulationControl.Instance.RegisterBody(this);

            if (_isAttractorSearchActive)
            {
                StartCoroutine(ContiniousAttractorSearch());
            }

            if (OnEnabledEvent != null)
            {
                OnEnabledEvent();
            }
        }
        
        /// <summary>
        /// Called when object disabled.
        /// </summary>
        private void OnDisable()
        {
            SimulationControl.Instance.UnregisterBody(this);
            
            if (OnDisabledEvent != null)
            {
                OnDisabledEvent();
            }
        }

        /// <summary>
        /// Called when object destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (OnBodyDestroyedEvent != null)
            {
                OnBodyDestroyedEvent(this);
            }
            if (OnDestroyedEvent != null)
            {
                OnDestroyedEvent();
            }
        }

        /// <summary>
        /// Recalculate Orbit state if visual state was changed.
        /// </summary>
        private void Update()
        {
            if (AttractorRef != null)
            {
                if (_transformRef.position != (Vector3)Position || OrbitData.AttractorMass != AttractorRef.Mass)
                {
                    CalculateNewOrbitData();
                }
            }
            else
            {
                Position = new Vector3d(_transformRef.position);
            }
        }

        /// <summary>
        /// Start method for initializing orbit state from
        /// global SimulationControl manager.
        /// </summary>
        private void Start()
        {

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                if (SimControlRef == null)
                {
                    if (SimulationControl.Instance == null)
                    {
                        Debug.Log("SpaceGravity2D: Simulation Control not found");
                        enabled = false;
                        return;
                    }

                    SimControlRef = SimulationControl.Instance;
                }
#if UNITY_EDITOR
            }
            else
            {
                if (SimControlRef == null)
                {
                    SimControlRef = GameObject.FindObjectOfType<SimulationControl>();
                }

                if (SimControlRef != null)
                {
                    OrbitData.GravitationalConstant = SimControlRef.GravitationalConstant;
                    OrbitData.EclipticNormal = SimControlRef.EclipticNormal;
                    OrbitData.EclipticUp = SimControlRef.EclipticUp;
                }
            }
#endif
        }

        /// <summary>
        /// Coroutine for infinity background search of most proper attractor.
        /// </summary>
        private IEnumerator ContiniousAttractorSearch()
        {
            yield return null;
            float timer = 0;
            while (isActiveAndEnabled && _isAttractorSearchActive)
            {
                timer += Time.deltaTime;
                if (timer >= SearchAttractorInterval)
                {
                    timer = 0;
                    FindAndSetMostProperAttractor();
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Autofind and precache required components references.
        /// </summary>
        public void FindReferences()
        {
            if (_transformRef == null || SimControlRef == null)
            {
                if (_transformRef == null)
                {
                    _transformRef = transform;
                }

                if (SimControlRef == null)
                {
                    SimControlRef = SimulationControl.Instance ?? GameObject.FindObjectOfType<SimulationControl>();
                }
            }
        }

        /// <summary>
        /// Circularize orbit.
        /// Orbit plane will not be changed.
        /// </summary>
        /// <remarks>
        /// Behaviour update: resulting orbit will be always same orientation, as before calling this method (not reversing velocity).
        /// </remarks>
        public void MakeOrbitCircle()
        {
            if (AttractorRef)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FindReferences();
                    AttractorRef.FindReferences();
                    UnityEditor.Undo.RecordObject(this, "Round orbit");
                }
#endif
                var v = CelestialBodyUtils.CalcCircleOrbitVelocity(
                    AttractorRef._position,
                    _position,
                    AttractorRef.Mass,
                    Mass,
                    OrbitData.OrbitNormal,
                    SimControlRef.GravitationalConstant
                );

                if (RelativeVelocity != v)
                {
                    RelativeVelocity = v;
                    OrbitData.IsDirty = true;
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("SpaceGravity2D: Can't round orbit. " + name + " has no attractor");
            }
#endif
        }

        /// <summary>
        /// Circularize orbit.
        /// Orbit plane will be unchanged.
        /// </summary>
        public void MakeOrbitCircle(bool clockwise)
        {
            if (AttractorRef)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FindReferences();
                    AttractorRef.FindReferences();
                    UnityEditor.Undo.RecordObject(this, "Round orbit");
                }
#endif
                var dotProduct = CelestialBodyUtils.DotProduct(OrbitData.OrbitNormal, SimControlRef.EclipticNormal); //sign of this value determines orbit orientation
                if (Mathd.Abs(OrbitData.OrbitNormal.sqrMagnitude - 1d) > 0.5d)
                {
                    OrbitData.OrbitNormal = SimControlRef.EclipticNormal;
                }
                var v = CelestialBodyUtils.CalcCircleOrbitVelocity(
                    AttractorRef._position,
                    _position,
                    AttractorRef.Mass,
                    Mass,
                    OrbitData.OrbitNormal * (clockwise && dotProduct >= 0 || !clockwise && dotProduct < 0 ? 1 : -1),
                    SimControlRef.GravitationalConstant
                );

                if (RelativeVelocity != v)
                {
                    RelativeVelocity = v;
                    OrbitData.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Assign new attractor reference (or null) to this instance.
        /// </summary>
        /// <param name="attr">Attractor instance or null.</param>
        public void SetAttractor(CelestialBody attr)
        {
            if (attr == null || (attr != AttractorRef && attr != this))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RecordObject(this, "attractor ref change");
                }
#endif
                AttractorRef = attr;
                OrbitData.IsDirty = true;
            }
        }

        /// <summary>
        /// Set new attractor at the end of frame or instantly.
        /// </summary>
        [Obsolete(message: "Use same method with single parameter.", error: false)]
        public void SetAttractor(CelestialBody attr, bool checkIsInRange, bool instant = false)
        {
            if ((attr == null || (attr != AttractorRef && attr.Mass > Mass)) && attr != this)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RecordObject(this, "attractor ref change");
                }

                if (!Application.isPlaying || instant)
                {
#else
				if (instant) {
#endif
                    AttractorRef = attr;
                    OrbitData.IsDirty = true;
                    return;
                }

                if (_newAttractorsBuffer.Count == 0)
                {
                    StartCoroutine(SetNearestAttractor(checkIsInRange));
                }

                _newAttractorsBuffer.Enqueue(attr);
            }
        }

        private IEnumerator SetNearestAttractor(bool checkIsInRange)
        {
            yield return new WaitForEndOfFrame();
            if (_newAttractorsBuffer.Count == 0)
            {
                yield break;
            }
            if (_newAttractorsBuffer.Count == 1)
            {
                if (checkIsInRange)
                {
                    var attr = _newAttractorsBuffer.Dequeue();
                    if (attr == null || (attr._position - _position).magnitude < Mathd.Min(attr.MaxAttractionRange, SimControlRef.MaxAttractionRange))
                    {
                        AttractorRef = attr;
                        OrbitData.IsDirty = true;
                    }
                }
                else
                {
                    AttractorRef = _newAttractorsBuffer.Dequeue();
                    OrbitData.IsDirty = true;
                }

                _newAttractorsBuffer.Clear();
                yield break;
            }
            CelestialBody nearest = _newAttractorsBuffer.Dequeue();
            var sqrDistance = nearest != null ? (nearest._position - _position).sqrMagnitude : double.MaxValue;
            while (_newAttractorsBuffer.Count > 0)
            {
                var cb = _newAttractorsBuffer.Dequeue();
                if (cb == nearest)
                {
                    continue;
                }

                if (cb != null && (cb._position - _position).sqrMagnitude < sqrDistance)
                {
                    nearest = cb;
                    sqrDistance = (cb._position - _position).sqrMagnitude;
                }
            }

            AttractorRef = nearest;
            OrbitData.IsDirty = true;
        }

        /// <summary>
        /// Stop Kepler motion type and return to N-body motion type at next frame.
        /// </summary>
        public void TerminateKeplerMotion()
        {
            IsKeplerMotion = false;
        }

        /// <summary>
        /// Find and assign attractor with shortest distance to this body.
        /// </summary>
        [ContextMenu("Find nearest attractor")]
        public void FindAndSetNearestAttractor()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                FindReferences();
            }
#endif
            if (SimControlRef)
            {
                SimControlRef.SetNearestAttractorForBody(this);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("SpaceGravity2D: Simulation Control not found");
            }
#endif
        }

        /// <summary>
        /// Find and assign attractor with largest relative gravitational influence to this body.
        /// </summary>
        [ContextMenu("Find Most Proper Attractor")]
        public void FindAndSetMostProperAttractor()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                FindReferences();
            }
#endif
            if (SimControlRef)
            {
                SimControlRef.SetMostProperAttractorForBody(this);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("SpaceGravity2D: Simulation Control not found");
            }
#endif
        }

        /// <summary>
        /// Find ans assign attractor with largest mass on scene.
        /// </summary>
        public void FindAndSetBiggestAttractor()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                FindReferences();
            }
#endif
            if (SimControlRef)
            {
                SimControlRef.SetBiggestAttractorForBody(this);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("SpaceGravity2D: Simulation Control not found");
            }
#endif
        }

        /// <summary>
        /// Make projection of position and velocity onto ecliptic plane (make 2d).
        /// </summary>
        /// <remarks>
        /// If call this method every frame, simulation will 
        /// be restricted in 2d plane, which may be usefull.
        /// There is such setting in SpaceGravity2DWindow.
        /// </remarks>
        [ContextMenu("Project (pos and v) onto ecliptic plane")]
        public void ProjectOntoEclipticPlane()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.RecordObject(this, "ecliptic projection");
                UnityEditor.Undo.RecordObject(_transformRef, "ecliptic projection");
            }
#endif
            var projectedPos = _position - SimControlRef.EclipticNormal * CelestialBodyUtils.DotProduct(_position, SimControlRef.EclipticNormal);
            var projectedV = Velocity - SimControlRef.EclipticNormal * CelestialBodyUtils.DotProduct(Velocity, SimControlRef.EclipticNormal);
            _position = projectedPos;
            _transformRef.position = (Vector3)projectedPos;
            Velocity = projectedV;
            OrbitData.IsDirty = true;
        }

        /// <summary>
        /// Recalculate orbit data from current position, velocity and attractor data.
        /// </summary>
        /// <remarks>
        /// Should be called after manual change of transform of body or attractor, or other visual parameters to update internal orbit state.
        /// </remarks>
        [ContextMenu("Recalculate orbit data")]
        public void CalculateNewOrbitData()
        {
            if (AttractorRef)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    FindReferences();
                    AttractorRef.FindReferences();
                }
#endif
                _position = new Vector3d(_transformRef.position);
                OrbitData.AttractorMass = AttractorRef.Mass;
                OrbitData.EclipticNormal = SimControlRef.EclipticNormal;
                OrbitData.EclipticUp = SimControlRef.EclipticUp;
                OrbitData.Position = _position - AttractorRef._position;
                OrbitData.Velocity = Velocity - AttractorRef.Velocity;
                OrbitData.CalculateNewOrbitData();
            }
        }

        /// <summary>
        /// If internal orbit data was changed, this method will update corresponding visual parameters.
        /// </summary>
        /// <remarks>
        /// Call this method if manually changing orbit data, while motion type is KeplerMotion.
        /// (N-body doesn't require refreshing, because orbit data is recalulating every frame anyway).
        /// </remarks>
        public void RefreshCurrentPositionAndVelocityFromOrbitData()
        {
            if (AttractorRef != null)
            {
                Position = AttractorRef._position + OrbitData.Position;
                Velocity = AttractorRef.Velocity + OrbitData.Velocity;
            }
        }

        /// <summary>
        /// Apply additional rotation to whole orbit.
        /// </summary>
        /// <param name="rotation">Rotation to add.</param>
        /// <remarks>
        /// For example, quaternion.euler(15,0,0) will rotate orbit for 15 degrees around x axis.
        /// </remarks>
        public void RotateOrbitAroundFocus(Quaternion rotation)
        {
            if (AttractorRef != null)
            {
                OrbitData.Position = _position - AttractorRef._position;
                OrbitData.Velocity = Velocity - AttractorRef.Velocity;
                OrbitData.RotateOrbit(rotation);
                Position = AttractorRef.Position + OrbitData.Position;
                Velocity = AttractorRef.Velocity + OrbitData.Velocity;
                OrbitData.IsDirty = true;
            }
        }

        /// <summary>
        /// Get orbit points for current orbit.
        /// </summary>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.")]
        public Vector3[] GetOrbitPoints(int pointsCount = 50, bool localSpace = false, float maxDistance = 1000f)
        {
            Vector3[] points = null;
            GetOrbitPointsNoAlloc(ref points, pointsCount, localSpace, maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit points for current orbit.
        /// </summary>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.")]
        public Vector3d[] GetOrbitPointsDouble(int pointsCount = 50, bool localSpace = false, float maxDistance = 1000f)
        {
            Vector3d[] points = null;
            GetOrbitPointsNoAlloc(ref points, pointsCount, localSpace, maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit points for current orbit without allocation of points array.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// And target points count not always equal to pointsCount parameter, because sometimes orbit curve doesn't require maximym points count.
        /// </remarks>
        /// <param name="points">Array of orbut curve points.</param>
        /// <param name="pointsCount">Maximum curve points count.</param>
        /// <param name="localSpace">Is curve centered to this body, or to world space.</param>
        /// <param name="maxDistance">Max distance for curve points.</param>
        public void GetOrbitPointsNoAlloc(ref Vector3[] points, int pointsCount = 50, bool localSpace = false, float maxDistance = 1000f)
        {
            if (!AttractorRef)
            {
                // Without attractor return straight path.
                if (Velocity.sqrMagnitude < 1e-004)
                {
                    // If velocity too small, don't generate path.
                    if (points == null || points.Length > 0)
                    {
                        points = new Vector3[0];
                    }
                    return;
                }

                var normal = (Vector3)Velocity.normalized;
                if (points == null || points.Length != 3)
                {
                    points = new Vector3[3];
                }

                if (localSpace)
                {
                    points[0] = -normal * maxDistance;
                    points[1] = new Vector3();
                    points[2] = normal * maxDistance;
                }
                else
                {
                    points[0] = _transformRef.position - normal * maxDistance;
                    points[1] = _transformRef.position;
                    points[2] = _transformRef.position + normal * maxDistance;
                }
            }
            else
            {
                OrbitData.GetOrbitPointsNoAlloc(ref points, pointsCount, localSpace ? new Vector3() : AttractorRef._transformRef.position, maxDistance);
            }
        }

        /// <summary>
        /// Get orbit points for current orbit without allocation of points array.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// And target points count not always equal to pointsCount parameter, because sometimes orbit curve doesn't require maximym points count.
        /// </remarks>
        /// <param name="points">Array of orbut curve points.</param>
        /// <param name="pointsCount">Maximum curve points count.</param>
        /// <param name="localSpace">Is curve centered to this body, or to world space.</param>
        /// <param name="maxDistance">Max distance for curve points.</param>
        public void GetOrbitPointsNoAlloc(ref Vector3d[] points, int pointsCount = 50, bool localSpace = false, double maxDistance = 1000d)
        {
            if (!AttractorRef)
            {
                // Without attractor return straight path.
                if (Velocity.sqrMagnitude < 1e-004)
                {
                    // If velocity too small, don't generate path.
                    if (points == null || points.Length > 0)
                    {
                        points = new Vector3d[0];
                    }
                    return;
                }

                var normal = Velocity.normalized;
                if (points == null || points.Length != 3)
                {
                    points = new Vector3d[3];
                }

                if (localSpace)
                {
                    points[0] = -normal * maxDistance;
                    points[1] = new Vector3d();
                    points[2] = normal * maxDistance;
                }
                else
                {
                    points[0] = Position - normal * maxDistance;
                    points[1] = Position;
                    points[2] = Position + normal * maxDistance;
                }
            }
            else
            {
                OrbitData.GetOrbitPointsNoAlloc(ref points, pointsCount, localSpace ? new Vector3d() : AttractorRef.Position, maxDistance);
            }
        }

        /// <summary>
        /// Add additional velocity to this body' velocity at the end of frame
        /// and switch to n-body motion type.
        /// </summary>
        /// <param name="deltaVelocity">Additional velocity.</param>
        public void AddExternalVelocity(Vector3d deltaVelocity)
        {
            AdditionalVelocity += deltaVelocity;
            TerminateKeplerMotion();
            OrbitData.IsDirty = true;
        }

        /// <summary>
        /// Add additional velocity to this body' velocity at the end of frame
        /// and switch to n-body motion type.
        /// </summary>
        /// <param name="deltaVelocity">Additional velocity.</param>
        public void AddExternalVelocity(Vector3 deltaVelocity)
        {
            AddExternalVelocity(new Vector3d(deltaVelocity));
        }

        /// <summary>
        /// Add force to this body's velocity.
        /// Velocity will be changed at the end of frame.
        /// </summary>
        /// <param name="forceVector">Force direction and magnitude vector.</param>
        public void AddExternalForce(Vector3d forceVector)
        {
            AdditionalVelocity += forceVector / Mass;
            TerminateKeplerMotion();
            OrbitData.IsDirty = true;
        }

        /// <summary>
        /// Add force to this body's velocity.
        /// Velocity will be changed at the end of frame.
        /// </summary>
        /// <param name="forceVector">Force direction and magnitude vector.</param>
        public void AddExternalForce(Vector3 forceVector)
        {
            AddExternalForce(new Vector3d(forceVector));
        }

        /// <summary>
        /// Set new position and recalculate orbit.
        /// </summary>
        /// <param name="newPosition">New world position.</param>
        public void SetPosition(Vector3d newPosition)
        {
            Position = newPosition;
            OrbitData.IsDirty = true;
        }

        /// <summary>
        /// Set new position and recalculate orbit.
        /// </summary>
        /// <param name="newPosition">New world position.</param>
        public void SetPosition(Vector3 newPosition)
        {
            SetPosition(new Vector3d(newPosition));
        }

        /// <summary>
        /// Get position relative to center point of current orbit when eccentric anomaly is equal to specified value.
        /// </summary>
        /// <param name="eccentricAnomaly">Position on current orbit, determined by eccentric anomaly orbital parameter.</param>
        /// <returns>Position, relative to orbit center.</returns>
        public Vector3d GetCentralPositionAtEccentricAnomaly(double eccentricAnomaly)
        {
            return OrbitData.GetCentralPositionAtEccentricAnomaly(eccentricAnomaly);
        }

        /// <summary>
        /// Get position relative to center point of current orbit when true anomaly is equal to specified value.
        /// </summary>
        /// <param name="trueAnomaly">Position on current orbit, determined by true anomaly orbital parameter.</param>
        /// <returns>Position, relative to orbit center.</returns>
        public Vector3d GetCentralPositionAtTrueAnomaly(double trueAnomaly)
        {
            return OrbitData.GetCentralPositionAtTrueAnomaly(trueAnomaly);
        }

        /// <summary>
        /// Get position relative to focal point of current orbit when eccentric anomaly is equal to specified value.
        /// </summary>
        /// <param name="eccentricAnomaly">Position on current orbit, determined by eccentric anomaly orbital parameter.</param>
        /// <returns>Position, relative to orbit focus.</returns>
        public Vector3d GetFocalPositionAtEccentricAnomaly(double eccentricAnomaly)
        {
            return OrbitData.GetFocalPositionAtEccentricAnomaly(eccentricAnomaly);
        }

        /// <summary>
        /// Get position relative to focal point of current orbit when true anomaly is equal to specified value.
        /// </summary>
        /// <param name="trueAnomaly">Position on current orbit, determined by true anomaly orbital parameter.</param>
        /// <returns>Position, relative to orbit focus.</returns>
        public Vector3d GetFocalPositionAtTrueAnomaly(double trueAnomaly)
        {
            return OrbitData.GetFocalPositionAtTrueAnomaly(trueAnomaly);
        }

        /// <summary>
        /// Get velocity, relative to current attractor, when eccentric anomaly is equal to specified value.
        /// </summary>
        /// <param name="eccentricAnomaly">Velocity, relative to attractor at orbit point, determined by eccentric anomaly.</param>
        /// <returns>Relative velocity.</returns>
        public Vector3d GetRelVelocityAtEccentricAnomaly(double eccentricAnomaly)
        {
            return OrbitData.GetVelocityAtEccentricAnomaly(eccentricAnomaly);
        }

        /// <summary>
        /// Get velocity, relative to current attractor, when true anomaly is equal to specified value.
        /// </summary>
        /// <param name="trueAnomaly">Velocity, relative to attractor at orbit point, determined by true anomaly.</param>
        /// <returns>Relative velocity.</returns>
        public Vector3d GetRelVelocityAtTrueAnomaly(double trueAnomaly)
        {
            return OrbitData.GetVelocityAtTrueAnomaly(trueAnomaly);
        }

        /// <summary>
        /// Progress dynamic orbit values by specified delta time.
        /// </summary>
        /// <remarks>
        /// If current motion type is Kepler Motion, then this method is called every frame from
        /// global scene controller and such orbit parameters, as mean anomaly, true anomaly and eccentric anomaly
        /// will be changed according to specified delta time. And then new position and velocity values will be calculated from anomalies.
        /// </remarks>
        /// <param name="deltatime">Progress time.</param>
        public void UpdateObjectOrbitDynamicParameters(double deltatime)
        {
            if (AttractorRef == null)
            {
                return;
            }
            OrbitData.UpdateOrbitAnomaliesByTime(deltatime);
            OrbitData.SetPositionByCurrentAnomaly();
            OrbitData.SetVelocityByCurrentAnomaly();
        }

        /// <summary>
        /// Get local position of ascending node of current orbit.
        /// </summary>
        /// <param name="asc">Resulting point.</param>
        /// <returns>Operation success status.</returns>
        public bool GetAscendingNode(out Vector3 asc)
        {
            if (AttractorRef != null)
            {
                if (OrbitData.GetAscendingNode(out asc))
                {
                    return true;
                }
            }
            asc = new Vector3();
            return false;
        }

        /// <summary>
        /// Get local position of descending node of current orbit.
        /// </summary>
        /// <param name="desc">Resulting point.</param>
        /// <returns>Operation success status.</returns>
        public bool GetDescendingNode(out Vector3 desc)
        {
            if (AttractorRef != null)
            {
                if (OrbitData.GetDescendingNode(out desc))
                {
                    return true;
                }
            }
            desc = new Vector3();
            return false;
        }

        /// <summary>
        /// Gets normal vector to plane, which is defined by body position and velocity vector.
        /// If orbit is valid, velocity normal is equal to orbit normal,
        /// otherwise velocity normal will be calculated as perpendicular to velocity and ecliptic up vector.
        /// </summary>
        /// <returns>Unit vector, perpendicular to velocity.</returns>
        public Vector3 GetVelocityPlaneNormal()
        {
            if (AttractorRef != null && OrbitData.IsValidOrbit)
            {
                return (Vector3)OrbitData.OrbitNormal;
            }
            else
            {
                if (Velocity.sqrMagnitude > 1e-3)
                {
                    var planeAlignedPerpendicularDirection = (Vector3)CelestialBodyUtils.CrossProduct((Vector3)Velocity.normalized, (Vector3)SimControlRef.EclipticNormal);
                    return CelestialBodyUtils.CrossProduct(planeAlignedPerpendicularDirection, (Vector3)Velocity.normalized);
                }
                else
                {
                    return (Vector3)SimControlRef.EclipticNormal;
                }
            }
        }

    }//celestial body class

}//namespace
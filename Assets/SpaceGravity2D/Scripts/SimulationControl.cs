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
    /// Main controller for gravitational motion on scene.
    /// Controls behaviour of celestial bodies, and holds global settings of gravitational simulation.
    /// </summary>
    [AddComponentMenu("SpaceGravity2D/SimulationControl")]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SimulationControl : MonoBehaviour
    {
        /// <summary>
        /// List of available n-body calculation algorythms.
        /// </summary>
        public enum NBodyCalculationType
        {
            /// <summary>
            /// Fastest n-body algorythm.
            /// </summary>
            Euler = 0,

            /// <summary>
            /// More stable n-body algorythm.
            /// </summary>
            Verlet,

            /// <summary>
            /// Slowest and more precise n-body algorythm.
            /// </summary>
            /// <remarks>
            /// Note - this algorythm may become very unstable if distance 
            /// between attracting bodies becomes very close to value of position delta at current frame.
            /// </remarks>
            RungeKutta,
        }

        /// <summary>
        /// Main gravitational constant.
        /// </summary>
        /// <remarks>
        /// The real value 6.67384 * 10E-11 may not be very useful for gaming purposes.
        /// Less G const value - slower motion of all objects is, and higher value - faster motion.
        /// </remarks>
        [SerializeField]
        [FormerlySerializedAs("gravitationalConstant")]
        private double _gravitationalConstant = 0.0001d;

        /// <summary>
        /// Gets Gravitational constant value.
        /// Sets Gravitational constant for all active bodies.
        /// </summary>
        /// <remarks>
        /// This property is usefull in runtime, when active celestial bodies have already fetched G constant from controller and
        /// it needs to be refreshed manually for all of them.
        /// </remarks>
        public double GravitationalConstant
        {
            get
            {
                return _gravitationalConstant;
            }
            set
            {
                _gravitationalConstant = value;
                ApplyGravConstToAllBodies();
            }
        }

        /// <summary>
        /// Gets Gravitational constant value.
        /// Sets Gravitational constant for all active bodies and changes all velocities proportional for making orbits unchanged.
        /// </summary>
        /// <remarks>
        /// This property is usefull when need to change G const, but not need to change already customized orbits.
        /// Less G const value - slower motion of all objects is, and higher value - faster motion.
        /// </remarks>
        public double GravitationalConstantProportional
        {
            get
            {
                return _gravitationalConstant;
            }
            set
            {
                if (_gravitationalConstant != value)
                {
                    var deltaRatio = Mathd.Abs(_gravitationalConstant) < 1e-23d ? 1d : value / _gravitationalConstant;
                    _gravitationalConstant = value;
                    ApplyGravConstToAllBodies();
                    ChangeAllVelocitiesByFactor(Mathd.Sqrt(Mathd.Abs(deltaRatio)));
                }
            }
        }

        /// <summary>
        /// Global constraint for gravitational attraction range.
        /// </summary>
        [Obsolete("Use MaxAttractionRange instead.")]
        public double maxAttractionRange
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
        /// Global constraint for gravitational attraction range.
        /// </summary>
        [FormerlySerializedAs("maxAttractionRange")]
        public double MaxAttractionRange = double.PositiveInfinity;

        /// <summary>
        /// Global constraint for gravitational attraction range.
        /// </summary>
        [Obsolete("Use MinAttractionRange instead.")]
        public double minAttractionRange
        {
            get
            {
                return MinAttractionRange;
            }
            set
            {
                MinAttractionRange = value;
            }
        }

        /// <summary>
        /// Global constraint for gravitational attraction range.
        /// </summary>
        /// <remarks>
        /// It is better to set this value equal minimal body size. 
        /// Not recommended to set 0 value, because infinity velocities will occur, when two bodies will approach each other too close.
        /// </remarks>
        [FormerlySerializedAs("minAttractionRange")]
        public double MinAttractionRange = 0.1d;

        /// <summary>
        /// TimeScale of simulation process. May be dynamicaly changed, but very large values decreasing precision of calculations
        /// </summary>
        [Obsolete("Use TimeScale instead.")]
        public double timeScale
        {
            get
            {
                return TimeScale;
            }
            set
            {
                TimeScale = value;
            }
        }

        /// <summary>
        /// TimeScale of simulation process. May be dynamicaly changed, but very large values decreasing precision of calculations
        /// </summary>
        [FormerlySerializedAs("timeScale")]
        public double TimeScale = 1d;

        /// <summary>
        /// Mass threshold for body to became attractor
        /// </summary>
        [Obsolete("Use MinAttractorMass instead.")]
        public double minAttractorMass
        {
            get
            {
                return MinAttractorMass;
            }
            set
            {
                MinAttractorMass = value;
            }
        }

        /// <summary>
        /// Mass threshold for body to became attractor
        /// </summary>
        [FormerlySerializedAs("minAttractorMass")]
        public double MinAttractorMass = 100d;

#if UNITY_EDITOR
        /// <summary>
        /// The configuration of editor scene view tools.
        /// </summary>
        public SceneViewSettings SceneElementsDisplayParameters = new SceneViewSettings();
#endif

        [Obsolete("Use Bodies instead.")]
        public List<CelestialBody> bodies
        {
            get
            {
                return Bodies;
            }
        }

        /// <summary>
        /// References to all active celestial bodies on scene.
        /// </summary>
        /// <remarks>
        /// CelestialBody instance would register intself in this controller when it will be activated.
        /// This cache should not be serialized because it's contains runtime references only.
        /// </remarks>
        [NonSerialized]
        public List<CelestialBody> Bodies = new List<CelestialBody>();

        /// <summary>
        /// Optimisation cache. Refreshed each frame.
        /// </summary>
        private List<CelestialBody> _attractorsCache = new List<CelestialBody>();

        /// <summary>
        /// Singletorn reference.
        /// </summary>
        [Obsolete("Use Instance instead")]
        public static SimulationControl instance
        {
            get
            {
                return Instance;
            }
        }

        /// <summary>
        /// Singletorn reference.
        /// </summary>
        public static SimulationControl Instance;

        /// <summary>
        /// Current n-body simulation type.
        /// </summary>
        [Obsolete("Use CalculationType instead.")]
        public NBodyCalculationType calculationType
        {
            get
            {
                return CalculationType;
            }
            set
            {
                CalculationType = value;
            }
        }

        /// <summary>
        /// Current n-body simulation type.
        /// </summary>
        [FormerlySerializedAs("calculationType")]
        public NBodyCalculationType CalculationType = NBodyCalculationType.Verlet;


        /// <summary>
        /// Is simulation affected by Time.timescale value.
        /// </summary>
        [Obsolete("Use AffectedByGlobalTimescale instead")]
        public bool affectedByGlobalTimescale
        {
            get
            {
                return AffectedByGlobalTimescale;
            }
            set
            {
                AffectedByGlobalTimescale = value;
            }
        }

        /// <summary>
        /// Is simulation affected by Time.timescale value.
        /// </summary>
        [FormerlySerializedAs("affectedByGlobalTimescale")]
        public bool AffectedByGlobalTimescale;

        /// <summary>
        /// Is bodies positions restricted to be in single plane.
        /// </summary>
        [Obsolete("Use KeepBodiesOnEclipticPlane instead.")]
        public bool keepBodiesOnEclipticPlane
        {
            get
            {
                return KeepBodiesOnEclipticPlane;
            }
            set
            {
                KeepBodiesOnEclipticPlane = value;
            }
        }

        /// <summary>
        /// Is bodies positions restricted to be in single plane.
        /// </summary>
        /// <remarks>
        /// May be used to make 2d world.
        /// </remarks>
        [FormerlySerializedAs("keepBodiesOnEclipticPlane")]
        public bool KeepBodiesOnEclipticPlane;

        [SerializeField]
        internal Vector3d _eclipticNormal = new Vector3d(0, 0, -1);

        [Obsolete("Use EclipticNormal instead.")]
        public Vector3d eclipticNormal
        {
            get
            {
                return _eclipticNormal;
            }
            set
            {
                EclipticNormal = value;
            }
        }

        /// <summary>
        /// Gets or sets ecliptic normal vector. Vector magnitude is always 1.
        /// </summary>
        public Vector3d EclipticNormal
        {
            get
            {
                return _eclipticNormal;
            }
            set
            {
                _eclipticNormal = value.normalized;
                // Check if value is zero or inf.
                var sqrMag = _eclipticNormal.sqrMagnitude;
                if (sqrMag < 0.99d || sqrMag > 1.01d)
                {
                    _eclipticNormal = new Vector3d(0, 0, -1);
                }

                ApplyEclipticNormalsToAllBodies();
            }
        }

        [SerializeField]
        private Vector3d _eclipticUp = new Vector3d(0, 1, 0);

        /// <summary>
        /// Gets or sets ecliptic up direction vector. Vector magnitude is always 1.
        /// </summary>
        [Obsolete("Use EclipticUp instead.")]
        public Vector3d eclipticUp
        {
            get
            {
                return EclipticUp;
            }
            set
            {
                EclipticUp = value;
            }
        }

        /// <summary>
        /// Gets or sets ecliptic up direction vector. Vector magnitude is always 1.
        /// </summary>
        public Vector3d EclipticUp
        {
            get
            {
                return _eclipticUp;
            }
            set
            {
                _eclipticUp = value.normalized;
                if (_eclipticUp.magnitude < 0.9d)
                {//check if value is zero
                    _eclipticUp = new Vector3d(0, 1, 0);
                }
                //To make sure new Up vector value is orthogonal to eclipticNormal:
                var v = CelestialBodyUtils.CrossProduct(_eclipticNormal, _eclipticUp);
                _eclipticUp = CelestialBodyUtils.CrossProduct(_eclipticNormal, v).normalized;
                ApplyEclipticNormalsToAllBodies();
            }
        }

        public SimulationControl()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            //Singleton:
            if (Instance && Instance != this)
            {
                Debug.Log("SpaceGravity2D: SimulationControl already exists");
                enabled = false;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            StartCoroutine(SimulationLoop());
        }

        private IEnumerator SimulationLoop()
        {
            yield return null;
            while (true)
            {
                if (KeepBodiesOnEclipticPlane)
                {
                    ProjectAllBodiesOnEcliptic();
                }
                if (AffectedByGlobalTimescale)
                {
                    SimulationStep(Time.unscaledDeltaTime * TimeScale);
                }
                else
                {
                    SimulationStep(Time.deltaTime * TimeScale);
                }
                yield return null;
            }
        }

        public void ProjectAllBodiesOnEcliptic()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var __bodies = GameObject.FindObjectsOfType<CelestialBody>();
                for (int i = 0; i < __bodies.Length; i++)
                {
                    __bodies[i].ProjectOntoEclipticPlane();
                }
                return;
            }
#endif
            for (int i = 0; i < Bodies.Count; i++)
            {
                Bodies[i].ProjectOntoEclipticPlane();
            }
        }

        /// <summary>
        /// Simulate gravity on scene. 
        /// Newtoninan motion and keplerian motion type
        /// </summary>
        private void SimulationStep(double deltaTime)
        {
            // Cache attractors to temporary list which improves 
            // performance in situations, when scene contains a lot of
            // non -attracting low mass celestial bodies.
            _attractorsCache.Clear();
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].isActiveAndEnabled && Bodies[i].Mass > MinAttractorMass)
                {
                    _attractorsCache.Add(Bodies[i]);
                }
            }

            KeplerianMotionStep(deltaTime);
            NewtonianMotionStep(deltaTime);

            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].OrbitData.IsDirty)
                {
                    Bodies[i].OrbitData.IsDirty = false;
                    Bodies[i].CalculateNewOrbitData();
                }
            }
        }

        private void KeplerianMotionStep(double deltaTime)
        {

            for (int i = 0; i < Bodies.Count; i++)
            {
                if (!Bodies[i].isActiveAndEnabled || Bodies[i].IsFixedPosition)
                {
                    continue;
                }
                // Keplerian motion type:
                if (Bodies[i].UseKeplerMotion && Bodies[i].IsKeplerMotion)
                {
                    if (Bodies[i].AttractorRef != null)
                    {
                        if (Bodies[i].AttractorRef.Mass < Bodies[i].Mass)
                        {
                            Bodies[i].AttractorRef = null;
                            Bodies[i].IsKeplerMotion = false;
                        }
                        else
                        {
                            if (Bodies[i].OrbitData.IsDirty)
                            {
                                Bodies[i].CalculateNewOrbitData();
                                Bodies[i].RefreshCurrentPositionAndVelocityFromOrbitData();
                            }
                            else
                            {
                                Bodies[i].UpdateObjectOrbitDynamicParameters(deltaTime);
                                Bodies[i].RefreshCurrentPositionAndVelocityFromOrbitData();
                            }
                        }
                    }
                    else
                    {
                        Bodies[i].Position = Bodies[i].Position + Bodies[i].Velocity * deltaTime;
                    }
                }
            }
        }

        private void NewtonianMotionStep(double deltaTime)
        {
            // Newtonian motion type:
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (!Bodies[i].isActiveAndEnabled || Bodies[i].IsFixedPosition || Bodies[i].UseKeplerMotion && Bodies[i].IsKeplerMotion)
                {
                    continue;
                }

                if (double.IsInfinity(Bodies[i].Velocity.x) || double.IsNaN(Bodies[i].Velocity.x))
                {
                    Debug.Log("SpaceGravity2D: Velocity is " + (double.IsNaN(Bodies[i].Velocity.x) ? "NaN !" : "INF !") + "\nbody: " + name);
                    Bodies[i].Velocity = new Vector3d();
                }
                switch (CalculationType)
                {
                    case NBodyCalculationType.Euler:
                        CelestialBodyUtils.CalcAccelerationEulerForBody(Bodies[i], deltaTime, MinAttractionRange, MaxAttractionRange, _attractorsCache);
                        if (!Bodies[i].AdditionalVelocity.isZero)
                        {
                            Bodies[i].Velocity += Bodies[i].AdditionalVelocity;
                            Bodies[i].AdditionalVelocity = Vector3d.zero;
                        }
                        if (double.IsInfinity(Bodies[i].Velocity.x) || double.IsNaN(Bodies[i].Velocity.x))
                        {
                            Bodies[i].Velocity = new Vector3d();
                        }
                        Bodies[i].Position = Bodies[i].Position + Bodies[i].Velocity * deltaTime;
                        break;
                    case NBodyCalculationType.Verlet:
                        Bodies[i].Position += Bodies[i].Velocity * (deltaTime / 2d);
                        CelestialBodyUtils.CalcAccelerationEulerForBody(Bodies[i], deltaTime, MinAttractionRange, MaxAttractionRange, _attractorsCache);
                        if (!Bodies[i].AdditionalVelocity.isZero)
                        {
                            Bodies[i].Velocity += Bodies[i].AdditionalVelocity;
                            Bodies[i].AdditionalVelocity = Vector3d.zero;
                        }
                        if (double.IsInfinity(Bodies[i].Velocity.x) || double.IsNaN(Bodies[i].Velocity.x))
                        {
                            Bodies[i].Velocity = new Vector3d();
                        }
                        Bodies[i].Position += Bodies[i].Velocity * (deltaTime / 2d);
                        break;
                    case NBodyCalculationType.RungeKutta:
                        CelestialBodyUtils.CalcAccelerationRungeKuttaForBody(Bodies[i], deltaTime, MinAttractionRange, MaxAttractionRange, _attractorsCache);
                        if (!Bodies[i].AdditionalVelocity.isZero)
                        {
                            Bodies[i].Velocity += Bodies[i].AdditionalVelocity;
                            Bodies[i].AdditionalVelocity = Vector3d.zero;
                        }
                        if (double.IsInfinity(Bodies[i].Velocity.x) || double.IsNaN(Bodies[i].Velocity.x))
                        {
                            Bodies[i].Velocity = new Vector3d();
                        }
                        Bodies[i].Position += Bodies[i].Velocity * deltaTime * 0.5d;
                        break;
                }
                Bodies[i].OrbitData.IsDirty = true;
                if (Bodies[i].UseKeplerMotion)
                {
                    // Transit to keplerian motion at next frame.
                    Bodies[i].IsKeplerMotion = true;
                }
            }
        }

        /// <summary>
        /// Find attractor, which have most gravitational influence at target body.
        /// </summary>
        /// <param name="body">Target body.</param>
        /// <returns>Most proper attractor or null.</returns>
        /// <remarks>
        /// Search logic:
        /// Calculate mutual perturbation for every pair of attractors in scene and select one, 
        /// which attracts the body with biggest force and is least affected by others.
        /// </remarks>
        public CelestialBody FindMostProperAttractor(CelestialBody body)
        {
            if (body == null)
            {
                return null;
            }
            CelestialBody resultAttractor = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            foreach (var otherBody in Bodies)
            {
                if (otherBody == body || !otherBody.isActiveAndEnabled || otherBody.Mass < MinAttractorMass || (otherBody.Position - body.Position).magnitude > Mathd.Min(MaxAttractionRange, otherBody.MaxAttractionRange))
                {
                    continue;
                }
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    otherBody.FindReferences();
                }
#endif
                if (resultAttractor == null)
                {
                    resultAttractor = otherBody;
                }
                else
                    if (CelestialBodyUtils.RelativePerturbationRatio(body, resultAttractor, otherBody) > CelestialBodyUtils.RelativePerturbationRatio(body, otherBody, resultAttractor))
                {
                    resultAttractor = otherBody;
                }
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear(); //bodies must be empty in editor mode
            }
#endif
            return resultAttractor;
        }

        /// <summary>
        /// Find attracter with biggest mass on scene.
        /// </summary>
        /// <returns>Biggest attractor or null.</returns>
        public CelestialBody FindBiggestAttractor()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            if (Bodies.Count == 0)
            {
                return null;
            }
            if (Bodies.Count == 1)
            {
                return Bodies[0];
            }
            var biggestMassIndex = -1;
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].Mass > MinAttractorMass)
                {
                    if (biggestMassIndex >= 0)
                    {
                        if (Bodies[i].Mass > Bodies[biggestMassIndex].Mass)
                        {
                            biggestMassIndex = i;
                        }
                    }
                    else
                    {
                        biggestMassIndex = i;
                    }
                }
            }
            CelestialBody result = null;
            if (biggestMassIndex >= 0)
            {
                result = Bodies[biggestMassIndex];
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear();
            }
#endif
            return result;
        }

        /// <summary>
        /// Find attractor with shortest distance to target body.
        /// </summary>
        /// <param name="body">Target body.</param>
        /// <returns>Nearest attractor or null.</returns>
        public CelestialBody FindNearestAttractor(CelestialBody body)
        {
            if (body == null)
            {
                return null;
            }
            CelestialBody resultAttractor = null;
            double _minSqrDistance = 0;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            foreach (var otherBody in Bodies)
            {
                if (otherBody == body || otherBody.Mass < MinAttractorMass || (otherBody.Position - body.Position).magnitude > Mathd.Min(MaxAttractionRange, otherBody.MaxAttractionRange))
                {
                    continue;
                }
                double _sqrDistance = (body.Position - otherBody.Position).sqrMagnitude;
                if (resultAttractor == null || _minSqrDistance > _sqrDistance)
                {
                    resultAttractor = otherBody;
                    _minSqrDistance = _sqrDistance;
                }
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear(); //_bodies must be empty in editor mode
            }
#endif
            return resultAttractor;
        }

        /// <summary>
        /// Fast and simple way to find attractor; 
        /// But note, that not always nearest attractor is most proper
        /// </summary>
        public void SetNearestAttractorForBody(CelestialBody body)
        {
            body.SetAttractor(FindNearestAttractor(body));
        }

        /// <summary>
        /// Find attractor which has biggest gravitational influence on body comparing to others. If fail, null will be assigned.
        /// It can be used in realtime for implementing more precise transitions beetween spheres of influence, 
        /// but performance cost is high
        /// </summary>
        public void SetMostProperAttractorForBody(CelestialBody body)
        {

            body.SetAttractor(FindMostProperAttractor(body));
        }

        /// <summary>
        /// Assign biggest attractor on scene to target body.
        /// </summary>
        /// <param name="body"></param>
        public void SetBiggestAttractorForBody(CelestialBody body)
        {
            body.SetAttractor(FindBiggestAttractor());
        }

        /// <summary>
        /// Used for changing gravitational parameter without breaking orbits.
        /// </summary>
        public void ChangeAllVelocitiesByFactor(double multiplier)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            for (int i = 0; i < Bodies.Count; i++)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(Bodies[i], "Proportional velocity change");
#endif
                Bodies[i].Velocity *= multiplier;
                Bodies[i].OrbitData.IsDirty = true;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear(); //_bodies must be empty in editor mode
            }
#endif
        }

        /// <summary>
        /// Refresh gravitational constant of orbitData of all celestial bodies.
        /// </summary>
        public void ApplyGravConstToAllBodies()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            for (int i = 0; i < Bodies.Count; i++)
            {
                Bodies[i].OrbitData.GravitationalConstant = _gravitationalConstant;
                Bodies[i].OrbitData.IsDirty = true;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear(); //_bodies must be empty in editor mode
            }
#endif
        }

        /// <summary>
        /// Set ecliptic value to OrbitData of all celestial bodies.
        /// </summary>
        public void ApplyEclipticNormalsToAllBodies()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies = new List<CelestialBody>(GameObject.FindObjectsOfType<CelestialBody>());
            }
#endif
            for (int i = 0; i < Bodies.Count; i++)
            {
                Bodies[i].OrbitData.EclipticNormal = EclipticNormal;
                Bodies[i].OrbitData.EclipticUp = EclipticUp;
                Bodies[i].OrbitData.IsDirty = true;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Bodies.Clear(); //_bodies must be empty in editor mode
            }
#endif
        }

        /// <summary>
        /// Register created and enabled celestial body in cache.
        /// </summary>
        /// <param name="body"></param>
        internal void RegisterBody(CelestialBody body)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            this.Bodies.Add(body);
            body.OrbitData.GravitationalConstant = _gravitationalConstant;
            body.OrbitData.EclipticNormal = EclipticNormal;
            body.OrbitData.EclipticUp = EclipticUp;
        }

        /// <summary>
        /// Remove destroyed or disabled celestial body from cache.
        /// </summary>
        /// <param name="body"></param>
        internal void UnregisterBody(CelestialBody body)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            this.Bodies.Remove(body);
        }
    }
}
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Runtime data container for actor instance.
    /// </summary>
    public class ActorData
    {
        /// <summary>
        /// Reference to root gameobject of pooled body.
        /// </summary>
        public GameObject GameObjectRef;

        /// <summary>
        /// Reference to root transform of pooled body.
        /// </summary>
        public Transform TransformRef;

        /// <summary>
        /// Reference to CelestialBody component on pooled body.
        /// </summary>
        public CelestialBody CelestialBodyRef;

        /// <summary>
        /// Reference to OrbitDisplay component on pooled body.
        /// </summary>
        public OrbitDisplay OrbitDisplayRef;

        /// <summary>
        /// Reference to PredicitonSystemTarget component on pooled body.
        /// </summary>
        public PredictionSystemTarget PredictionDisplayRef;

        /// <summary>
        /// Reference to collider on pooled body.
        /// </summary>
        public Collider ColliderRef;

        /// <summary>
        /// Reference to VelocityHandle object.
        /// </summary>
        public VelocityDisplay VelocityDisplayRef;

        /// <summary>
        /// Reference to collider, which controls velocity.
        /// </summary>
        public VelocityHandle VelocityHandleRef;
    }
}
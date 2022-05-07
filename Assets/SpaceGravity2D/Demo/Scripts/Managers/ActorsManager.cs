using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for containing references to all actors (celestial bodies with mesh visual components).
    /// Provides fabric methods for creating new actors.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ActorsManager : MonoBehaviour
    {
        /// <summary>
        /// Prefab for new actor instances.
        /// </summary>
        public GameObject ActorTemplate;

        /// <summary>
        /// Singleton static reference.
        /// </summary>
        public static ActorsManager Instance;

        /// <summary>
        /// Reference to object, which will be set as parent to all newly created actors.
        /// </summary>
        public Transform ActorsRootRef;

        /// <summary>
        /// All actors on scene.
        /// </summary>
        public List<ActorData> Actors = new List<ActorData>();

        public event Action<ActorData> OnBeforeDestroyActorEvent;

        private int _totalSpawned;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorsManager"/> class.
        /// </summary>
        public ActorsManager()
        {
            Instance = this;
        }

        /// <summary>
        /// Creates the actor on scene and cache it inside Actors list.
        /// </summary>
        /// <param name="name">The name of object.</param>
        /// <param name="position">The world position.</param>
        /// <param name="velocity">The world velocity.</param>
        /// <param name="keplerMotion">If set to <c>true</c> activate keplerMotion.</param>
        /// <param name="mass">Body mass.</param>
        /// <returns>Created actor container.</returns>
        public ActorData CreateActor(string name, Vector3d position, Vector3d velocity, bool keplerMotion, double mass)
        {
            var actor = CreateActor();
            if (!string.IsNullOrEmpty(name))
            {
                actor.GameObjectRef.name = name;
            }
            actor.CelestialBodyRef.Mass = mass;
            actor.CelestialBodyRef.SetPosition(position);
            actor.CelestialBodyRef.Velocity = velocity;
            actor.CelestialBodyRef.UseKeplerMotion = keplerMotion;
            return actor;
        }

        public ActorData FindCreatedActor(CelestialBody celestialBody)
        {
            for (int i = 0; i < this.Actors.Count; i++)
            {
                if (object.ReferenceEquals(this.Actors[i].CelestialBodyRef, celestialBody))
                {
                    return this.Actors[i];
                }
            }
            return null;
        }

        public ActorData CreateActor()
        {
            var go = Instantiate(ActorTemplate);
            go.name = "actor_" + _totalSpawned;
            _totalSpawned++;
            go.transform.SetParent(ActorsRootRef);
            go.SetActive(true);
            var cb = go.GetComponent<CelestialBody>();
            var collider = go.transform.Find("Sphere").GetComponent<SphereCollider>();
            var predSys = go.GetComponent<PredictionSystemTarget>();
            var orbDraw = go.GetComponent<OrbitDisplay>();
            if (DemoSceneManager.Instance.IsShowOrbits)
            {
                if (orbDraw.LineRenderer != null)
                {
                    orbDraw.LineRenderer.enabled = true;
                }
                predSys.enabled = true;
            }
            else
            {
                if (orbDraw.LineRenderer != null)
                {
                    orbDraw.LineRenderer.enabled = false;
                }
                predSys.enabled = false;
            }
            var v = cb.GetComponent<VelocityDisplay>();
            v.enabled = DemoSceneManager.Instance.IsShowVectors;
            var handle = go.GetComponentInChildren<VelocityHandle>();
            handle.gameObject.SetActive(DemoSceneManager.Instance.IsShowVectors);
            handle.GetComponent<SphereCollider>().isTrigger = true;

            cb.IsAttractorSearchActive = true;
            collider.isTrigger = true;
            var result = new ActorData()
            {
                GameObjectRef = go,
                TransformRef = go.transform,
                CelestialBodyRef = cb,
                ColliderRef = cb.GetComponent<Collider>(),
                OrbitDisplayRef = orbDraw,
                PredictionDisplayRef = predSys,
                VelocityDisplayRef = v,
                VelocityHandleRef = handle
            };
            Actors.Add(result);
            result.CelestialBodyRef.OnDestroyedEvent += () =>
            {
                Actors.Remove(result);
                result.CelestialBodyRef = null;
            };
            return result;
        }

        public void DestroyActor(ActorData actor)
        {
            if (actor != null && actor.GameObjectRef != null)
            {
                if (OnBeforeDestroyActorEvent != null)
                {
                    OnBeforeDestroyActorEvent(actor);
                }
                Actors.Remove(actor);
                Destroy(actor.GameObjectRef);
            }
        }
    }
}
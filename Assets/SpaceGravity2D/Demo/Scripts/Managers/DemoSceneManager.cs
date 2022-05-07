using System;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Controls scene actor selection state and global scene parameters.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class DemoSceneManager : MonoBehaviour
    {
        public static DemoSceneManager Instance;

        public Action<ActorData> OnActorSelectionChangedEvent;

        public ActorData CurrentSelected { get; private set; }

        private bool _isShowOrbits;
        private bool _isShowVectors;

        public bool IsShowOrbits
        {
            get
            {
                return _isShowOrbits;
            }
            set
            {
                _isShowOrbits = value;
                SetAllOrbitsActive(value);
            }
        }
        public bool IsShowVectors
        {
            get
            {
                return _isShowVectors;
            }
            set
            {
                _isShowVectors = value;
                SetAllVectorsActive(value);
            }
        }

        public DemoSceneManager()
        {
            Instance = this;
        }

        public void SelectActor(ActorData actor)
        {
            CurrentSelected = actor;
            if (OnActorSelectionChangedEvent != null)
            {
                OnActorSelectionChangedEvent(actor);
            }
        }

        private void SetAllOrbitsActive(bool active)
        {
            for (int i = 0; i < ActorsManager.Instance.Actors.Count; i++)
            {
                ActorsManager.Instance.Actors[i].OrbitDisplayRef.enabled = true;
                if (ActorsManager.Instance.Actors[i].CelestialBodyRef.UseKeplerMotion)
                {
                    if (ActorsManager.Instance.Actors[i].OrbitDisplayRef.LineRenderer != null)
                    {
                        ActorsManager.Instance.Actors[i].OrbitDisplayRef.LineRenderer.enabled = active;
                    }
                    ActorsManager.Instance.Actors[i].PredictionDisplayRef.enabled = false;
                }
                else
                {
                    if (ActorsManager.Instance.Actors[i].OrbitDisplayRef.LineRenderer != null)
                    {
                        ActorsManager.Instance.Actors[i].OrbitDisplayRef.LineRenderer.enabled = false;
                    }
                    ActorsManager.Instance.Actors[i].PredictionDisplayRef.enabled = active;
                }
            }
        }

        private void SetAllVectorsActive(bool active)
        {
            for (int i = 0; i < ActorsManager.Instance.Actors.Count; i++)
            {
                ActorsManager.Instance.Actors[i].VelocityDisplayRef.enabled = active;
                ActorsManager.Instance.Actors[i].VelocityHandleRef.gameObject.SetActive(active);
            }
        }
    }
}
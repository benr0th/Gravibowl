using System.Collections;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Controller for selection mark object.
    /// Automatically display selection over selected bodies.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class SelectionMark : MonoBehaviour
    {
        private TransformScalerByMass _scaler;

        private void Start()
        {
            ActorsManager.Instance.OnBeforeDestroyActorEvent += (actor) =>
              {
                  if (transform.parent == actor.TransformRef)
                  {
                      transform.SetParent(null);
                      gameObject.SetActive(false);
                  }
              };
            _scaler = GetComponent<TransformScalerByMass>();
            if (_scaler == null)
            {
                _scaler = gameObject.AddComponent<TransformScalerByMass>();
            }
            _scaler.ScaleType = TransformScalerByMass.ScaleFunctionType.SqrPow3;
            DemoSceneManager.Instance.OnActorSelectionChangedEvent += OnSelect;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_scaler.CelestialBodyRef == null)
            {
                // Tracking of target object destruction.
                gameObject.SetActive(false);
            }
        }

        private void OnSelect(ActorData actor)
        {
            if (actor != null && actor.TransformRef != null && actor.CelestialBodyRef != null)
            {
                _scaler.CelestialBodyRef = actor.CelestialBodyRef;
                gameObject.SetActive(true);
                transform.SetParent(actor.TransformRef);
                transform.localPosition = new Vector3();
            }
            else
            {
                transform.SetParent(null);
                gameObject.SetActive(false);
                _scaler.CelestialBodyRef = null;
            }
        }

    }
}
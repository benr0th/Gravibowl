using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Processing mouse inputs and provides actors selection actions.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ActorSelectByClickManager : MonoBehaviour
    {
        private void Start()
        {
            InputProvider.Instance.OnClickEvent += OnClick;
        }

        private void OnClick(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex)
        {
            var ray = Camera.main.ScreenPointToRay(pointerScreenPosition);
            RaycastHit hit;
            ActorData actor = null;
            if (Physics.Raycast(ray, out hit))
            {
                var cb = hit.collider.GetComponentInParent<CelestialBody>();
                if (cb != null)
                {
                    actor = ActorsManager.Instance.FindCreatedActor(cb);
                }
            }
            DemoSceneManager.Instance.SelectActor(actor);
        }
    }
}
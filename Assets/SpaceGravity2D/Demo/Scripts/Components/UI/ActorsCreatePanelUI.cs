using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Controller for creating and destroying bodies on scene via mouse input.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ActorsCreatePanelUI : MonoBehaviour
    {
        public Toggle AddToggle;
        public Toggle DelToggle;
        public GameObject SlidersPanel;
        public Slider SpeedSlider;
        public Slider MassSlider;
        public float MaxMass = 10000;
        public float MaxSpeed = 100;

        private void Start()
        {
            SlidersPanel.SetActive(false);
            SpeedSlider.minValue = 1;
            SpeedSlider.maxValue = MaxSpeed;
            MassSlider.minValue = 1;
            MassSlider.maxValue = MaxMass;

            AddToggle.onValueChanged.AddListener((b) =>
            {
                if (b)
                {
                    DelToggle.isOn = false;
                }
                SlidersPanel.SetActive(b);
            });
            DelToggle.onValueChanged.AddListener((b) =>
            {
                if (b)
                {
                    AddToggle.isOn = false;
                }
            });
            SpeedSlider.onValueChanged.AddListener((f) =>
            {
            });
            MassSlider.onValueChanged.AddListener((f) =>
            {
            });

            InputProvider.Instance.OnClickEvent += OnClick;
        }

        private void OnClick(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                if (AddToggle.isOn)
                {
                    var ray = Camera.main.ScreenPointToRay(pointerScreenPosition);
                    var mass = MassSlider.value;
                    var speed = SpeedSlider.value;
                    ActorsManager.Instance.CreateActor(name: null, position: new Vector3d(ray.origin), velocity: new Vector3d(ray.direction * speed), keplerMotion: false, mass: mass);
                }
                else if (DelToggle.isOn)
                {
                    var ray = Camera.main.ScreenPointToRay(pointerScreenPosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        var cb = hit.collider.GetComponentInParent<CelestialBody>();
                        if (cb != null)
                        {
                            var actor = ActorsManager.Instance.FindCreatedActor(cb);
                            if (actor != null)
                            {
                                ActorsManager.Instance.DestroyActor(actor);
                            }
                        }
                    }
                }
            }
        }
    }
}
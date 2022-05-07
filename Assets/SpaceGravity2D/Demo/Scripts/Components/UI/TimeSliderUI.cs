using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// UI Slider component, which is used to control simulation timescale.
    /// </summary>
    /// <seealso cref="UnityEngine.UI.Slider" />
    public class TimeSliderUI : Slider
    {
        public bool SnapInMiddle = true;
        public float SnapToZeroTreshold = 0.05f;
        public RectTransform LeftFillArea;
        public RectTransform RightFillArea;

        protected override void Start()
        {
            base.Start();
            var transforms = GetComponentsInChildren<RectTransform>();
            if (Application.isPlaying)
            {
                if (LeftFillArea != null)
                {
                    LeftFillArea.anchorMin = new Vector2(LeftFillArea.anchorMax.x, 0);
                    LeftFillArea.pivot = new Vector2(1f, 0.5f);
                }
                if (RightFillArea != null)
                {
                    RightFillArea.anchorMax = new Vector2(RightFillArea.anchorMin.x, 1);
                    RightFillArea.pivot = new Vector2(0f, 0.5f);
                }
                RefreshFillAreas();
                if (SimulationControl.Instance != null)
                {
                    value = (float)SimulationControl.Instance.TimeScale;
                }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            RefreshFillAreas();
            if (SimulationControl.Instance != null)
            {
                SimulationControl.Instance.TimeScale = value;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (SnapInMiddle)
            {
                if (Mathd.Abs(m_Value) < SnapToZeroTreshold)
                {
                    value = 0f;
                    RefreshFillAreas();
                }
            }
            if (SimulationControl.Instance != null)
            {
                SimulationControl.Instance.TimeScale = value;
            }
        }

        private void RefreshFillAreas()
        {
            if (handleRect != null)
            {
                if (LeftFillArea != null)
                {
                    LeftFillArea.sizeDelta = new Vector2(LeftFillArea.position.x - handleRect.position.x, LeftFillArea.sizeDelta.y);
                }
                if (RightFillArea != null)
                {
                    RightFillArea.sizeDelta = new Vector2(handleRect.position.x - RightFillArea.position.x, RightFillArea.sizeDelta.y);
                }
            }
        }
    }
}
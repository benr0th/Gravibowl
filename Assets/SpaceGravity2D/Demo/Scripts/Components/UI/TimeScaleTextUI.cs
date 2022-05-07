using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for displaying current timescale in ui text.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(Text))]
    public class TimeScaleTextUI : MonoBehaviour
    {
        private double _timescale;

        private void Start()
        {
            _timescale = SimulationControl.Instance.TimeScale;
            GetComponent<Text>().text = _timescale.ToString("0.0");
        }

        private void Update()
        {
            if (_timescale != SimulationControl.Instance.TimeScale)
            {
                _timescale = SimulationControl.Instance.TimeScale;
                GetComponent<Text>().text = _timescale.ToString("0.0");
            }
        }
    }
}
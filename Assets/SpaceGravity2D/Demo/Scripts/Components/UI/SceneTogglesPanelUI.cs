using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Controller for global scene parameters toggles.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class SceneTogglesPanelUI : MonoBehaviour
    {
        public Toggle VectorsToggle;
        public Toggle OrbitsToggle;
        private bool _isRefreshing;

        private void Start()
        {
            VectorsToggle.onValueChanged.AddListener((b) =>
            {
                if (!_isRefreshing)
                {
                    DemoSceneManager.Instance.IsShowVectors = b;
                }
            });
            OrbitsToggle.onValueChanged.AddListener((b) =>
            {
                if (!_isRefreshing)
                {
                    DemoSceneManager.Instance.IsShowOrbits = b;
                }
            });

        }

        private void RefreshState()
        {
            _isRefreshing = true;
            VectorsToggle.isOn = DemoSceneManager.Instance.IsShowVectors;
            OrbitsToggle.isOn = DemoSceneManager.Instance.IsShowOrbits;
            _isRefreshing = false;
        }
    }
}
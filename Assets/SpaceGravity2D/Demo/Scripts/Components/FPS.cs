using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Attach to UI Text component to display FPS.
    /// </summary>
    public class FPS : MonoBehaviour
    {
        public float updateFreq = 0.1f;
        private FPScounter _counter = new FPScounter(0.1f);
        private Text displayText;

        private void Start()
        {
            displayText = GetComponentInChildren<Text>();
            if (!displayText)
            {
                Debug.LogError("SpaceGravity2D: Text component not found.");
                enabled = false;
            }
            _counter = new FPScounter(updateFreq);
        }

        private void Update()
        {
            if (displayText != null)
            {
                _counter.Update(Time.deltaTime);
                displayText.text = _counter.FPS.ToString("FPS:0.00");
            }
        }

        private class FPScounter
        {
            private int frames = 0;
            private float frequency = 0;
            private float timerAscend = 0;
            private float timerDescend = 0;

            /// <summary>
            /// Gets current calculated value of FPS.
            /// </summary>
            public float FPS { get; private set; }

            public FPScounter(float freq)
            {
                frequency = freq;
                timerDescend = freq;
            }

            /// <summary>
            /// Update frame fps counter.
            /// </summary>
            /// <param name="deltatime"></param>
            public void Update(float deltatime)
            {
                timerAscend += deltatime;
                timerDescend -= deltatime;
                frames++;
                if (timerDescend <= 0)
                {
                    FPS = frames / timerAscend;
                    frames = 0;
                    timerDescend = frequency;
                    timerAscend = 0f;
                }
            }
        }
    }
}

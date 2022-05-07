using UnityEngine;

namespace SpaceGravity2D
{
    /// <summary>
    /// Component for celestial body object, which helps to control how PredictionSystem will display predicted motion path.
    /// </summary>
    [AddComponentMenu("SpaceGravity2D/PredictionSystemTarget")]
    public class PredictionSystemTarget : MonoBehaviour
    {
        public Material OrbitMaterial;
        public float OrbitWidth = 0.1f;

        private void Start()
        {
            // Empty method for make enable toggle active.
        }
    }
}
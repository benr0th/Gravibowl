using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for making any transform always look at camera.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            var screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        }
    }
}
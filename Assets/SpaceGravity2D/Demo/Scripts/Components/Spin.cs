using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Spin transform around local axis with constant speed.
    /// </summary>
    public class Spin : MonoBehaviour
    {
        public float SpeedX;
        public float SpeedY;
        public float SpeedZ;

        private void Update()
        {
            transform.Rotate(SpeedX * 60f * Time.deltaTime, SpeedY * 60f * Time.deltaTime, SpeedZ * 60f * Time.deltaTime);
        }
    }
}
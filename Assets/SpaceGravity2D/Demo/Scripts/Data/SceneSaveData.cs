using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Container for body save data.
    /// </summary>
    [System.Serializable]
    public class BodySaveData
    {
        public string Name = "name";
        public Vector3d Position;
        public Vector3d Velocity;
        public bool IsKeplerMotion;
        public double Mass = 1;
        public float Scale = 1;
    }

    /// <summary>
    /// Container for scene save data.
    /// </summary>
    [System.Serializable]
    public class SceneSaveData
    {
        public double GravConst = 0.0001f;
        public double TimeScale = 1f;
        public float ScaleMlt = 1f;
        public BodySaveData[] Bodies = new BodySaveData[0];
    }
}
using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for loading saved level on startup.
    /// Only one instance should exist on scene.
    /// </summary>
    public class SaveFileLoader : MonoBehaviour
    {
        /// <summary>
        /// Assign json file to make scene loading at scene start.
        /// </summary>
        public TextAsset SaveFileToLoad;

        private void Awake()
        {
            if (SaveFileToLoad != null)
            {
                var json = SaveFileToLoad.text;
                if (!string.IsNullOrEmpty(json))
                {
                    LoadSceneFromJson(json);
                }
            }
        }

        private void LoadSceneFromJson(string json)
        {
            SceneSaveData data;
            try
            {
                data = JsonUtility.FromJson<SceneSaveData>(json);
            }
            catch
            {
                return;
            }
            SimulationControl.Instance.GravitationalConstant = data.GravConst;
            SimulationControl.Instance.TimeScale = data.TimeScale;
            for (int i = 0; i < data.Bodies.Length; i++)
            {
                var actor = ActorsManager.Instance.CreateActor(data.Bodies[i].Name, data.Bodies[i].Position, data.Bodies[i].Velocity, data.Bodies[i].IsKeplerMotion, data.Bodies[i].Mass);
                actor.CelestialBodyRef.FindAndSetMostProperAttractor();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Save Scene To Json")]
        public void SaveSceneToJson()
        {
            SceneSaveData data = new SceneSaveData
            {
                GravConst = SimulationControl.Instance.GravitationalConstant,
                ScaleMlt = 1f,
                TimeScale = SimulationControl.Instance.TimeScale
            };
            if (Application.isPlaying)
            {
                data.Bodies = new BodySaveData[ActorsManager.Instance.Actors.Count];
                for (int i = 0; i < data.Bodies.Length; i++)
                {
                    data.Bodies[i] = new BodySaveData()
                    {
                        Scale = ActorsManager.Instance.Actors[i].TransformRef.localScale.x,
                        Position = ActorsManager.Instance.Actors[i].CelestialBodyRef.Position,
                        Velocity = ActorsManager.Instance.Actors[i].CelestialBodyRef.Velocity,
                        Mass = ActorsManager.Instance.Actors[i].CelestialBodyRef.Mass,
                        Name = ActorsManager.Instance.Actors[i].GameObjectRef.name,
                        IsKeplerMotion = ActorsManager.Instance.Actors[i].CelestialBodyRef.UseKeplerMotion
                    };
                }
            }
            else
            {
                var _bodies = GameObject.FindObjectsOfType<CelestialBody>();
                data.Bodies = new BodySaveData[_bodies.Length];
                for (int i = 0; i < data.Bodies.Length; i++)
                {
                    data.Bodies[i] = new BodySaveData()
                    {
                        Position = _bodies[i].Position,
                        Velocity = _bodies[i].Velocity,
                        Mass = _bodies[i].Mass,
                        Name = _bodies[i].name,
                        Scale = _bodies[i].transform.localScale.x,
                        IsKeplerMotion = _bodies[i].UseKeplerMotion
                    };
                }
            }
            string filename = "SavedScene";

            // Not persistence data path, because we can save only in editor.
            string path = Application.dataPath + "/SpaceGravity2D/Demo/JsonData/";
            int t = 0;
            while (t < 100 && System.IO.File.Exists(path + filename + ".txt"))
            {
                t++;
                filename = "SavedScene (" + t + ")";
            }
            System.IO.File.WriteAllText(path + filename + ".txt", JsonUtility.ToJson(data, true));
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("SpaceGravity2D: Objects saved to " + filename);
        }
#endif
    }
}
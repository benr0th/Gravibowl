using UnityEngine;
using UnityEditor;

namespace SpaceGravity2D.Inspector
{
    /// <summary>
    /// Unity editor window for SimulationControl settings for current scene.
    /// Also manages displaying orbits path lines and other tools in sceneview window.
    /// </summary>
    [InitializeOnLoad]
    public class SimulationParametersWindow : EditorWindow
    {
        /// <summary>
        /// Reference to scene simulation control.
        /// If doesn't exist, new one will be created automatically.
        /// </summary>
        private SimulationControl _simControl;

        /// <summary>
        /// Serialized reference to simulation control.
        /// </summary>
        private SerializedObject _simControlSerialized;

        /// <summary>
        /// Reference of manager for displaying HUD with orbits and tools in editorview.
        /// </summary>
        private static SceneViewDisplayManager _displayManager = new SceneViewDisplayManager();

        #region serialized properties

        private SerializedProperty inflRangeProp;
        private SerializedProperty inflRangeMinProp;
        private SerializedProperty timeScaleProp;
        private SerializedProperty minMassProp;
        private SerializedProperty calcTypeProp;
        private SerializedProperty eclipticNormalProp;
        private SerializedProperty eclipticUpProp;
        private SerializedProperty sceneViewDisplayParametersProp;

        #endregion

        #region initialization

        /// <summary>
        /// Create new window.
        /// </summary>
        [MenuItem("Window/Space Gravity 2D Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<SimulationParametersWindow>();
        }

        void OnEnable()
        {
            if (_displayManager == null)
            {
                _displayManager = new SceneViewDisplayManager();
            }
        }

        /// <summary>
        /// Find SimulationControl reference on scene.
        /// </summary>
        /// <returns>Object reference or null.</returns>
        public static SimulationControl FindSimulationControlGameObject()
        {
            if (SimulationControl.Instance != null)
            {
                return SimulationControl.Instance;
            }
            var simControl = GameObject.FindObjectOfType<SimulationControl>();
            return simControl;
        }

        /// <summary>
        /// Create new simulation control gameobject.
        /// </summary>
        /// <returns>Created object reference.</returns>
        [MenuItem("GameObject/SpaceGravity2D/Simulation Control")]
        public static SimulationControl CreateSimulationControl()
        {
            var obj = new GameObject("SimulationControl");
            Undo.RegisterCreatedObjectUndo(obj, "SimControl creation");
            //Debug.Log("SpaceGravity2D: Simulation Control created");
            return Undo.AddComponent<SimulationControl>(obj);
        }

        /// <summary>
        /// Create serialized properties for simulation control.
        /// </summary>
        private void InitializeProperties()
        {
            _simControlSerialized = new SerializedObject(_simControl);
            inflRangeProp = _simControlSerialized.FindProperty("MaxAttractionRange");
            inflRangeMinProp = _simControlSerialized.FindProperty("MinAttractionRange");
            timeScaleProp = _simControlSerialized.FindProperty("TimeScale");
            minMassProp = _simControlSerialized.FindProperty("MinAttractorMass");
            calcTypeProp = _simControlSerialized.FindProperty("CalculationType");
            eclipticNormalProp = _simControlSerialized.FindProperty("_eclipticNormal");
            eclipticUpProp = _simControlSerialized.FindProperty("_eclipticUp");
            sceneViewDisplayParametersProp = _simControlSerialized.FindProperty("SceneElementsDisplayParameters");
        }
        #endregion

        #region scenewindow and editorwindow GUI

        /// <summary>
        /// Scroll rect state for current window.
        /// </summary>
        private Vector2 _scrollPos;

        /// <summary>
        /// Editor window onGUI.
        /// </summary>
        private void OnGUI()
        {
            if (!_simControl || _simControlSerialized == null)
            {
                _simControl = FindSimulationControlGameObject();
                if (!_simControl)
                {
                    EditorGUILayout.LabelField("SimulationControl instance not found on scene.");
                    return;
                }
                InitializeProperties();
            }

            if (SimulationControl.Instance == null)
            {
                SimulationControl.Instance = _simControl;
            }

            _simControlSerialized.Update();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true, GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Global Parameters:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(calcTypeProp, new GUIContent("N-Body algorithm(?)", "Euler - fastest performance, \nVerlet - fast and more stable, \nRungeKutta - more precise."));

            var gravConst = EditorGUILayout.DoubleField(new GUIContent("Gravitational Constant(?)", "Main constant. The real value 6.67384 * 10E-11 may not be very useful for gaming purposes."), _simControl.GravitationalConstant);
            if (gravConst != _simControl.GravitationalConstant)
            {
                Undo.RecordObject(_simControl, "Grav.Const change");
                _simControl.GravitationalConstant = gravConst;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Change Grav. Const. with proportional adjusting of all velocities on scene:");
                gravConst = EditorGUILayout.DoubleField(new GUIContent("Grav.Const.Proportional(?)", "Change gravitational constant AND keep all orbits unaffected."), _simControl.GravitationalConstant);
                if (gravConst != _simControl.GravitationalConstant)
                {
                    Undo.RecordObject(_simControl, "Grav.Const change");
                    _simControl.GravitationalConstantProportional = gravConst;
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(inflRangeProp, new GUIContent("Max influence range(?)", "Global max range of n-body attraction."));
            EditorGUILayout.PropertyField(inflRangeMinProp, new GUIContent("Min influence range(?)", "Global min range of n-body attraction."));
            EditorGUILayout.PropertyField(timeScaleProp, new GUIContent("Time Scale(?)", "Time multiplier. Note: high value will decrease n-body calculations precision."));
            EditorGUILayout.PropertyField(minMassProp, new GUIContent("Min attractor mass(?)", "Mass threshold for body to became attractor."));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Keep ALL bodies on ecliptic plane:");
            var keep2d = EditorGUILayout.Toggle(new GUIContent("2d mode(?)", "Force all bodies to project positions and velocities onto ecliptic plane."), _simControl.KeepBodiesOnEclipticPlane);
            if (keep2d != _simControl.KeepBodiesOnEclipticPlane)
            {
                Undo.RecordObject(_simControl, "2d mode toggle");
                _simControl.KeepBodiesOnEclipticPlane = keep2d;
                if (keep2d)
                {
                    _simControl.ProjectAllBodiesOnEcliptic();
                }
            }

            EditorGUILayout.LabelField("Is affected by global timescale:");
            var scaledTime = EditorGUILayout.Toggle(new GUIContent("Global timescale(?)", "Toggle ignoring of Time.timeScale."), _simControl.AffectedByGlobalTimescale);
            if (scaledTime != _simControl.AffectedByGlobalTimescale)
            {
                Undo.RecordObject(_simControl, "affected by timescale toggle");
                _simControl.AffectedByGlobalTimescale = scaledTime;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(eclipticNormalProp, new GUIContent("Ecliptic Normal Vector(?)", "Perpendicular to ecliptic plane."));
            EditorGUILayout.PropertyField(eclipticUpProp, new GUIContent("Ecliptic Up Vector(?)", "Up vector on ecliptic plane. Always perpendicular to ecliptic normal. Used for rotation tool."));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Set ecliptic normal along axis:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X"))
            {
                SetEclipticNormal(new Vector3d(1, 0, 0), new Vector3d(0, 0, 1));
            }

            GUILayout.Space(6);
            if (GUILayout.Button("-X"))
            {
                SetEclipticNormal(new Vector3d(-1, 0, 0), new Vector3d(0, 0, -1));
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Y"))
            {
                SetEclipticNormal(new Vector3d(0, 1, 0), new Vector3d(0, 0, 1));
            }

            GUILayout.Space(6);
            if (GUILayout.Button("-Y"))
            {
                SetEclipticNormal(new Vector3d(0, -1, 0), new Vector3d(0, 0, -1));
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Z"))
            {
                SetEclipticNormal(new Vector3d(0, 0, 1), new Vector3d(1, 0, 0));
            }

            GUILayout.Space(6);
            if (GUILayout.Button("-Z"))
            {
                SetEclipticNormal(new Vector3d(0, 0, -1), new Vector3d(-1, 0, 0));
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Ecliptic plane is used to calculate ascending/descending nodes,");
            EditorGUILayout.LabelField("orbit inclination and for 2D-restricted simulation (if this option is active).");
            bool eclipticRotateTool = GUILayout.Toggle(_displayManager.IsEclipticRotating, "Rotate Ecliptic Plane", "Button");
            if (eclipticRotateTool != _displayManager.IsEclipticRotating)
            {
                _displayManager.IsEclipticRotating = eclipticRotateTool;
            }

            bool orbitRotateTool = GUILayout.Toggle(_displayManager.IsOrbitRotating, "Rotate Orbit Of Selected Obj.", "Button");
            if (orbitRotateTool != _displayManager.IsOrbitRotating)
            {
                _displayManager.IsOrbitRotating = orbitRotateTool;
            }

            EditorGUIUtility.labelWidth = 250f;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(sceneViewDisplayParametersProp, true);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            GUILayout.EndScrollView();
            if (GUI.changed)
            {
                _simControlSerialized.ApplyModifiedProperties();
                SceneView.RepaintAll();
            }
        }

        #endregion

        private void SetEclipticNormal(Vector3d normal, Vector3d up)
        {
            Undo.RecordObject(_simControl, "Ecliptic normal change");
            _simControl.EclipticNormal = normal;
            _simControl.EclipticUp = up;
            EditorUtility.SetDirty(_simControl);
        }

        /// <summary>
        /// Tool: inverse velocity of selection.
        /// </summary>
        public static void InverseVelocityFor(GameObject[] objects)
        {
            foreach (var obj in objects)
            {
                var cBody = obj.GetComponent<CelestialBody>();
                if (cBody)
                {
                    Undo.RecordObject(cBody, "Inverse velocity");
                    cBody.RelativeVelocity = -cBody.RelativeVelocity;
                    cBody.TerminateKeplerMotion();
                    Undo.IncrementCurrentGroup();
                    EditorUtility.SetDirty(cBody);
                }
            }
        }
    }
}
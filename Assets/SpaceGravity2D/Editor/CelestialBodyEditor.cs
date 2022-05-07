using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpaceGravity2D.Inspector
{
    /// <summary>
    /// Custom editor for CelestialBody component.
    /// </summary>
    [CustomEditor(typeof(CelestialBody))]
    [CanEditMultipleObjects()]
    public class CelestialBodyEditor : Editor
    {
        /// <summary>
        /// Target reference (current selected in editor).
        /// </summary>
        private CelestialBody _body;

        /// <summary>
        /// Multiple selection references.
        /// </summary>
        private CelestialBody[] _bodies;

        private SerializedProperty dynamicChangeIntervalProp;
        private SerializedProperty attractorProp;
        private SerializedProperty maxInfRangeProp;
        private SerializedProperty velocityProp;

        /// <summary>
        /// Switch between degress and radians display.
        /// </summary>
        private bool _preferDegrees;

        /// <summary>
        /// Create new CelestialObject on scene.
        /// </summary>
        [MenuItem("GameObject/SpaceGravity2D/CelestialBody")]
        public static void CreateGameObject()
        {
            var go = new GameObject("CelestialBody");
            Undo.RegisterCreatedObjectUndo(go, "new CelestialBody");
            go.AddComponent<CelestialBody>();
            Selection.activeObject = go;
        }

        /// <summary>
        /// Initialize properties to display.
        /// </summary>
        private void OnEnable()
        {
            _body = target as CelestialBody;
            var celestials = new List<CelestialBody>();
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var gocb = Selection.gameObjects[i].GetComponent<CelestialBody>();
                if (gocb != null)
                {
                    celestials.Add(gocb);
                }
            }
            _bodies = celestials.ToArray();
            for (int i = 0; i < _bodies.Length; i++)
            {
                _bodies[i].FindReferences();
            }
            if (!_body.SimControlRef)
            {
                _body.SimControlRef = SimulationParametersWindow.FindSimulationControlGameObject() ??
                                      SimulationParametersWindow.CreateSimulationControl();
            }
            dynamicChangeIntervalProp = serializedObject.FindProperty("SearchAttractorInterval");
            attractorProp = serializedObject.FindProperty("AttractorRef");
            maxInfRangeProp = serializedObject.FindProperty("MaxAttractionRange");
            velocityProp = serializedObject.FindProperty("Velocity");

            AssignIconImage();
        }

        private void AssignIconImage()
        {
            Texture2D icon = (Texture2D)Resources.Load("Textures/icon");
            if (icon != null)
            {
                typeof(EditorGUIUtility).InvokeMember(
                    "SetIconForObject",
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    null,
                    new object[] { _body, icon }
                );
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShowActionsButtons();
            ShowToggles();
            ShowGravityProperties();
            ShowVectors();
            ShowOrbitParameters();

            if (GUI.changed)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    _bodies[i].OrbitData.IsDirty = true;
                    EditorUtility.SetDirty(_bodies[i]);
                }
            }
        }

        private void ShowToggles()
        {
            EditorGUILayout.LabelField("Toggles:", EditorStyles.boldLabel);
            ShowToggleFixedPosition();
            ShowToggleKeplerMotion();
            ShowToggleDrawOrbit();
            ShowToggleDynamicAttractorSearch();
        }

        private void ShowToggleFixedPosition()
        {
            var isFixedPropValue = GUILayout.Toggle(_body.IsFixedPosition, new GUIContent("Is fixed position", "Relative to attractor"), "Button");
            if (isFixedPropValue != _body.IsFixedPosition)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Toggle fixed position");
                    _bodies[i].IsFixedPosition = isFixedPropValue;
                }
            }
        }

        private void ShowToggleKeplerMotion()
        {
            GUILayout.BeginHorizontal();
            {
                var useRailValue = GUILayout.Toggle(_body.UseKeplerMotion, new GUIContent("Kepler Motion", "Static unchangable orbit motion type."), "Button");
                if (useRailValue == _body.UseKeplerMotion)
                {
                    useRailValue = !GUILayout.Toggle(!_body.UseKeplerMotion, new GUIContent("N-Body Motion", "Dynamic orbit motion type."), "Button");
                }

                if (useRailValue != _body.UseKeplerMotion)
                {
                    for (int i = 0; i < _bodies.Length; i++)
                    {
                        Undo.RecordObject(_bodies[i], "Toggle keplerian motion");
                        _bodies[i].UseKeplerMotion = useRailValue;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowToggleDrawOrbit()
        {
            var drawOrbitValue = GUILayout.Toggle(_body.IsDrawOrbit, new GUIContent("Draw Orbit", "Drawing orbits depends on global settings"), "Button");
            if (drawOrbitValue != _body.IsDrawOrbit)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Toggle object orbit draw");
                    _bodies[i].IsDrawOrbit = drawOrbitValue;
                }
            }
        }

        private void ShowToggleDynamicAttractorSearch()
        {
            var dynamicChangingValue = GUILayout.Toggle(_body.IsAttractorSearchActive, new GUIContent("Dynamic attractor changing", "search most proper attractor continiously. It is recommended not to use on many objects due to performance. For large amount of bodies better to use spheres-of-influence colliders"), "Button");
            if (dynamicChangingValue != _body.IsAttractorSearchActive)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Toggle attractor searching");
                    _bodies[i].IsAttractorSearchActive = dynamicChangingValue;
                }
            }
            if (dynamicChangingValue)
            {
                EditorGUI.indentLevel++;
                ShowFloatFieldAttractorSearchInterval();
                EditorGUI.indentLevel--;
            }
        }

        private void ShowFloatFieldAttractorSearchInterval()
        {
            EditorGUI.showMixedValue = dynamicChangeIntervalProp.hasMultipleDifferentValues;
            var intervalValue = EditorGUILayout.FloatField(new GUIContent("search interval", "in seconds"), dynamicChangeIntervalProp.floatValue);
            if (intervalValue != dynamicChangeIntervalProp.floatValue)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Change search interval");
                    _bodies[i].SearchAttractorInterval = intervalValue;
                }
            }
            EditorGUI.showMixedValue = false;
        }

        private void ShowInputFieldMass()
        {
            var mixedMass = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].Mass != _body.Mass)
                {
                    mixedMass = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedMass;
            var massValue = EditorGUILayout.DoubleField("Mass", _body.Mass);
            if (massValue != _body.Mass)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Mass change");
                    _bodies[i].Mass = massValue;
                }
            }
        }

        private void ShowInputFieldInfluenceRange()
        {
            EditorGUI.showMixedValue = maxInfRangeProp.hasMultipleDifferentValues;
            var maxInfValue = EditorGUILayout.FloatField(
                new GUIContent("influence range:", "Body's own max influence range of attraction force. this option competes with the same global property"),
                maxInfRangeProp.floatValue
                );
            if (maxInfValue != maxInfRangeProp.floatValue)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Body gravity range change");
                    _bodies[i].MaxAttractionRange = maxInfValue;
                }
            }
        }

        private void ShowInputFieldAttractorReference()
        {
            EditorGUI.showMixedValue = attractorProp.hasMultipleDifferentValues;
            var attrValue = EditorGUILayout.ObjectField(new GUIContent("Attractor"), attractorProp.objectReferenceValue, typeof(CelestialBody), true) as CelestialBody;
            if (attrValue != attractorProp.objectReferenceValue)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    if (_bodies[i] != attrValue)
                    {
                        Undo.RecordObject(_bodies[i], "Attractor ref change");
                        _bodies[i].AttractorRef = attrValue;
                    }
                }
            }
        }

        private void ShowButtonRemoveAttractor()
        {
            if (attractorProp.objectReferenceValue == null)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Remove attractor"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    if (_bodies[i].AttractorRef != null)
                    {
                        Undo.RecordObject(_bodies[i], "Removing attractor ref");
                        _bodies[i].AttractorRef = null;
                        _bodies[i].TerminateKeplerMotion();
                    }
                }
            }
            GUI.enabled = true;
        }

        private void ShowGravityProperties()
        {
            ShowInputFieldMass();
            ShowInputFieldInfluenceRange();
            ShowInputFieldAttractorReference();
            ShowButtonFindNearestAttractor();
            ShowButtonFindBiggestAttractor();
            ShowButtonFindMostPropperAttractor();
            ShowButtonRemoveAttractor();
        }

        private void ShowVectors()
        {
            EditorGUILayout.BeginVertical("box");
            {
                ShowVectorFieldVelocity();
                ShowInputFieldVelocityMagnitude();
                ShowButtonInverseVelocity();
                ShowButtonResetVelocity();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("box");
            {
                ShowVectorFieldRelativeVelocity();
                ShowInputFieldRelativeVelocityMagnitude();
                ShowButtonInverseRelativeVelocity();
                ShowButtonResetRelativeVelocity();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            ShowVectorFieldBodyPosition();
        }

        private void ShowVectorFieldVelocity()
        {
            EditorGUI.showMixedValue = velocityProp.hasMultipleDifferentValues;
            EditorGUILayout.PropertyField(velocityProp, new GUIContent("Velocity(?)", "World space Velocity vector."));
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void ShowInputFieldVelocityMagnitude()
        {
            var mixedLen = false;
            var lenSqr = _body.Velocity.sqrMagnitude;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (lenSqr != _bodies[i].Velocity.sqrMagnitude)
                {
                    mixedLen = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedLen;
            var startLen = _body.Velocity.magnitude;
            var lenValue = EditorGUILayout.FloatField(new GUIContent("Velocity magnitude", tooltip: "You can edit velocity magnitude without changing direction."), (float)startLen);
            if (lenValue != startLen)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    var bodyVelocityLen = _bodies[i].Velocity.magnitude;
                    if (!Mathd.Approximately(lenValue, bodyVelocityLen))
                    {
                        Undo.RecordObject(_bodies[i], "Velocity magnitude change");
                        _bodies[i].Velocity = _bodies[i].Velocity * (Mathd.Approximately(bodyVelocityLen, 0) ? lenValue : lenValue / bodyVelocityLen);
                    }
                }
            }
        }

        private void ShowVectorFieldRelativeVelocity()
        {
            var mixedRelVelocity = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i] != _body && _bodies[i].RelativeVelocity != _body.RelativeVelocity)
                {
                    mixedRelVelocity = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedRelVelocity;
            var relVelocityValue = Vector3dField(new GUIContent("Relative Velocity(?)", "Velocity relative to orbit attractor."), _body.RelativeVelocity);// EditorGUILayout.Vector3Field("Relative velocity:", (Vector3)_body.RelativeVelocity);
            if (relVelocityValue != _body.RelativeVelocity)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Rel.Velocity change");
                    _bodies[i].RelativeVelocity = relVelocityValue;
                }
            }
        }
        private void ShowInputFieldRelativeVelocityMagnitude()
        {
            var mixedRelLen = false;
            var relLenSqr = _body.RelativeVelocity.sqrMagnitude;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (relLenSqr != _bodies[i].RelativeVelocity.sqrMagnitude)
                {
                    mixedRelLen = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedRelLen;
            var startRelLen = _body.RelativeVelocity.magnitude;
            var relLenValue = EditorGUILayout.FloatField(new GUIContent("Rel.Velocity magn.(?)", "Velocity magniute, relative to orbit attractor."), (float)startRelLen);
            if (relLenValue != startRelLen)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    var bodyRelVelocityLen = _bodies[i].RelativeVelocity.magnitude;
                    if (!Mathd.Approximately(relLenValue, bodyRelVelocityLen))
                    {
                        Undo.RecordObject(_bodies[i], "Rel.Velocity magnitude change");
                        _bodies[i].RelativeVelocity = _bodies[i].RelativeVelocity * (Mathd.Approximately(bodyRelVelocityLen, 0) ? relLenValue : relLenValue / bodyRelVelocityLen);
                    }
                }
            }
        }

        private void ShowVectorFieldBodyPosition()
        {
            EditorGUI.BeginChangeCheck();
            var relPositionValue = Vector3dField(new GUIContent("Relative Position(?)", "Position relative to attractor."), _body.RelativePosition);
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Rel.Position change");
                    _bodies[i].RelativePosition = relPositionValue;
                }
            }
        }

        private void ShowButtonInverseVelocity()
        {
            if (GUILayout.Button("Inverse velocity"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    if (!_bodies[i].Velocity.isZero)
                    {
                        Undo.RecordObject(_bodies[i], "Inverse velocity");
                        _bodies[i].Velocity = -1 * _bodies[i].Velocity;
                    }
                }
            }
        }

        private void ShowButtonResetVelocity()
        {
            if (GUILayout.Button("Reset global velocity"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Reset Velocity");
                    _bodies[i].Velocity = Vector3d.zero;
                    _bodies[i].TerminateKeplerMotion();
                }
            }
        }

        private void ShowButtonInverseRelativeVelocity()
        {
            if (GUILayout.Button("Inverse relative velocity"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    if (!_bodies[i].RelativeVelocity.isZero)
                    {
                        Undo.RecordObject(_bodies[i], "Inverse rel velocity");
                        _bodies[i].RelativeVelocity = -1 * _bodies[i].RelativeVelocity;
                    }
                }
            }
        }

        private void ShowButtonResetRelativeVelocity()
        {
            if (GUILayout.Button("Reset relative velocity"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Reset Rel. Velocity");
                    _bodies[i].RelativeVelocity = Vector3d.zero;
                    _bodies[i].TerminateKeplerMotion();
                }
            }
        }

        private void ShowButtonFindNearestAttractor()
        {
            if (GUILayout.Button(new GUIContent("Find nearest attractor", "Note: nearest attractor not always most proper")))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Nearest attractor assign");
                    _bodies[i].FindAndSetNearestAttractor();
                    _bodies[i].TerminateKeplerMotion();
                }
            }
        }

        private void ShowButtonFindBiggestAttractor()
        {
            if (GUILayout.Button(new GUIContent("Find biggest attractor")))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Biggest attractor assign");
                    _bodies[i].FindAndSetBiggestAttractor();
                    _bodies[i].TerminateKeplerMotion();
                }
            }
        }

        private void ShowButtonFindMostPropperAttractor()
        {
            if (GUILayout.Button(new GUIContent("Find most proper attractor", "Choose most realistic attractor for this body at current position")))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Most proper attractor assign");
                    _bodies[i].FindAndSetMostProperAttractor();
                    _bodies[i].TerminateKeplerMotion();
                }
            }
        }

        private void ShowButtonCircularizeOrbit()
        {
            if (!_body.AttractorRef)
            {
                GUI.enabled = false; //turn button off if attractor object is not assigned
            }
            if (GUILayout.Button("Make Orbit Circle"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Rounding orbit");
                    _bodies[i].MakeOrbitCircle();
                    _bodies[i].TerminateKeplerMotion();
                }
            }
            if (!_body.AttractorRef)
            {
                GUI.enabled = true;
            }
        }

        private void ShowButtonProjection()
        {
            if (GUILayout.Button("Project onto ecliptic plane"))
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i], "Projection onto ecliptic");
                    _bodies[i].ProjectOntoEclipticPlane();
                }
            }
        }

        private void ShowButtonOrbitRotateTool()
        {
            if (SceneViewDisplayManager.Instance != null)
            {
                var b = GUILayout.Toggle(SceneViewDisplayManager.Instance.IsOrbitRotating, new GUIContent("Rotate Orbit Tool"), "Button");
                if (b != SceneViewDisplayManager.Instance.IsOrbitRotating)
                {
                    SceneViewDisplayManager.Instance.IsOrbitRotating = b;
                }
            }
        }

        private void ShowButtonVelocityRotateTool()
        {
            if (SceneViewDisplayManager.Instance != null)
            {
                var b = GUILayout.Toggle(SceneViewDisplayManager.Instance.IsVelocityRotating, new GUIContent("Rotate Velocity Tool"), "Button");
                if (b != SceneViewDisplayManager.Instance.IsVelocityRotating)
                {
                    SceneViewDisplayManager.Instance.IsVelocityRotating = b;
                }
            }
        }

        private void ShowActionsButtons()
        {
            EditorGUILayout.LabelField("Tools:", EditorStyles.boldLabel);
            ShowButtonCircularizeOrbit();
            ShowButtonProjection();

            ShowButtonOrbitRotateTool();
            ShowButtonVelocityRotateTool();
        }

        private void ShowOrbitParameters()
        {
            EditorGUILayout.LabelField("Current state:", EditorStyles.boldLabel);
            if (_body.AttractorRef == null)
            {
                ShowOrbitNullState();
                return;
            }
            var preferDegrees = _preferDegrees;
            ShowAngleUnitsToggle();
            EditorGUILayout.Space();
            ShowInputFieldMeanAnomaly(preferDegrees);
            EditorGUILayout.Space();
            ShowInputFieldTrueAnomaly(preferDegrees);
            EditorGUILayout.Space();
            ShowInputFieldEccAnomaly(preferDegrees);
            EditorGUILayout.Space();
            ShowOrbitState(preferDegrees);
        }

        private void ShowOrbitNullState()
        {
            EditorGUILayout.LabelField("Eccentricity", "-");
            EditorGUILayout.LabelField("Mean Anomaly", "-");
            EditorGUILayout.LabelField("True Anomaly", "-");
            EditorGUILayout.LabelField("Eccentric Anomaly", "-");
            EditorGUILayout.LabelField("Argument of Periapsis", "-");
            EditorGUILayout.LabelField("Apoapsis", "-");
            EditorGUILayout.LabelField("Periapsis", "-");
            EditorGUILayout.LabelField("Period", "-");
            EditorGUILayout.LabelField("Energy", "-");
            EditorGUILayout.LabelField("Distance to focus", "-");
            EditorGUILayout.LabelField("Semi major axys", "-");
            EditorGUILayout.LabelField("Semi minor axys", "-");
        }

        private void ShowAngleUnitsToggle()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Angle display:");
                _preferDegrees = !GUILayout.Toggle(!_preferDegrees, "Radians", "Button");
                _preferDegrees = GUILayout.Toggle(_preferDegrees, "Degrees", "Button");
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowInputFieldEccentricity()
        {
            bool mixedEcc = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].Eccentricity != _body.Eccentricity)
                {
                    mixedEcc = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedEcc;
            var eccValue = EditorGUILayout.DoubleField(new GUIContent("Eccentricity"), _body.Eccentricity);
            if (eccValue != _body.Eccentricity)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i].transform, "Eccentricity change");
                    Undo.RecordObject(_bodies[i], "Eccentricity change");
                    _bodies[i].Eccentricity = eccValue;
                }
            }
        }

        private void ShowInputFieldMeanAnomaly(bool preferDegrees)
        {
            bool mixedAnomaly_m = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].MeanAnomaly != _body.MeanAnomaly)
                {
                    mixedAnomaly_m = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedAnomaly_m;
            var anomalyInput_m = preferDegrees ? _body.MeanAnomaly * Mathd.Rad2Deg : _body.MeanAnomaly;
            var anomalyValue_m = EditorGUILayout.DoubleField(new GUIContent("Mean Anomaly(rad)"), anomalyInput_m);
            if (anomalyValue_m != anomalyInput_m)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i].transform, "Mean anomaly change");
                    Undo.RecordObject(_bodies[i], "Mean anomaly change");
                    _bodies[i].MeanAnomaly = preferDegrees ? anomalyValue_m * Mathd.Deg2Rad : anomalyValue_m;
                }
            }
            EditorGUI.showMixedValue = false;
        }

        private void ShowInputFieldTrueAnomaly(bool preferDegrees)
        {
            bool mixedAnomaly_t = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].TrueAnomaly != _body.TrueAnomaly)
                {
                    mixedAnomaly_t = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedAnomaly_t;
            var anomalyInput_t = preferDegrees ? _body.TrueAnomaly * Mathd.Rad2Deg : _body.TrueAnomaly;
            var anomalyValue_t = EditorGUILayout.DoubleField(new GUIContent("True Anomaly(rad)"), anomalyInput_t);

            if (anomalyValue_t != anomalyInput_t)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i].transform, "True anomaly change");
                    Undo.RecordObject(_bodies[i], "True anomaly change");
                    _bodies[i].TrueAnomaly = preferDegrees ? anomalyValue_t * Mathd.Deg2Rad : anomalyValue_t;
                }
            }
            EditorGUI.showMixedValue = false;
        }

        private void ShowInputFieldEccAnomaly(bool preferDegrees)
        {
            bool mixedAnomaly_e = false;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].EccentricAnomaly != _body.EccentricAnomaly)
                {
                    mixedAnomaly_e = true;
                    break;
                }
            }
            EditorGUI.showMixedValue = mixedAnomaly_e;
            var anomalyInput_e = preferDegrees ? _body.EccentricAnomaly * Mathd.Rad2Deg : _body.EccentricAnomaly;
            var anomalyValue_e = EditorGUILayout.DoubleField(new GUIContent("Eccentric Anomaly(rad)"), anomalyInput_e);
            if (anomalyValue_e != anomalyInput_e)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    Undo.RecordObject(_bodies[i].transform, "Eccentric anomaly change");
                    Undo.RecordObject(_bodies[i], "Eccentric anomaly change");
                    _bodies[i].EccentricAnomaly = preferDegrees ? anomalyValue_e * Mathd.Deg2Rad : anomalyValue_e;
                }
            }
            EditorGUI.showMixedValue = false;
        }

        private void ShowOrbitState(bool preferDegrees)
        {
            EditorGUILayout.LabelField("Apoapsis", _body.OrbitData.ApoapsisDistance.ToString());
            EditorGUILayout.LabelField("Periapsis", _body.OrbitData.PeriapsisDistance.ToString());
            EditorGUILayout.LabelField("Period", _body.OrbitData.Period.ToString());
            EditorGUILayout.LabelField("Energy", _body.OrbitData.EnergyTotal.ToString());
            EditorGUILayout.LabelField("Distance to focus", _body.OrbitData.AttractorDistance.ToString("0.000"));
            EditorGUILayout.LabelField("Semi major axis", _body.OrbitData.SemiMajorAxis.ToString("0.000"));
            EditorGUILayout.LabelField("Semi minor axis", _body.OrbitData.SemiMinorAxis.ToString("0.000"));
            EditorGUILayout.LabelField("Semi minor axis normal", _body.OrbitData.SemiMinorAxisBasis.ToString("0.000"));
            EditorGUILayout.LabelField("Semi major axis normal", _body.OrbitData.SemiMajorAxisBasis.ToString("0.000"));
            EditorGUILayout.LabelField("Orbit normal", _body.OrbitData.OrbitNormal.ToString("0.000"));
            EditorGUILayout.LabelField("Inclination", preferDegrees ? (_body.OrbitData.Inclination * Mathd.Rad2Deg).ToString("0.000") : _body.OrbitData.Inclination.ToString("0.000"));
        }

        private static Vector3d Vector3dField(GUIContent content, Vector3d vector)
        {
            // Get free space for 3 lines.
            var rect = EditorGUILayout.GetControlRect();
            EditorGUILayout.GetControlRect();
            EditorGUILayout.GetControlRect();
            var xyzWidth = 15f;
            var tempWidth = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, tempWidth - xyzWidth, rect.height), content);
            EditorGUIUtility.labelWidth = xyzWidth;
            double x = EditorGUI.DoubleField(new Rect(rect.x + tempWidth - xyzWidth, rect.y, rect.width - tempWidth + xyzWidth, rect.height), "x", vector.x);
            double y = EditorGUI.DoubleField(new Rect(rect.x + tempWidth - xyzWidth, rect.y + rect.height, rect.width - tempWidth + xyzWidth, rect.height), "y", vector.y);
            double z = EditorGUI.DoubleField(new Rect(rect.x + tempWidth - xyzWidth, rect.y + rect.height * 2, rect.width - tempWidth + xyzWidth, rect.height), "z", vector.z);
            EditorGUIUtility.labelWidth = tempWidth;
            return new Vector3d(x, y, z);
        }
    }

}
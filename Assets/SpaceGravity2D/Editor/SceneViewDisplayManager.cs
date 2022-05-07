using UnityEditor;
using UnityEngine;

namespace SpaceGravity2D
{
    /// <summary>
    /// Controller for HUD tools (orbits, buttons, labels display) in sceneview.
    /// </summary>
    public class SceneViewDisplayManager
    {
        /// <summary>
        /// Reference to simulation control, which is used to access global properties.
        /// </summary>
        private SimulationControl _simControl;

        /// <summary>
        /// Singleton static reference.
        /// </summary>
        public static SceneViewDisplayManager Instance;

        /// <summary>
        /// Stores all bodies references in scene and is updated every onGUI frame.
        /// </summary>
        private CelestialBody[] _bodies = new CelestialBody[0];

        /// <summary>
        /// SceneView buttons texture, which is autoloaded at init time from Assets/Resources folder.
        /// </summary>
        private Texture2D _arrowsBtnImage;

        /// <summary>
        /// SceneView buttons texture, which is autoloaded at init time from Assets/Resources folder.
        /// </summary>
        private Texture2D _orbitsBtnImage;

        /// <summary>
        /// SceneView gui style.
        /// </summary>
        private GUIStyle _buttonActiveStyle;

        /// <summary>
        /// SceneView gui style.
        /// </summary>
        private GUIStyle _buttonInactiveStyle;

        /// <summary>
        /// Wait interval for repeated simulation control search, if first attempt failed.
        /// </summary>
        private const double _waitDuration = 1.0;

        /// <summary>
        /// Timer for simulation control search delay.
        /// </summary>
        private double _waitTime = 0;

        /// <summary>
        /// Current state of ecliptic rotation tool (active/inactive).
        /// </summary>
        private bool _isEclipticRotating;

        /// <summary>
        /// Get the current state of ecliptic rotation tool.
        /// Sets the new state of tool.
        /// </summary>
        public bool IsEclipticRotating
        {
            get
            {
                return _isEclipticRotating;
            }
            set
            {
                _isEclipticRotating = value;
                _isOrbitRotating = false;
                _isVelocityRotating = false;
                if (value)
                {
                    _simControl.SceneElementsDisplayParameters.DrawEclipticMark = true;
                    Selection.activeGameObject = null;
                }
            }
        }

        /// <summary>
        /// Current state of orbit rotation tool.
        /// </summary>
        private bool _isOrbitRotating;

        /// <summary>
        /// Gets the state of orbit rotation tool.
        /// Sets the new state of the tool.
        /// </summary>
        public bool IsOrbitRotating
        {
            get
            {
                return _isOrbitRotating;
            }
            set
            {
                _isEclipticRotating = false;
                _isOrbitRotating = value;
                _isVelocityRotating = false;
                if (value)
                {
                    _simControl.SceneElementsDisplayParameters.DrawOrbits = true;
                }
            }
        }

        /// <summary>
        /// Current state of velocity rotation tool.
        /// </summary>
        private bool _isVelocityRotating;

        /// <summary>
        /// Gets the current state of velocity rotation tool.
        /// Sets new state of the tool.
        /// </summary>
        public bool IsVelocityRotating
        {
            get
            {
                return _isVelocityRotating;
            }
            set
            {
                _isEclipticRotating = false;
                _isOrbitRotating = false;
                _isVelocityRotating = value;
                if (value)
                {
                    _simControl.SceneElementsDisplayParameters.DrawOrbits = true;
                    _simControl.SceneElementsDisplayParameters.DrawVelocityVectors = true;
                }
            }
        }

        /// <summary>
        /// Initialize new instance of this type.
        /// </summary>
        public SceneViewDisplayManager()
        {
            EditorApplication.update += StartupUpdate;
            Instance = this;
        }

        /// <summary>
        /// Editor update subscribed callback. 
        /// </summary>
        /// <remarks>
        /// Subscribe OnSceneGUI event if _simControl is not null. 
        /// If SimulationControl is not exists on scene, continuously retry untill it's not created.
        /// </remarks>
        private void StartupUpdate()
        {
            if (_waitTime > EditorApplication.timeSinceStartup)
            {
                // Wait for check time.
                return;
            }
            if (_simControl == null)
            {
                if (SimulationControl.Instance != null)
                {
                    _simControl = SimulationControl.Instance;
                }
                else
                {
                    var simControl = GameObject.FindObjectOfType<SimulationControl>();
                    if (simControl != null)
                    {
                        SimulationControl.Instance = simControl;
                        _simControl = simControl;
                    }
                    else
                    {
                        _waitTime = EditorApplication.timeSinceStartup + _waitDuration;
                        // If simulation control is not created, exit and wait for next check time.
                        return;
                    }
                }
            }
            _arrowsBtnImage = Resources.Load("Textures/arrowsBtn") as Texture2D;
            _orbitsBtnImage = Resources.Load("Textures/orbitsBtn") as Texture2D;
            CreateButtonStyle();

            // Subscribe our OnSceneGUI for updates callbacks.
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
            SceneView.RepaintAll();

            // Don't call this function anymore.
            EditorApplication.update -= StartupUpdate;
        }

        /// <summary>
        /// Create background textures and styles for SceneView buttons.
        /// </summary>
        private void CreateButtonStyle()
        {
            int width = 50;
            int height = 50;
            int borderWidth = 1;
            
            CreateActiveButtonStyle(width, height, borderWidth);
            CreateInactiveButtonStyle(width, height, borderWidth);
        }

        private void CreateActiveButtonStyle(int width, int height, int borderWidth)
        {
            Color32 normalBGcolor_enabled = new Color32();
            Color32 normalBGcolorBorder_enabled = Color.green;

            Texture2D normalTex_enabled = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color32[] cols = new Color32[width * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cols[i * width + j] = i < borderWidth || i >= height - borderWidth || j < borderWidth || j >= width - borderWidth ? normalBGcolorBorder_enabled : normalBGcolor_enabled;
                }
            }
            normalTex_enabled.SetPixels32(cols);
            normalTex_enabled.Apply();

            _buttonActiveStyle = new GUIStyle
            {
                padding = new RectOffset(5, 5, 5, 5),
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            _buttonActiveStyle.normal.background = normalTex_enabled;
            _buttonActiveStyle.normal.textColor = Color.white;
            _buttonActiveStyle.active.background = normalTex_enabled;
            _buttonActiveStyle.active.textColor = Color.white;
            _buttonActiveStyle.hover.background = normalTex_enabled;
            _buttonActiveStyle.hover.textColor = Color.white;
            _buttonActiveStyle.focused.background = normalTex_enabled;
            _buttonActiveStyle.focused.textColor = Color.white;

        }

        private void CreateInactiveButtonStyle(int width, int height, int borderWidth)
        {
            Color32 normalBGcolor_disabled = new Color32();
            Color32 normalBGcolorBorder_disabled = Color.grey;

            Texture2D normalTex_disabled = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color32[] cols = new Color32[width * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    cols[i * width + j] = i < borderWidth || i >= height - borderWidth || j < borderWidth || j >= width - borderWidth ? normalBGcolorBorder_disabled : normalBGcolor_disabled;
                }
            }
            normalTex_disabled.SetPixels32(cols);
            normalTex_disabled.Apply();

            _buttonInactiveStyle = new GUIStyle
            {
                padding = new RectOffset(5, 5, 5, 5),
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            _buttonInactiveStyle.normal.background = normalTex_disabled;
            _buttonInactiveStyle.normal.textColor = Color.white;
            _buttonInactiveStyle.active.background = normalTex_disabled;
            _buttonInactiveStyle.active.textColor = Color.white;
            _buttonInactiveStyle.hover.background = normalTex_disabled;
            _buttonInactiveStyle.hover.textColor = Color.white;
            _buttonInactiveStyle.focused.background = normalTex_disabled;
            _buttonInactiveStyle.focused.textColor = Color.white;
        }

        /// <summary>
        /// Draw all velocitiy vectors and orbits in scene window and process mouse dragging events.
        /// </summary>
        public void OnSceneGUI(SceneView sceneView)
        {
            if (!_simControl)
            {
                // Simulation control was destroyed?
                SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
                EditorApplication.update += StartupUpdate;
                return;
            }
            // Cache scene celestial bodies.
            _bodies = GameObject.FindObjectsOfType<CelestialBody>();

            DisplayCirclesOverBodies(sceneView.rotation);
            DrawOrbitElementsAndLabels();
            DrawSceneOrbitsAndVectors();
            DrawEditorViewQuickAccessButtons();
        }

        private void DrawEditorViewQuickAccessButtons()
        {
            Handles.BeginGUI();
            var positionRect = new Rect(_simControl.SceneElementsDisplayParameters.SceneViewButtonsPosition + new Vector2(5, 5), new Vector2(40, 40) * _simControl.SceneElementsDisplayParameters.SceneViewButtonsScale);
            if (GUI.Button(positionRect, _arrowsBtnImage, _simControl.SceneElementsDisplayParameters.DrawVelocityVectors ? _buttonActiveStyle : _buttonInactiveStyle))
            {
                Undo.RecordObject(_simControl, "toggle velocity arrows display");
                _simControl.SceneElementsDisplayParameters.DrawVelocityVectors = !_simControl.SceneElementsDisplayParameters.DrawVelocityVectors;
                EditorUtility.SetDirty(_simControl);
            }
            positionRect.y += 45 * _simControl.SceneElementsDisplayParameters.SceneViewButtonsScale;
            if (GUI.Button(positionRect, _orbitsBtnImage, _simControl.SceneElementsDisplayParameters.DrawOrbits ? _buttonActiveStyle : _buttonInactiveStyle))
            {
                Undo.RecordObject(_simControl, "toggle orbits display");
                _simControl.SceneElementsDisplayParameters.DrawOrbits = !_simControl.SceneElementsDisplayParameters.DrawOrbits;
                EditorUtility.SetDirty(_simControl);
            }
            Handles.EndGUI();
        }

        #region tools and utils

        private void DrawOrbitElementsAndLabels()
        {
            var prms = _simControl.SceneElementsDisplayParameters;
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].IsValidOrbit)
                {
                    if (prms.DrawPeriapsisPoint)
                    {
                        // Periapsis.
                        DrawX((Vector3)_bodies[i].OrbitPeriapsisPoint, prms.CirclesScale, Color.green, (Vector3)_bodies[i].OrbitData.OrbitNormal, (Vector3)_bodies[i].OrbitData.SemiMajorAxisBasis);
                    }
                    if (prms.DrawPeriapsisLine)
                    {
                        Handles.color = new Color(1, 1, 0);
                        Handles.DrawLine((Vector3)_bodies[i].AttractorRef.Position, (Vector3)_bodies[i].OrbitPeriapsisPoint);
                    }
                    if (prms.DrawPeriapsisLabel)
                    {
                        DrawLabelScaled((Vector3)_bodies[i].OrbitPeriapsisPoint, "P", Color.white, 10f);
                    }
                    if (prms.DrawApoapsisPoint || prms.DrawApoapsisLabel || prms.DrawApoapsisLine)
                    {
                        // Apoapsis.
                        if (!double.IsInfinity(_bodies[i].OrbitApoapsisPoint.x) && !double.IsNaN(_bodies[i].OrbitApoapsisPoint.x))
                        {
                            if (prms.DrawApoapsisPoint)
                            {
                                DrawX((Vector3)_bodies[i].OrbitApoapsisPoint, prms.CirclesScale, Color.green, (Vector3)_bodies[i].OrbitData.OrbitNormal, (Vector3)_bodies[i].OrbitData.SemiMajorAxisBasis);
                            }
                            if (prms.DrawApoapsisLine)
                            {
                                Handles.color = new Color(1, 0.5f, 0);
                                Handles.DrawLine((Vector3)_bodies[i].AttractorRef.Position, (Vector3)_bodies[i].OrbitApoapsisPoint);
                            }
                            if (prms.DrawApoapsisLabel)
                            {
                                DrawLabelScaled((Vector3)_bodies[i].OrbitApoapsisPoint, "A", Color.white, 10f);
                            }
                        }
                    }
                    if (prms.DrawCenterOfMassPoint)
                    {
                        // Center of mass.
                        Handles.color = Color.white;
                        Handles.DrawWireDisc((Vector3)_bodies[i].CenterOfMass, (Vector3)_bodies[i].OrbitData.OrbitNormal, 1f);
                    }
                    if (prms.DrawAscendingNodeLabel || prms.DrawAscendingNodeLine || prms.DrawAscendingNodePoint)
                    {
                        Vector3 asc;
                        if (_bodies[i].GetAscendingNode(out asc))
                        {
                            // Ascending node.
                            asc = asc + (Vector3)_bodies[i].AttractorRef.Position;
                            if (prms.DrawAscendingNodePoint)
                            {
                                DrawX(asc, prms.CirclesScale, Color.blue, (Vector3)_bodies[i].OrbitData.OrbitNormal, (Vector3)_bodies[i].OrbitData.SemiMajorAxisBasis);
                            }
                            if (prms.DrawAscendingNodeLine)
                            {
                                Handles.color = new Color(0.1f, 0.3f, 0.8f, 0.8f);
                                Handles.DrawLine((Vector3)_bodies[i].AttractorRef.Position, asc);
                            }
                            if (prms.DrawAscendingNodeLabel)
                            {
                                DrawLabelScaled(asc, "ASC", Color.white, 10f);
                            }
                        }
                    }
                    if (prms.DrawDescendingNodeLabel || prms.DrawDescendingNodeLine || prms.DrawDescendingNodePoint)
                    {
                        Vector3 desc;
                        if (_bodies[i].GetDescendingNode(out desc))
                        {
                            // Descending node.
                            desc = desc + (Vector3)_bodies[i].AttractorRef.Position;
                            if (prms.DrawDescendingNodePoint)
                            {
                                DrawX(desc, prms.CirclesScale, Color.blue, (Vector3)_bodies[i].OrbitData.OrbitNormal, (Vector3)_bodies[i].OrbitData.SemiMajorAxisBasis);
                            }
                            if (prms.DrawDescendingNodeLine)
                            {
                                Handles.color = new Color(0.8f, 0.3f, 0.1f, 0.8f);
                                Handles.DrawLine((Vector3)_bodies[i].AttractorRef.Position, desc);
                            }
                            if (prms.DrawDescendingNodeLabel)
                            {
                                DrawLabelScaled(desc, "DESC", Color.white, 10f);
                            }
                        }
                    }
                    if (prms.DrawInclinationLabel)
                    {
                        // Inclination.
                        DrawInclinationMarkForBody(_bodies[i], prms.NormalAxisScale);
                    }
                    if (prms.DrawRadiusVector)
                    {
                        // Radius vector.
                        Handles.color = Color.gray;
                        Handles.DrawLine((Vector3)_bodies[i].AttractorRef.Position, (Vector3)_bodies[i].Position);
                    }
                    if (prms.DrawOrbitsNormal)
                    {
                        // Orbit normal.
                        Handles.color = new Color(0.16f, 0.92f, 0.88f, 0.8f);
                        Handles.DrawLine((Vector3)_bodies[i].OrbitCenterPoint, (Vector3)_bodies[i].OrbitCenterPoint + (Vector3)_bodies[i].OrbitData.OrbitNormal * prms.NormalAxisScale * 5f);
                        Handles.DrawWireDisc((Vector3)_bodies[i].OrbitCenterPoint, (Vector3)_bodies[i].OrbitData.OrbitNormal, prms.NormalAxisScale * 2f);
                    }
                    if (prms.DrawSemiAxis)
                    {
                        // SemiMinor axis normal.
                        Handles.color = Color.blue;
                        Handles.DrawLine((Vector3)_bodies[i].OrbitCenterPoint, (Vector3)_bodies[i].OrbitCenterPoint + (Vector3)_bodies[i].OrbitData.SemiMinorAxisBasis * prms.NormalAxisScale * 5f);
                        // SemiMajor axis normal.
                        Handles.color = Color.red;
                        Handles.DrawLine((Vector3)_bodies[i].OrbitCenterPoint, (Vector3)_bodies[i].OrbitCenterPoint + (Vector3)_bodies[i].OrbitData.SemiMajorAxisBasis * prms.NormalAxisScale * 5f);
                    }
                }


                if (_simControl.SceneElementsDisplayParameters.DrawBodiesEclipticProjection)
                {
                    Handles.color = Color.yellow;
                    var ProjectionPos = _bodies[i].Position - _simControl.EclipticNormal * CelestialBodyUtils.DotProduct(_bodies[i].Position, _simControl.EclipticNormal);
                    if ((ProjectionPos - _bodies[i].Position).sqrMagnitude > 1e-003d)
                    {
                        Handles.DrawDottedLine((Vector3)_bodies[i].Position, (Vector3)ProjectionPos, 2f);
                        Handles.DrawWireDisc((Vector3)ProjectionPos, (Vector3)_simControl.EclipticNormal, _simControl.SceneElementsDisplayParameters.CirclesScale);
                    }
                }
            }
        }

        private void DrawSceneOrbitsAndVectors()
        {
            DrawAllOrbitsInEditor();
            ProcessVelocityArrows();
            DisplayEcliptic();
            EclipticRotationTool();
            OrbitRotationTool();
            VelocityRotationTool();
        }

        private void DisplayCirclesOverBodies(Quaternion rotation)
        {
            if (_simControl.SceneElementsDisplayParameters.DrawCirclesOverBodies)
            {
                Handles.color = new Color(0.56f, 0.89f, 0.4f, 0.6f);
                var selectedCB = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<CelestialBody>() : null;
                for (int i = 0; i < _bodies.Length; i++)
                {
                    if (selectedCB == _bodies[i])
                    {
                        continue;
                    }
                    if (Handles.Button((Vector3)_bodies[i].Position, rotation, (float)_simControl.SceneElementsDisplayParameters.CirclesScale, (float)_simControl.SceneElementsDisplayParameters.CirclesScale, Handles.CircleHandleCap))
                    {
                        Selection.activeGameObject = _bodies[i].gameObject;
                        break;
                    }
                }
            }
        }

        private void DisplayEcliptic()
        {
            if (_simControl.SceneElementsDisplayParameters.DrawEclipticMark)
            {
                DrawArrow(Vector3.zero, (Vector3)_simControl.EclipticNormal * _simControl.SceneElementsDisplayParameters.EclipticMarkScale, Color.magenta, (Vector3)_simControl.EclipticUp);
                Handles.color = Color.magenta;
                Handles.DrawWireDisc(Vector3.zero, (Vector3)_simControl.EclipticNormal, _simControl.SceneElementsDisplayParameters.EclipticMarkScale);
                Handles.color = Color.gray;
                Handles.DrawLine(Vector3.zero, (Vector3)_simControl.EclipticUp * _simControl.SceneElementsDisplayParameters.EclipticMarkScale);
            }
        }

        private void EclipticRotationTool()
        {
            if (_isEclipticRotating)
            {
                if (Selection.activeGameObject != null)
                {
                    IsEclipticRotating = false;
                    return;
                }
                var currentRot = Quaternion.LookRotation((Vector3)_simControl.EclipticNormal, (Vector3)_simControl.EclipticUp);

                var rot = Handles.RotationHandle(currentRot, Vector3.zero);
                if (GUI.changed && currentRot != rot)
                {
                    Undo.RecordObject(_simControl, "Ecliptic plane orientation change");
                    var rotFromTo = rot * Quaternion.Inverse(currentRot);
                    _simControl.EclipticNormal = new Vector3d(rotFromTo * (Vector3)_simControl.EclipticNormal);
                    _simControl.EclipticUp = new Vector3d(rotFromTo * (Vector3)_simControl.EclipticUp);
                    EditorUtility.SetDirty(_simControl);
                }

            }
        }


        private void OrbitRotationTool()
        {
            if (_isOrbitRotating)
            {
                if (Selection.activeGameObject == null)
                {
                    IsOrbitRotating = false;
                    return;
                }
                var cb = Selection.activeGameObject.GetComponent<CelestialBody>();
                if (cb == null || cb.AttractorRef == null)
                {
                    IsOrbitRotating = false;
                    return;
                }
                var currentRot = Quaternion.LookRotation((Vector3)cb.OrbitData.OrbitNormal, (Vector3)cb.OrbitData.SemiMinorAxisBasis);
                var rot = Handles.RotationHandle(currentRot, (Vector3)cb.AttractorRef.Position);
                if (GUI.changed && currentRot != rot)
                {
                    Undo.RecordObject(cb, "Orbit rotation change");
                    cb.RotateOrbitAroundFocus(rot * Quaternion.Inverse(currentRot));
                    EditorUtility.SetDirty(cb);
                }
            }
        }

        private void VelocityRotationTool()
        {
            if (_isVelocityRotating)
            {
                if (Selection.activeGameObject == null)
                {
                    IsVelocityRotating = false;
                    return;
                }
                var cb = Selection.activeGameObject.GetComponent<CelestialBody>();
                if (cb == null)
                {
                    IsOrbitRotating = false;
                    return;
                }
                var currentRot = Quaternion.LookRotation((Vector3)cb.Velocity, (Vector3)cb.OrbitData.OrbitNormal);
                var rot = Handles.RotationHandle(currentRot, (Vector3)cb.Position);
                if (GUI.changed && currentRot != rot)
                {
                    Undo.RecordObject(cb, "Velocity change");
                    var rotFromTo = rot * Quaternion.Inverse(currentRot);
                    cb.Velocity = new Vector3d(rotFromTo * (Vector3)cb.Velocity);
                    cb.OrbitData.IsDirty = true;
                    EditorUtility.SetDirty(cb);
                }
            }
        }

        /// <summary>
        /// Process mouse drag and hover events and draw velocity vectors if enabled
        /// </summary>
        private void ProcessVelocityArrows()
        {
            if (!_simControl.SceneElementsDisplayParameters.DrawVelocityVectors)
            {
                return;
            }
            Vector3d velocity = new Vector3d();
            Vector3 hpos = new Vector3();
            Vector3 pos = new Vector3();
            for (int i = 0; i < _bodies.Length; i++)
            {
                if (_bodies[i].isActiveAndEnabled)
                {
                    velocity = _simControl.SceneElementsDisplayParameters.EditGlobalVelocity ?
                        _bodies[i].Velocity :
                        _bodies[i].RelativeVelocity;
                    pos = (Vector3)(_bodies[i].Position + velocity * _simControl.SceneElementsDisplayParameters.VelocitiesArrowsScale);
                    Handles.CapFunction capFunc;
                    switch (_simControl.SceneElementsDisplayParameters.VelocityHandlerType)
                    {
                        case VelocityHandlerType.Circle:
                            capFunc = Handles.CircleHandleCap;
                            break;
                        case VelocityHandlerType.Sphere:
                            capFunc = Handles.SphereHandleCap;
                            break;
                        case VelocityHandlerType.Dot:
                            capFunc = Handles.DotHandleCap;
                            break;
                        default:
                            continue;
                    }
                    Handles.color = Color.white;
                    hpos = Handles.FreeMoveHandle(pos, Quaternion.identity, _simControl.SceneElementsDisplayParameters.HandleScale * HandleUtility.GetHandleSize(pos), Vector3.zero, capFunc);
                    if (pos != hpos)
                    {
                        // Project onto orbit plane.
                        if (_bodies[i].AttractorRef != null && !_simControl.SceneElementsDisplayParameters.EditGlobalVelocity && (_simControl.SceneElementsDisplayParameters.KeepOrbitPlaneWhileChangeVelocity || _simControl.KeepBodiesOnEclipticPlane))
                        {
                            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                            hpos = CelestialBodyUtils.GetRayPlaneIntersectionPoint((Vector3)_bodies[i].Position, (Vector3)_bodies[i].OrbitData.OrbitNormal, ray.origin, ray.direction);
                        }

                        velocity = (new Vector3d(hpos) - _bodies[i].Position) / _simControl.SceneElementsDisplayParameters.VelocitiesArrowsScale;
                        Undo.RecordObject(_bodies[i], "Velocity change");
                        if (_simControl.SceneElementsDisplayParameters.EditGlobalVelocity)
                        {
                            _bodies[i].Velocity = velocity;
                        }
                        else
                        {
                            _bodies[i].RelativeVelocity = velocity;
                        }
                        _bodies[i].OrbitData.IsDirty = true;
                        if (_simControl.SceneElementsDisplayParameters.SelectBodyWhenDraggingVelocity)
                        {
                            Selection.activeGameObject = _bodies[i].gameObject;
                        }
                    }
                }
            }
            ShowAllVelocitiesVectors();
        }


        /// <summary>
        /// Draw velocity vectors in scene view for all active celestial bodies
        /// </summary>
        private void ShowAllVelocitiesVectors()
        {
            foreach (var body in _bodies)
            {
                if (body.isActiveAndEnabled)
                {
                    if (_simControl.SceneElementsDisplayParameters.DrawArrowsHead)
                    {
                        DrawArrow(
                            body.Position,
                            body.Position + (_simControl.SceneElementsDisplayParameters.EditGlobalVelocity ? body.Velocity : body.RelativeVelocity) * _simControl.SceneElementsDisplayParameters.VelocitiesArrowsScale,
                            Selection.activeTransform != null && Selection.activeTransform == body.transform ? Color.cyan : Color.green,
                            body.IsValidOrbit ? body.OrbitData.OrbitNormal : _simControl.EclipticNormal
                        );
                    }
                    else
                    {
                        Handles.color = Selection.activeTransform == body.transform ? Color.cyan : Color.green;
                        Handles.DrawLine(
                            (Vector3)body.Position,
                            (Vector3)(body.Position + (_simControl.SceneElementsDisplayParameters.EditGlobalVelocity ? body.Velocity : body.RelativeVelocity) * _simControl.SceneElementsDisplayParameters.VelocitiesArrowsScale));
                    }
                }
            }
        }

        private static void DrawArrow(Vector3d from, Vector3d to, Color col, Vector3d normal)
        {
            DrawArrow((Vector3)from, (Vector3)to, col, (Vector3)normal);
        }

        /// <summary>
        /// Draw simple arrow in scene window at given world coordinates
        /// </summary>
        private static void DrawArrow(Vector3 from, Vector3 to, Color col, Vector3 normal)
        {
            var dir = to - from;
            float dist = dir.magnitude;
            var dirNorm = dir / dist; //normalized vector
            float headSize = dist / 6f;
            var _colBefore = Handles.color;
            Handles.color = col;
            Vector3 sideNormal = CelestialBodyUtils.CrossProduct(dir, normal).normalized;
            Handles.DrawLine(from, from + dirNorm * (dist - headSize));
            Handles.DrawLine(from + dirNorm * (dist - headSize) + sideNormal * headSize / 2f, from + dirNorm * (dist - headSize) - sideNormal * headSize / 2f);
            Handles.DrawLine(from + dirNorm * (dist - headSize) + sideNormal * headSize / 2f, from + dir);
            Handles.DrawLine(from + dirNorm * (dist - headSize) - sideNormal * headSize / 2f, from + dir);
            Handles.color = _colBefore;
        }

        /// <summary>
        /// Draw orbits for bodies which has drawing orbit enabled
        /// </summary>
        private void DrawAllOrbitsInEditor()
        {
            if (_simControl.SceneElementsDisplayParameters.DrawOrbits)
            {
                foreach (var body in _bodies)
                {
                    if (body.IsDrawOrbit)
                    {
                        DrawOrbitInEditorFor(body);
                    }
                }
            }
        }

        private void DrawOrbitInEditorFor(CelestialBody body)
        {
            int pointsCount = _simControl.SceneElementsDisplayParameters.OrbitPointsCount;
            if (body.isActiveAndEnabled)
            {
                if (!Application.isPlaying && body.AttractorRef != null && body.OrbitData.IsDirty)
                {
                    if (body.AttractorRef.Mass <= 0)
                    {
                        body.AttractorRef.Mass = 1e-007;//to avoid div by zero
                    }
                    body.CalculateNewOrbitData();
                }
                Handles.color = Color.white;
                Vector3d[] points = null;
                body.GetOrbitPointsNoAlloc(ref points, pointsCount, false, (float)_simControl.SceneElementsDisplayParameters.MaxOrbitDistance);
                for (int i = 1; i < points.Length; i++)
                {
                    Handles.DrawLine((Vector3)points[i - 1], (Vector3)points[i]);
                }
                if (_simControl.SceneElementsDisplayParameters.DrawOrbitsEclipticProjection && points.Length > 0)
                {
                    var point1 = points[0] - _simControl.EclipticNormal * CelestialBodyUtils.DotProduct(points[0], _simControl.EclipticNormal);
                    var point2 = Vector3d.zero;
                    Handles.color = Color.gray;
                    for (int i = 1; i < points.Length; i++)
                    {
                        point2 = points[i] - _simControl.EclipticNormal * CelestialBodyUtils.DotProduct(points[i], _simControl.EclipticNormal);
                        Handles.DrawLine((Vector3)point1, (Vector3)point2);
                        point1 = point2;
                    }
                }
            }
        }

        private void DrawInclinationMarkForBody(CelestialBody body, float scale)
        {
            var norm = CelestialBodyUtils.CrossProduct((Vector3)body.OrbitData.OrbitNormal, (Vector3)_simControl.EclipticNormal);
            Handles.color = Color.white;
            var p = CelestialBodyUtils.CrossProduct(norm, (Vector3)_simControl.EclipticNormal).normalized;
            Handles.DrawLine((Vector3)body.OrbitFocusPoint, (Vector3)body.OrbitFocusPoint + p * 3f * scale);
            Handles.DrawLine((Vector3)body.OrbitFocusPoint, (Vector3)body.OrbitFocusPoint + CelestialBodyUtils.RotateVectorByAngle(p, (float)body.OrbitData.Inclination, -norm.normalized) * 3f * scale);
            Handles.DrawWireArc((Vector3)body.OrbitFocusPoint, -norm, p, (float)(body.OrbitData.Inclination * Mathd.Rad2Deg), 1f * scale);
            DrawLabelScaled((Vector3)body.OrbitFocusPoint + p * scale, (body.OrbitData.Inclination * Mathd.Rad2Deg).ToString("0") + "\u00B0", Color.white, 10);
        }

        /// <summary>
        /// Draw two crossing lines in scene view.
        /// </summary>
        private static void DrawX(Vector3 pos, float size, Color col, Vector3 normal, Vector3 up)
        {
            Handles.color = col;
            Vector3 right = CelestialBodyUtils.CrossProduct(up, normal).normalized;
            Handles.DrawLine(pos + up * size, pos - up * size);
            Handles.DrawLine(pos + right * size, pos - right * size);
        }

        private static void DrawLabel(Vector3 pos, string text, Color color, float sizeMlt)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontSize = (int)sizeMlt;
            Handles.BeginGUI();
            GUI.Label(new Rect(HandleUtility.WorldToGUIPoint(pos), new Vector2(100, 100)), text, style);
            Handles.EndGUI();
        }

        private void DrawLabelScaled(Vector3 pos, string text, Color color, float sizeMlt)
        {
            if (_simControl.SceneElementsDisplayParameters.DrawLabels)
            {
                DrawLabel(pos, text, color, sizeMlt * _simControl.SceneElementsDisplayParameters.LabelsScale);
            }
        }
        #endregion
    }
}
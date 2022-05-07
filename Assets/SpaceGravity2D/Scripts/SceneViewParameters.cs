#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceGravity2D
{
    /// <summary>
    /// List of visual types of velocity line handle.
    /// </summary>
    public enum VelocityHandlerType
    {
        Circle,
        Dot,
        Sphere,
        None
    }

    /// <summary>
    /// Config class for scene tools display.
    /// </summary>
    [System.Serializable]
    public class SceneViewSettings
    {
        /// <summary>
        /// Sliders scale multiplier.
        /// </summary>
        [FormerlySerializedAs("maxRangeMlt")]
        const float MaxRangeMlt = 3f;

        /// <summary>
        /// Show editable velocity vectors of bodies.
        /// </summary>
        [Header("Velocity Vectors Parameters:")]
        [Tooltip("Show editable velocity vectors of bodies")]
        [FormerlySerializedAs("drawVelocityVectors")]
        public bool DrawVelocityVectors = true;

        /// <summary>
        /// Change velocity vectors display style.
        /// </summary>
        [Tooltip("Change velocity vectors display style")]
        [FormerlySerializedAs("drawArrowsHead")]
        public bool DrawArrowsHead = false;

        /// <summary>
        /// Select velocity handler.
        /// </summary>
        [Tooltip("Select velocity handler")]
        [FormerlySerializedAs("velocityHandlerType")]
        public VelocityHandlerType VelocityHandlerType = VelocityHandlerType.Circle;

        /// <summary>
        /// Change selection on edit.
        /// </summary>
        [Tooltip("Change selection on edit")]
        [FormerlySerializedAs("selectBodyWhenDraggingVelocity")]
        public bool SelectBodyWhenDraggingVelocity = true;

        /// <summary>
        /// Change global velocity instead of relative.
        /// </summary>
        [Tooltip("Change global velocity instead of relative")]
        [FormerlySerializedAs("editGlobalVelocity")]
        public bool EditGlobalVelocity = false;

        /// <summary>
        /// Prevent changing orientation of orbit plane, when dragging velocity vector.
        /// </summary>
        [Tooltip("Prevent changing orientation of orbit plane, when dragging velocity vector")]
        [FormerlySerializedAs("keepOrbitPlaneWhileChangeVelocity")]
        public bool KeepOrbitPlaneWhileChangeVelocity = true;

        /// <summary>
        /// Scale of velocity lines.
        /// </summary>
        [Tooltip("Scale of velocity lines")]
        [Range(0.01f, 10f * MaxRangeMlt)]
        [FormerlySerializedAs("velocitiesArrowsScale")]
        public float VelocitiesArrowsScale = 1f;

        /// <summary>
        /// Scale of velocity handles.
        /// </summary>
        [Tooltip("Scale of velocity handles")]
        [Range(0.01f, 1f * MaxRangeMlt)]
        [FormerlySerializedAs("handleScale")]
        public float HandleScale = 0.1f;

        /// <summary>
        /// Orbit display in editor parameters.
        /// </summary>
        [Space(12)]
        [Header("Orbit display in editor parameters")]
        [Tooltip("Show orbits in editor view")]
        [FormerlySerializedAs("drawOrbits")]
        public bool DrawOrbits = true;

        /// <summary>
        /// Calculated points count of orbits.
        /// </summary>
        [Range(10, 100 * MaxRangeMlt)]
        [Tooltip("Calculated points count of orbits")]
        [FormerlySerializedAs("orbitPointsCount")]
        public int OrbitPointsCount = 50;

        /// <summary>
        /// Maximal distance of orbit points of displayed orbits.
        /// </summary>
        [Tooltip("Maximal distance of orbit points of displayed orbits")]
        [FormerlySerializedAs("maxOrbitDistance")]
        public double MaxOrbitDistance = 1000d;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("Show projection of current orbits on ecliptic plane")]
        [FormerlySerializedAs("drawOrbitsEclipticProjection")]
        public bool DrawOrbitsEclipticProjection = true;

        /// <summary>
        /// Show projection of bodies on ecliptic plane.
        /// </summary>
        [Tooltip("Show projection of bodies on ecliptic plane")]
        [FormerlySerializedAs("drawBodiesEclipticProjection")]
        public bool DrawBodiesEclipticProjection = true;

        /// <summary>
        /// Scale of sceneview HUD circles.
        /// </summary>
        [Range(0.01f, 10f * MaxRangeMlt)]
        [Tooltip("Scale of sceneview HUD circles")]
        [FormerlySerializedAs("circlesScale")]
        public float CirclesScale = 1f;

        /// <summary>
        /// Show selectable marks over bodies.
        /// </summary>
        [Tooltip("Show selectable marks over bodies")]
        [FormerlySerializedAs("drawCirclesOverBodies")]
        public bool DrawCirclesOverBodies = false;

        /// <summary>
        /// Show ecliptic orientation helper mark in world center.
        /// </summary>
        [Tooltip("Show ecliptic orientation helper mark in world center")]
        [FormerlySerializedAs("drawEclipticMark")]
        public bool DrawEclipticMark = false;

        /// <summary>
        /// Scale of eliptic plane mark in sceneview.
        /// </summary>
        [Range(0.01f, 10f * MaxRangeMlt)]
        [Tooltip("Scale of eliptic plane mark in sceneview")]
        [FormerlySerializedAs("eclipticMarkScale")]
        public float EclipticMarkScale = 1f;

        /// <summary>
        /// Show line from attractor to periapsis point of orbit.
        /// </summary>
        [Space(12)]
        public bool DrawPeriapsisLine = true;

        /// <summary>
        /// Show periapsis point in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawPeriapsisPoint")]
        public bool DrawPeriapsisPoint = false;

        /// <summary>
        /// Show periapsis point label in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawPeriapsisLabel")]
        public bool DrawPeriapsisLabel = false;

        /// <summary>
        /// Show line from attractor to apoapsis point of orbit, if exist.
        /// </summary>
        public bool DrawApoapsisLine = true;
        
        /// <summary>
        /// Show apoapsis point in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawApoapsisPoint")]
        public bool DrawApoapsisPoint = false;

        /// <summary>
        /// Show apoapsis point label in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawApoapsisLabel")]
        public bool DrawApoapsisLabel = false;

        /// <summary>
        /// Show ascending node point in sceneview.
        /// </summary>
        [Space(12)]
        [FormerlySerializedAs("drawAscendingNodePoint")]
        public bool DrawAscendingNodePoint = false;

        /// <summary>
        /// Show ascending node line in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawAscendingNodeLine")]
        public bool DrawAscendingNodeLine = false;

        /// <summary>
        /// Show ascending node label in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawAscendingNodeLabel")]
        public bool DrawAscendingNodeLabel = false;

        /// <summary>
        /// Show descending node point in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawDescendingNodePoint")]
        public bool DrawDescendingNodePoint = false;

        /// <summary>
        /// Show descending node line in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawDescendingNodeLine")]
        public bool DrawDescendingNodeLine = false;

        /// <summary>
        /// Show descending node label in sceneview.
        /// </summary>
        [FormerlySerializedAs("drawDescendingNodeLabel")]
        public bool DrawDescendingNodeLabel = false;

        /// <summary>
        /// Show or hide all labels in sceneview.
        /// </summary>
        [Space(12)]
        [Tooltip("Show or hide all labels")]
        [FormerlySerializedAs("drawLabels")]
        public bool DrawLabels = true;

        /// <summary>
        /// Show current orbits inclination mark relative to ecliptic plane.
        /// </summary>
        [Tooltip("Show current orbits inclination mark relative to ecliptic plane")]
        [FormerlySerializedAs("drawInclinationLabel")]
        public bool DrawInclinationLabel = false;

        /// <summary>
        /// Scale of labels in sceneview.
        /// </summary>
        [Range(0.01f, 10f * MaxRangeMlt)]
        [FormerlySerializedAs("labelsScale")]
        public float LabelsScale = 1f;

        /// <summary>
        /// Show lines from bodies to their attractors.
        /// </summary>
        [Tooltip("Show lines from bodies to their attractors")]
        [FormerlySerializedAs("drawRadiusVector")]
        public bool DrawRadiusVector = false;

        /// <summary>
        /// Show orbit normal vector pointing from the center of orbit.
        /// </summary>
        [Tooltip("Show orbit normal vector pointing from the center of orbit")]
        [FormerlySerializedAs("drawOrbitsNormal")]
        public bool DrawOrbitsNormal = false;

        /// <summary>
        /// Show orbit semi-major and semi-minor axis vectors with origin in orbit center.
        /// </summary>
        [Tooltip("Show orbit semi-major and semi-minor axis vectors with origin in orbit center")]
        [FormerlySerializedAs("drawSemiAxis")]
        public bool DrawSemiAxis = false;

        /// <summary>
        /// Show mark of current center of mass position for each orbit.
        /// </summary>
        [Tooltip("Show mark of current center of mass position for each orbit")]
        [FormerlySerializedAs("drawCenterOfMassPoint")]
        public bool DrawCenterOfMassPoint = false;

        /// <summary>
        /// Scale of displayed unit vectors of normals and axis.
        /// </summary>
        [Range(0.01f, 10f * MaxRangeMlt)]
        [Tooltip("Scale of displayed unit vectors of normals and axis")]
        [FormerlySerializedAs("normalAxisScale")]
        public float NormalAxisScale = 1f;

        /// <summary>
        /// Scale of scenview quick-access buttons.
        /// </summary>
        [Space(12)]
        [Range(0.01f, 10f * MaxRangeMlt)]
        [FormerlySerializedAs("sceneViewButtonsScale")]
        public float SceneViewButtonsScale = 1f;

        /// <summary>
        /// Position of scenview quick-access buttons with origin in upper right corner.
        /// </summary>
        [Tooltip("Screen position of scene view toggle buttons with origin in upper right corner")]
        [FormerlySerializedAs("sceneViewButtonsPosition")]
        public Vector2 SceneViewButtonsPosition;
    }
}
#endif

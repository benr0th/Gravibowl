using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGravity2D
{
    /// <summary>
    /// Basic prediction orbits calculator (singleton).
    /// Simulates whole scene with Euler n-body algorythm PointCount steps into future,
    /// and displays resulting orbits with linerenderers.
    /// </summary>
    [AddComponentMenu("SpaceGravity2D/PredictionSystem")]
    public class PredictionSystem : MonoBehaviour
    {
        /// <summary>
        /// Extrapolation body state data.
        /// </summary>
        private struct BodyPoints
        {
            public Vector3 pos;
            public Vector3 v;
            public float m;
            public bool isFixed;
            public bool isVisible;
            public Material material;
            public float width;
            public Vector3[] points;
        }

        /// <summary>
        /// Reference to scene's simulation control.
        /// </summary>
        public SimulationControl SimControl;
        
        /// <summary>
        /// Calculation step precision.
        /// Lower value - better precision.
        /// </summary>
        /// <remarks>
        /// Lower value increase precision, but decrease prediction range.
        /// Higher value decrease precision, and prediction range is increasing proportionaly.
        /// Can be balanced by PointsCount setting (Higher CalcStep - less PointsCount and vise versa).
        /// </remarks>
        public float CalcStep = 1f;

        /// <summary>
        /// Determines how many steps will be calculated into future.
        /// </summary>
        /// <remarks>
        /// If precision (The CalcStep) is low, pointsCount should be higher and vise versa.
        /// </remarks>
        public int PointsCount = 50;

        /// <summary>
        /// Global LineRenderer material.
        /// </summary>
        public Material LinesMaterial;

        /// <summary>
        /// Global LineRenderer width.
        /// </summary>
        public float LinesWidth = 0.05f;
        
        private BodyPoints[] bodies = new BodyPoints[0];

        private List<LineRenderer> lineRends = new List<LineRenderer>();

        private void Start()
        {
            if (SimControl == null)
            {
                SimControl = GameObject.FindObjectOfType<SimulationControl>();
            }
            if (SimControl == null)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (Mathd.Abs(SimControl.TimeScale) > 1e-6)
            {
                Calc();
            }
            ShowPredictOrbit();
        }

        private void OnDisable()
        {
            HideAllOrbits();
        }

        private void Calc()
        {
            // Filter disabled bodies:
            List<CelestialBody> targets = new List<CelestialBody>();
            for (int i = 0; i < SimControl.Bodies.Count; i++)
            {
                if (SimControl.Bodies[i].isActiveAndEnabled)
                {
                    targets.Add(SimControl.Bodies[i]);
                }
            }

            // Check is any body visible:
            if (!targets.Any(t =>
            {
                var targetComponent = t.GetComponent<PredictionSystemTarget>();
                return targetComponent == null || targetComponent.enabled;
            }))
            {
                for (int i = 0; i < lineRends.Count; i++)
                {
                    lineRends[i].enabled = false;
                }
                bodies = new BodyPoints[0];
                HideAllOrbits();
                // Don't calculate:
                return;
            }

            // Create working data array from bodies:
            bodies = new BodyPoints[targets.Count];
            for (int i = 0; i < bodies.Length; i++)
            {
                var targetComponent = targets[i].GetComponent<PredictionSystemTarget>();
                bool isVisibleOrb = targetComponent == null || targetComponent.enabled;
                Material mat = targetComponent == null ? LinesMaterial : targetComponent.OrbitMaterial;

                bodies[i] = new BodyPoints()
                {
                    pos = targets[i].transform.position,
                    v = (Vector3)targets[i].Velocity,
                    m = (float)targets[i].Mass,
                    isFixed = targets[i].IsFixedPosition,
                    isVisible = isVisibleOrb,
                    material = mat,
                    width = targetComponent == null ? LinesWidth : targetComponent.OrbitWidth,
                    points = new Vector3[PointsCount]
                };
            }

            // Calculate scene motion progress and record points into arrays:
            for (int i = 0; i < PointsCount; i++)
            {
                // Calculate next step velocities for each body:
                for (int j = 0; j < bodies.Length; j++)
                {
                    if (bodies[j].isFixed)
                    {
                        continue;
                    }
                    Vector3 acceleration = Vector3.zero;
                    for (int n = 0; n < bodies.Length; n++)
                    {
                        if (n != j)
                        {
                            acceleration += SpaceGravity2D.CelestialBodyUtils.AccelerationByAttractionForce(bodies[j].pos, bodies[n].pos, bodies[n].m * (float)SimControl.GravitationalConstant, 0.5f, (float)SimControl.MaxAttractionRange);
                        }
                    }
                    bodies[j].v += acceleration * CalcStep;
                }

                // Move bodies and store current step positions:
                for (int j = 0; j < bodies.Length; j++)
                {
                    if (bodies[j].isFixed)
                    {
                        continue;
                    }
                    bodies[j].points[i] = bodies[j].pos;
                    bodies[j].pos += bodies[j].v * CalcStep;
                }
            }
        }

        private void ShowPredictOrbit()
        {
            int t = 0;
            while (lineRends.Count < bodies.Length && t < 1000)
            {
                CreateLineRenderer();
                t++;
            }
            var i = 0;
            for (i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].isVisible)
                {
                    lineRends[i].positionCount = PointsCount + 1;
                    lineRends[i].startWidth = bodies[i].width;
                    lineRends[i].endWidth = bodies[i].width;
                    lineRends[i].material = bodies[i].material ?? LinesMaterial;
                    for (int j = 0; j < bodies[i].points.Length; j++)
                    {
                        lineRends[i].SetPosition(j, bodies[i].points[j]);
                    }
                    // Last point is not in array:
                    lineRends[i].SetPosition(PointsCount, bodies[i].pos);
                    lineRends[i].enabled = true;
                }
                else
                {
                    lineRends[i].enabled = false;
                }
            }
            for (; i < lineRends.Count; i++)
            {
                lineRends[i].enabled = false;
            }
        }

        private void CreateLineRenderer()
        {
            var obj = new GameObject("prediction orbit " + lineRends.Count);
            obj.transform.SetParent(transform);
            var lineRend = obj.AddComponent<LineRenderer>();
            lineRends.Add(lineRend);
        }

        public void HideAllOrbits()
        {
            for (int i = 0; i < lineRends.Count; i++)
            {
                lineRends[i].enabled = false;
            }
        }
    }
}
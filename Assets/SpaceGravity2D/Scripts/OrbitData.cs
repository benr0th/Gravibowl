using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SpaceGravity2D
{
    /// <summary>
    /// Orbit data container.
    /// Also contains methods for altering and updating orbit state.
    /// </summary>
    [Serializable]
    public class OrbitData
    {
        /// <summary>
        /// Minimal floating point value.
        /// </summary>
        [FormerlySerializedAs("epsilon")]
        public double Epsilon = 1e-010;

        /// <summary>
        /// Gravitational force coeficient.
        /// </summary>
        [FormerlySerializedAs("gravConst")]
        public double GravitationalConstant = 0.001;

        /// <summary>
        /// Normal vecotr of ecliptic plane.
        /// </summary>
        /// <remarks>
        /// Ecliptic plane is used for decoration,
        /// or when orbit is limited to two dimensions.
        /// </remarks>
        [FormerlySerializedAs("eclipticNormal")]
        public Vector3d EclipticNormal = new Vector3d(0, 0, 1);

        /// <summary>
        /// Perpendicular to EclipticNormal vector.
        /// Represents up direction on ecliptic plane.
        /// </summary>
        [FormerlySerializedAs("eclipticUp")]
        public Vector3d EclipticUp = new Vector3d(0, 1, 0);

        /// <summary>
        /// Current position of the body in local orbit space.
        /// </summary>
        [FormerlySerializedAs("position")]
        public Vector3d Position;
        
        /// <summary>
        /// Distance to attractor.
        /// </summary>
        [FormerlySerializedAs("attractorDistance")]
        public double AttractorDistance;

        /// <summary>
        /// Mass of the attractor.
        /// </summary>
        [FormerlySerializedAs("attractorMass")]
        public double AttractorMass;

        /// <summary>
        /// Current velocity direction and magnitude.
        /// </summary>
        [FormerlySerializedAs("velocity")]
        public Vector3d Velocity;

        /// <summary>
        /// Magnitude of semi minor axis of the orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("semiMinorAxis")]
        public double SemiMinorAxis;
        
        /// <summary>
        /// Magnitude of semi minor axis of the orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("SemiMajorAxis")]
        public double SemiMajorAxis;

        /// <summary>
        /// Focal parameter of orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("focalParameter")]
        public double FocalParameter;

        /// <summary>
        /// Eccentricity of orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("eccentricity")]
        public double Eccentricity;

        /// <summary>
        /// Total kinetic and potential energy of the orbit.
        /// </summary>
        [FormerlySerializedAs("energyTotal")]
        public double EnergyTotal;

        /// <summary>
        /// Period time of once cycle in seconds (if orbit is not hyperbolic).
        /// </summary>
        [FormerlySerializedAs("period")]
        public double Period;

        /// <summary>
        /// True anomaly in radians.
        /// </summary>
        [FormerlySerializedAs("trueAnomaly")]
        public double TrueAnomaly;

        /// <summary>
        /// Mean anomaly in radians.
        /// </summary>
        [FormerlySerializedAs("meanAnomaly")]
        public double MeanAnomaly;

        /// <summary>
        /// Eccentric anomaly in radians.
        /// </summary>
        [FormerlySerializedAs("eccentricAnomaly")]
        public double EccentricAnomaly;

        /// <summary>
        /// Square-constant parameter for orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("squaresConstant")]
        public double SquaresConstant;
        
        /// <summary>
        /// Periapsis point of the orbit.
        /// </summary>
        [FormerlySerializedAs("periapsis")]
        public Vector3d Periapsis;

        /// <summary>
        /// Distance to periapsis from the main focus point of orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("periapsisDistance")]
        public double PeriapsisDistance;

        /// <summary>
        /// Apoapsis point of the orbit. Not defined for hyperbolic orbits.
        /// </summary>
        [FormerlySerializedAs("apoapsis")]
        public Vector3d Apoapsis;

        /// <summary>
        /// Distance to apoapsis from the main focus point of orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("apoapsisDistance")]
        public double ApoapsisDistance;

        /// <summary>
        /// Position of center of orbit's elliptic curve relative to main focus point.
        /// </summary>
        [FormerlySerializedAs("centerPoint")]
        public Vector3d CenterPoint;

        /// <summary>
        /// Compression parameter of orbit's elliptic curve.
        /// </summary>
        [FormerlySerializedAs("orbitCompressionRatio")]
        public double OrbitCompressionRatio;

        /// <summary>
        /// Perpendicular vector to orbit's plane.
        /// </summary>
        [FormerlySerializedAs("orbitNormal")]
        public Vector3d OrbitNormal;

        /// <summary>
        /// Basis vector (direction) of Semi Minor Axis of the orbit.
        /// </summary>
        [FormerlySerializedAs("semiMinorAxisBasis")]
        public Vector3d SemiMinorAxisBasis;
        
        /// <summary>
        /// Basis vector (direction) of Semi Major Axis of the orbit.
        /// </summary>
        [FormerlySerializedAs("semiMajorAxisBasis")]
        public Vector3d SemiMajorAxisBasis;

        /// <summary>
        /// The orbit inclination in radians relative to ecliptic plane.
        /// </summary>
        [FormerlySerializedAs("inclination")]
        public double Inclination;

        /// <summary>
        /// if > 0, then orbit motion is clockwise.
        /// </summary>
        [FormerlySerializedAs("orbitNormalDotEclipticNormal")]
        public double OrbitNormalDotEclipticNormal;

        /// <summary>
        /// Is orbit state valid and error-free.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid orbit; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidOrbit
        {
            get
            {
                return Eccentricity >= 0 && Period > Epsilon && AttractorDistance > Epsilon && AttractorMass > Epsilon;
            }
        }

        /// <summary>
        /// Was orbit state changed without recalculating.
        /// </summary>
        public bool IsDirty = false;

        /// <summary>
        /// Calculates the full state of orbit from current body position, attractor position, attractor mass, velocity, and gravConstant.
        /// </summary>
        public void CalculateNewOrbitData()
        {
            IsDirty = false;
            var MG = AttractorMass * GravitationalConstant;
            AttractorDistance = Position.magnitude;
            var angularMomentumVector = CelestialBodyUtils.CrossProduct(Position, Velocity);
            OrbitNormal = angularMomentumVector.normalized;
            Vector3d eccVector;

            // Check if zero lenght.
            if (OrbitNormal.sqrMagnitude < 0.9 || OrbitNormal.sqrMagnitude > 1.1)
            {
                OrbitNormal = CelestialBodyUtils.CrossProduct(Position, EclipticUp).normalized;
                eccVector = new Vector3d();
            }
            else
            {
                eccVector = CelestialBodyUtils.CrossProduct(Velocity, angularMomentumVector) / MG - Position / AttractorDistance;
            }
            OrbitNormalDotEclipticNormal = CelestialBodyUtils.DotProduct(OrbitNormal, EclipticNormal);
            FocalParameter = angularMomentumVector.sqrMagnitude / MG;
            Eccentricity = eccVector.magnitude;
            EnergyTotal = Velocity.sqrMagnitude - 2 * MG / AttractorDistance;
            SemiMinorAxisBasis = CelestialBodyUtils.CrossProduct(angularMomentumVector, eccVector).normalized;
            if (SemiMinorAxisBasis.sqrMagnitude < 0.5)
            {
                SemiMinorAxisBasis = CelestialBodyUtils.CrossProduct(OrbitNormal, Position).normalized;
            }
            SemiMajorAxisBasis = CelestialBodyUtils.CrossProduct(OrbitNormal, SemiMinorAxisBasis).normalized;
            Inclination = Vector3d.Angle(OrbitNormal, EclipticNormal) * Mathd.Deg2Rad;
            if (Eccentricity < 1)
            {
                OrbitCompressionRatio = 1 - Eccentricity * Eccentricity;
                SemiMajorAxis = FocalParameter / OrbitCompressionRatio;
                SemiMinorAxis = SemiMajorAxis * Math.Sqrt(OrbitCompressionRatio);
                CenterPoint = -SemiMajorAxis * eccVector;
                Period = Mathd.PI_2 * Mathd.Sqrt(Mathd.Pow(SemiMajorAxis, 3) / MG);
                Apoapsis = CenterPoint + SemiMajorAxisBasis * SemiMajorAxis;
                Periapsis = CenterPoint - SemiMajorAxisBasis * SemiMajorAxis;
                PeriapsisDistance = Periapsis.magnitude;
                ApoapsisDistance = Apoapsis.magnitude;
                TrueAnomaly = Vector3d.Angle(Position, -SemiMajorAxisBasis) * Mathd.Deg2Rad;
                if (CelestialBodyUtils.DotProduct(CelestialBodyUtils.CrossProduct(Position, SemiMajorAxisBasis), OrbitNormal) < 0)
                {
                    TrueAnomaly = Mathd.PI_2 - TrueAnomaly;
                }
                EccentricAnomaly = CelestialBodyUtils.ConvertTrueToEccentricAnomaly(TrueAnomaly, Eccentricity);
                MeanAnomaly = EccentricAnomaly - Eccentricity * Math.Sin(EccentricAnomaly);
            }
            else
            {
                OrbitCompressionRatio = Eccentricity * Eccentricity - 1;
                SemiMajorAxis = FocalParameter / OrbitCompressionRatio;
                SemiMinorAxis = SemiMajorAxis * Math.Sqrt(OrbitCompressionRatio);
                CenterPoint = SemiMajorAxis * eccVector;
                Period = double.PositiveInfinity;
                Apoapsis = new Vector3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
                Periapsis = CenterPoint + SemiMajorAxisBasis * (SemiMajorAxis);
                PeriapsisDistance = Periapsis.magnitude;
                ApoapsisDistance = double.PositiveInfinity;
                TrueAnomaly = Vector3d.Angle(Position, eccVector) * Mathd.Deg2Rad;
                if (CelestialBodyUtils.DotProduct(CelestialBodyUtils.CrossProduct(Position, SemiMajorAxisBasis), OrbitNormal) < 0)
                {
                    TrueAnomaly = -TrueAnomaly;
                }
                EccentricAnomaly = CelestialBodyUtils.ConvertTrueToEccentricAnomaly(TrueAnomaly, Eccentricity);
                MeanAnomaly = Math.Sinh(EccentricAnomaly) * Eccentricity - EccentricAnomaly;
            }
        }

        /// <summary>
        /// Gets the velocity vector value at eccentric anomaly.
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly.</param>
        /// <returns>Velocity vector.</returns>
        public Vector3d GetVelocityAtEccentricAnomaly(double eccentricAnomaly)
        {
            return GetVelocityAtTrueAnomaly(CelestialBodyUtils.ConvertEccentricToTrueAnomaly(eccentricAnomaly, Eccentricity));
        }

        /// <summary>
        /// Gets the velocity value at true anomaly.
        /// </summary>
        /// <param name="trueAnomaly">The true anomaly.</param>
        /// <returns>Velocity vector.</returns>
        public Vector3d GetVelocityAtTrueAnomaly(double trueAnomaly)
        {
            if (FocalParameter < 1e-5)
            {
                return new Vector3d();
            }
            var sqrtMGdivP = Math.Sqrt(AttractorMass * GravitationalConstant / FocalParameter);
            double vX = sqrtMGdivP * (Eccentricity + Math.Cos(trueAnomaly));
            double vY = sqrtMGdivP * Math.Sin(trueAnomaly);
            return SemiMinorAxisBasis * vX + SemiMajorAxisBasis * vY;
        }

        /// <summary>
        /// Gets the central position at true anomaly.
        /// </summary>
        /// <param name="trueAnomaly">The true anomaly.</param>
        /// <returns>Position relative to orbit center.</returns>
        /// <remarks>
        /// Note: central position is not same as focal position.
        /// </remarks>
        public Vector3d GetCentralPositionAtTrueAnomaly(double trueAnomaly)
        {
            var ecc = CelestialBodyUtils.ConvertTrueToEccentricAnomaly(trueAnomaly, Eccentricity);
            return GetCentralPositionAtEccentricAnomaly(ecc);
        }

        /// <summary>
        /// Gets the central position at eccentric anomaly.
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly.</param>
        /// <returns>Position relative to orbit center.</returns>
        /// <remarks>
        /// Note: central position is not same as focal position.
        /// </remarks>
        public Vector3d GetCentralPositionAtEccentricAnomaly(double eccentricAnomaly)
        {
            Vector3d result = Eccentricity < 1 ?
                new Vector3d(Math.Sin(eccentricAnomaly) * SemiMinorAxis, -Math.Cos(eccentricAnomaly) * SemiMajorAxis) :
                new Vector3d(Math.Sinh(eccentricAnomaly) * SemiMinorAxis, Math.Cosh(eccentricAnomaly) * SemiMajorAxis);
            return SemiMinorAxisBasis * result.x + SemiMajorAxisBasis * result.y;
        }

        /// <summary>
        /// Gets the focal position at eccentric anomaly.
        /// </summary>
        /// <param name="eccentricAnomaly">The eccentric anomaly.</param>
        /// <returns>Position relative to attractor (focus).</returns>
        public Vector3d GetFocalPositionAtEccentricAnomaly(double eccentricAnomaly)
        {
            return GetCentralPositionAtEccentricAnomaly(eccentricAnomaly) + CenterPoint;
        }

        /// <summary>
        /// Gets the focal position at true anomaly.
        /// </summary>
        /// <param name="trueAnomaly">The true anomaly.</param>
        /// <returns>Position relative to attractor (focus).</returns>
        public Vector3d GetFocalPositionAtTrueAnomaly(double trueAnomaly)
        {
            return GetCentralPositionAtTrueAnomaly(trueAnomaly) + CenterPoint;
        }

        /// <summary>
        /// Gets the central position.
        /// </summary>
        /// <returns>Position relative to orbit center.</returns>
        /// <remarks>
        /// Note: central position is not same as focal position.
        /// </remarks>
        public Vector3d GetCentralPosition()
        {
            return Position - CenterPoint;
        }

        /// <summary>
        /// Get orbit curve points if current orbit state is valid.
        /// </summary>
        /// <param name="pointsCount">Max points count in curve.</param>
        /// <param name="maxDistance">Max distance for points in curve.</param>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.", error: false)]
        public Vector3d[] GetOrbitPoints(int pointsCount = 50, double maxDistance = 1000d)
        {
            var points = new Vector3d[0];
            GetOrbitPointsNoAlloc(ref points, pointsCount, new Vector3d(), maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit curve points if current orbit state is valid.
        /// </summary>
        /// <param name="pointsCount">Max points count in curve.</param>
        /// <param name="origin">World position of attractor (focus of orbit).</param>
        /// <param name="maxDistance">Max distance for points in curve.</param>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.", error: false)]
        public Vector3d[] GetOrbitPoints(int pointsCount, Vector3d origin, double maxDistance = 1000d)
        {
            var points = new Vector3d[0];
            GetOrbitPointsNoAlloc(ref points, pointsCount, origin, maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit curve points without array allocation, if current orbit state is valid.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// </remarks>
        /// <param name="points">Resulting orbit curve array.</param>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
        public void GetOrbitPointsNoAlloc(ref Vector3d[] points, int pointsCount = 50, double maxDistance = 1000d)
        {
            GetOrbitPointsNoAlloc(ref points, pointsCount, new Vector3d(), maxDistance);
        }

        /// <summary>
        /// Get orbit curve points without array allocation, if current orbit state is valid.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// </remarks>
        /// <param name="points">Resulting orbit curve array.</param>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="origin">World position of attractor (focus of orbit).</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
		public void GetOrbitPointsNoAlloc(ref Vector3d[] points, int pointsCount, Vector3d origin, double maxDistance = 1000d)
        {
            if (pointsCount < 2)
            {
                points = new Vector3d[0];
                return;
            }
            if (Eccentricity < 1)
            {
                if (points == null || points.Length != pointsCount)
                {
                    points = new Vector3d[pointsCount];
                }

                if (ApoapsisDistance < maxDistance)
                {
                    for (var i = 0; i < pointsCount; i++)
                    {
                        points[i] = GetFocalPositionAtEccentricAnomaly(i * Mathd.PI_2 / (pointsCount - 1d)) + origin;
                    }
                }
                else
                {
                    var maxAngle = CelestialBodyUtils.CalcTrueAnomalyForDistance(maxDistance, Eccentricity, SemiMajorAxis);
                    for (int i = 0; i < pointsCount; i++)
                    {
                        points[i] = GetFocalPositionAtTrueAnomaly(-maxAngle + i * 2d * maxAngle / (pointsCount - 1)) + origin;
                    }
                }
            }
            else
            {
                if (maxDistance < PeriapsisDistance)
                {
                    points = new Vector3d[0];
                    return;
                }

                if (points == null || points.Length != pointsCount)
                {
                    points = new Vector3d[pointsCount];
                }

                var maxAngle = CelestialBodyUtils.CalcTrueAnomalyForDistance(maxDistance, Eccentricity, SemiMajorAxis);
                for (int i = 0; i < pointsCount; i++)
                {
                    points[i] = GetFocalPositionAtTrueAnomaly(-maxAngle + i * 2d * maxAngle / (pointsCount - 1)) + origin;
                }
            }
        }

        /// <summary>
        /// Get orbit curve points if current orbit state is valid.
        /// </summary>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.")]
        public Vector3[] GetOrbitPoints(int pointsCount = 50, float maxDistance = 1000f)
        {
            var points = new Vector3[0];
            GetOrbitPointsNoAlloc(ref points, pointsCount, new Vector3(), maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit curve points if current orbit state is valid.
        /// </summary>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="origin">World position of attractor (focus of orbit).</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
        /// <returns>Orbit curve points array.</returns>
        [Obsolete("Use GetOrbitPointsNoAlloc instead.")]
        public Vector3[] GetOrbitPoints(int pointsCount, Vector3 origin, float maxDistance = 1000f)
        {
            var points = new Vector3[0];
            GetOrbitPointsNoAlloc(ref points, pointsCount, origin, maxDistance);
            return points;
        }

        /// <summary>
        /// Get orbit curve points without array allocation, if current orbit state is valid.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// </remarks>
        /// <param name="points">Resulting orbit curve array.</param>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
        public void GetOrbitPointsNoAlloc(ref Vector3[] points, int pointsCount = 50, float maxDistance = 1000f)
        {
            GetOrbitPointsNoAlloc(ref points, pointsCount, new Vector3(), maxDistance);
        }

        /// <summary>
        /// Get orbit curve points without array allocation, if current orbit state is valid.
        /// </summary>
        /// <remarks>
        /// Note: array allocation may sometimes occur, if specified array is null or lenght is not equal to target points count.
        /// </remarks>
        /// <param name="points">Resulting orbit curve array.</param>
        /// <param name="pointsCount">Max orbit curve points count.</param>
        /// <param name="origin">World position of attractor (focus of orbit).</param>
        /// <param name="maxDistance">Max distance for orbit curve points.</param>
        public void GetOrbitPointsNoAlloc(ref Vector3[] points, int pointsCount, Vector3 origin, float maxDistance = 1000f)
        {
            if (pointsCount < 2)
            {
                points = new Vector3[0];
                return;
            }
            if (Eccentricity < 1)
            {
                if (points == null || points.Length != pointsCount)
                {
                    points = new Vector3[pointsCount];
                }
                if (ApoapsisDistance < maxDistance)
                {
                    for (var i = 0; i < pointsCount; i++)
                    {
                        points[i] = (Vector3)GetFocalPositionAtEccentricAnomaly(i * Mathd.PI_2 / (pointsCount - 1d)) + origin;
                    }
                }
                else
                {
                    var maxAngle = CelestialBodyUtils.CalcTrueAnomalyForDistance(maxDistance, Eccentricity, SemiMajorAxis);
                    for (int i = 0; i < pointsCount; i++)
                    {
                        points[i] = (Vector3)GetFocalPositionAtTrueAnomaly(-maxAngle + i * 2d * maxAngle / (pointsCount - 1)) + origin;
                    }
                }
            }
            else
            {
                if (maxDistance < PeriapsisDistance)
                {
                    points = new Vector3[0];
                    return;
                }
                if (points == null || points.Length != pointsCount)
                {
                    points = new Vector3[pointsCount];
                }
                var maxAngle = CelestialBodyUtils.CalcTrueAnomalyForDistance(maxDistance, Eccentricity, SemiMajorAxis);

                for (int i = 0; i < pointsCount; i++)
                {
                    points[i] = (Vector3)GetFocalPositionAtTrueAnomaly(-maxAngle + i * 2d * maxAngle / (pointsCount - 1)) + origin;
                }
            }
        }

        /// <summary>
        /// Gets the ascending node of orbit.
        /// </summary>
        /// <param name="asc">The asc.</param>
        /// <returns><c>true</c> if ascending node exists, otherwise <c>false</c></returns>
        public bool GetAscendingNode(out Vector3 asc)
        {
            Vector3d v;
            if (GetAscendingNode(out v))
            {
                asc = (Vector3)v;
                return true;
            }
            asc = new Vector3();
            return false;
        }

        /// <summary>
        /// Gets the ascending node of orbit.
        /// </summary>
        /// <param name="asc">The asc.</param>
        /// <returns><c>true</c> if ascending node exists, otherwise <c>false</c></returns>
        public bool GetAscendingNode(out Vector3d asc)
        {
            var norm = CelestialBodyUtils.CrossProduct(OrbitNormal, EclipticNormal);
            var s = CelestialBodyUtils.DotProduct(CelestialBodyUtils.CrossProduct(norm, SemiMajorAxisBasis), OrbitNormal) < 0;
            var ecc = 0d;
            var trueAnom = Vector3d.Angle(norm, CenterPoint) * Mathd.Deg2Rad;
            if (Eccentricity < 1)
            {
                var cosT = Math.Cos(trueAnom);
                ecc = Math.Acos((Eccentricity + cosT) / (1d + Eccentricity * cosT));
                if (!s)
                {
                    ecc = Mathd.PI_2 - ecc;
                }
            }
            else
            {
                trueAnom = Vector3d.Angle(-norm, CenterPoint) * Mathd.Deg2Rad;
                if (trueAnom >= Mathd.Acos(-1d / Eccentricity))
                {
                    asc = new Vector3d();
                    return false;
                }
                var cosT = Math.Cos(trueAnom);
                ecc = CelestialBodyUtils.Acosh((Eccentricity + cosT) / (1 + Eccentricity * cosT)) * (!s ? -1 : 1);
            }
            asc = GetFocalPositionAtEccentricAnomaly(ecc);
            return true;
        }

        /// <summary>
        /// Gets the descending node of orbit.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns><c>true</c> if descending node exists, otherwise <c>false</c></returns>
        public bool GetDescendingNode(out Vector3 desc)
        {
            Vector3d v;
            if (GetDescendingNode(out v))
            {
                desc = (Vector3)v;
                return true;
            }
            desc = new Vector3();
            return false;
        }

        /// <summary>
        /// Gets the descending node of orbit.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns><c>true</c> if descending node exists, otherwise <c>false</c></returns>
        public bool GetDescendingNode(out Vector3d desc)
        {
            var norm = CelestialBodyUtils.CrossProduct(OrbitNormal, EclipticNormal);
            var s = CelestialBodyUtils.DotProduct(CelestialBodyUtils.CrossProduct(norm, SemiMajorAxisBasis), OrbitNormal) < 0;
            var ecc = 0d;
            var trueAnom = Vector3d.Angle(norm, -CenterPoint) * Mathd.Deg2Rad;
            if (Eccentricity < 1)
            {
                var cosT = Math.Cos(trueAnom);
                ecc = Math.Acos((Eccentricity + cosT) / (1d + Eccentricity * cosT));
                if (s)
                {
                    ecc = Mathd.PI_2 - ecc;
                }
            }
            else
            {
                trueAnom = Vector3d.Angle(norm, CenterPoint) * Mathd.Deg2Rad;
                if (trueAnom >= Mathd.Acos(-1d / Eccentricity))
                {
                    desc = new Vector3d();
                    return false;
                }
                var cosT = Math.Cos(trueAnom);
                ecc = CelestialBodyUtils.Acosh((Eccentricity + cosT) / (1 + Eccentricity * cosT)) * (s ? -1 : 1);
            }
            desc = GetFocalPositionAtEccentricAnomaly(ecc);
            return true;
        }

        /// <summary>
        /// Updates the kepler orbit state by defined deltatime.
        /// Orbit main parameters will remains unchanged, but all anomalies will progress in time.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        public void UpdateOrbitDataByTime(double deltaTime)
        {
            UpdateOrbitAnomaliesByTime(deltaTime);
            SetPositionByCurrentAnomaly();
            SetVelocityByCurrentAnomaly();
        }

        /// <summary>
        /// Updates the value of orbital anomalies by defined deltatime.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        /// <remarks>
        /// Only anomalies values will be changed. 
        /// Position and velocity states needs to be updated too after this method call.
        /// </remarks>
        public void UpdateOrbitAnomaliesByTime(double deltaTime)
        {
            if (Eccentricity < 1)
            {
                if (Period > 1e-5)
                {
                    MeanAnomaly += Mathd.PI_2 * deltaTime / Period;
                }
                MeanAnomaly %= Mathd.PI_2;
                if (MeanAnomaly < 0)
                {
                    MeanAnomaly = Mathd.PI_2 - MeanAnomaly;
                }
                EccentricAnomaly = CelestialBodyUtils.KeplerSolver(MeanAnomaly, Eccentricity);
                var cosE = Math.Cos(EccentricAnomaly);
                TrueAnomaly = Math.Acos((cosE - Eccentricity) / (1 - Eccentricity * cosE));
                if (MeanAnomaly > Mathd.PI)
                {
                    TrueAnomaly = Mathd.PI_2 - TrueAnomaly;
                }
                if (double.IsNaN(MeanAnomaly) || double.IsInfinity(MeanAnomaly))
                {
                    Debug.Log("SpaceGravity2D: NaN(INF) MEAN ANOMALY"); //litle paranoya
                    Debug.Break();
                }
                if (double.IsNaN(EccentricAnomaly) || double.IsInfinity(EccentricAnomaly))
                {
                    Debug.Log("SpaceGravity2D: NaN(INF) ECC ANOMALY");
                    Debug.Break();
                }
                if (double.IsNaN(TrueAnomaly) || double.IsInfinity(TrueAnomaly))
                {
                    Debug.Log("SpaceGravity2D: NaN(INF) TRUE ANOMALY");
                    Debug.Break();
                }
            }
            else
            {
                double n = Math.Sqrt(AttractorMass * GravitationalConstant / Math.Pow(SemiMajorAxis, 3));
                MeanAnomaly = MeanAnomaly + n * deltaTime;
                EccentricAnomaly = CelestialBodyUtils.KeplerSolverHyperbolicCase(MeanAnomaly, Eccentricity);
                TrueAnomaly = Math.Atan2(Math.Sqrt(Eccentricity * Eccentricity - 1.0) * Math.Sinh(EccentricAnomaly), Eccentricity - Math.Cosh(EccentricAnomaly));
            }
        }

        /// <summary>
        /// Updates position from eccentric anomaly state.
        /// </summary>
        public void SetPositionByCurrentAnomaly()
        {
            Position = GetFocalPositionAtEccentricAnomaly(EccentricAnomaly);
        }

        /// <summary>
        /// Sets velocity by current eccentric anomaly.
        /// </summary>
        public void SetVelocityByCurrentAnomaly()
        {
            Velocity = GetVelocityAtEccentricAnomaly(EccentricAnomaly);
        }

        /// <summary>
        /// Sets the eccentricity and updates all corresponding orbit state values.
        /// </summary>
        /// <param name="e">The new eccentricity value.</param>
        /// <remarks>
        /// Mean anomaly will try to preserve.
        /// </remarks>
        public void SetEccentricity(double e)
        {
            if (!IsValidOrbit)
            {
                return;
            }
            e = Mathd.Abs(e);
            var _periapsis = PeriapsisDistance; // Periapsis remains constant
            Eccentricity = e;
            var compresion = Eccentricity < 1 ? (1 - Eccentricity * Eccentricity) : (Eccentricity * Eccentricity - 1);
            SemiMajorAxis = Math.Abs(_periapsis / (1 - Eccentricity));
            FocalParameter = SemiMajorAxis * compresion;
            SemiMinorAxis = SemiMajorAxis * Mathd.Sqrt(compresion);
            CenterPoint = SemiMajorAxis * Math.Abs(Eccentricity) * SemiMajorAxisBasis;
            if (Eccentricity < 1)
            {
                EccentricAnomaly = CelestialBodyUtils.KeplerSolver(MeanAnomaly, Eccentricity);
                var cosE = Math.Cos(EccentricAnomaly);
                TrueAnomaly = Math.Acos((cosE - Eccentricity) / (1 - Eccentricity * cosE));
                if (MeanAnomaly > Mathd.PI)
                {
                    TrueAnomaly = Mathd.PI_2 - TrueAnomaly;
                }
            }
            else
            {
                EccentricAnomaly = CelestialBodyUtils.KeplerSolverHyperbolicCase(MeanAnomaly, Eccentricity);
                TrueAnomaly = Math.Atan2(Math.Sqrt(Eccentricity * Eccentricity - 1) * Math.Sinh(EccentricAnomaly), Eccentricity - Math.Cosh(EccentricAnomaly));
            }
            SetVelocityByCurrentAnomaly();
            SetPositionByCurrentAnomaly();

            CalculateNewOrbitData();
        }

        /// <summary>
        /// Sets the mean anomaly and updates all other anomalies.
        /// </summary>
        /// <param name="m">The m.</param>
        public void SetMeanAnomaly(double m)
        {
            if (!IsValidOrbit)
            {
                return;
            }
            MeanAnomaly = m % Mathd.PI_2;
            if (Eccentricity < 1)
            {
                if (MeanAnomaly < 0)
                {
                    MeanAnomaly += Mathd.PI_2;
                }
                EccentricAnomaly = CelestialBodyUtils.KeplerSolver(MeanAnomaly, Eccentricity);
                TrueAnomaly = CelestialBodyUtils.ConvertEccentricToTrueAnomaly(EccentricAnomaly, Eccentricity);
            }
            else
            {
                EccentricAnomaly = CelestialBodyUtils.KeplerSolverHyperbolicCase(MeanAnomaly, Eccentricity);
                TrueAnomaly = CelestialBodyUtils.ConvertEccentricToTrueAnomaly(EccentricAnomaly, Eccentricity);
            }
            SetPositionByCurrentAnomaly();
            SetVelocityByCurrentAnomaly();
        }

        /// <summary>
        /// Sets the true anomaly and updates all other anomalies.
        /// </summary>
        /// <param name="t">The t.</param>
        public void SetTrueAnomaly(double t)
        {
            if (!IsValidOrbit)
            {
                return;
            }
            t %= Mathd.PI_2;

            if (Eccentricity < 1)
            {
                if (t < 0)
                {
                    t += Mathd.PI_2;
                }
                EccentricAnomaly = CelestialBodyUtils.ConvertTrueToEccentricAnomaly(t, Eccentricity);
                MeanAnomaly = EccentricAnomaly - Eccentricity * Math.Sin(EccentricAnomaly);
            }
            else
            {
                EccentricAnomaly = CelestialBodyUtils.ConvertTrueToEccentricAnomaly(t, Eccentricity);
                MeanAnomaly = Math.Sinh(EccentricAnomaly) * Eccentricity - EccentricAnomaly;
            }
            SetPositionByCurrentAnomaly();
            SetVelocityByCurrentAnomaly();
        }

        /// <summary>
        /// Sets the eccentric anomaly and updates all other anomalies.
        /// </summary>
        /// <param name="e">The e.</param>
        public void SetEccentricAnomaly(double e)
        {
            if (!IsValidOrbit)
            {
                return;
            }
            e %= Mathd.PI_2;
            EccentricAnomaly = e;
            if (Eccentricity < 1)
            {
                if (e < 0)
                {
                    e = Mathd.PI_2 + e;
                }
                EccentricAnomaly = e;
                TrueAnomaly = CelestialBodyUtils.ConvertEccentricToTrueAnomaly(e, Eccentricity);
                MeanAnomaly = EccentricAnomaly - Eccentricity * Math.Sin(EccentricAnomaly);
            }
            else
            {
                TrueAnomaly = CelestialBodyUtils.ConvertEccentricToTrueAnomaly(e, Eccentricity);
                MeanAnomaly = Math.Sinh(EccentricAnomaly) * Eccentricity - EccentricAnomaly;
            }
            SetPositionByCurrentAnomaly();
            SetVelocityByCurrentAnomaly();
        }

        /// <summary>
        /// Rotates the relative position and velocity by same quaternion.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        public void RotateOrbit(Quaternion rotation)
        {
            Position = new Vector3d(rotation * ((Vector3)Position));
            Velocity = new Vector3d(rotation * ((Vector3)Velocity));
            CalculateNewOrbitData();
        }
    }
}

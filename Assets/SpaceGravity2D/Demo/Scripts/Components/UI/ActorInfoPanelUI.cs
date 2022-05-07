using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// UI panel controller for Actor Info window.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ActorInfoPanelUI : MonoBehaviour
    {
        public Toggle FollowToggle;
        public Text NameText;
        public Text AttractorNameText;
        public FloatValueControlUI VelocityValue;
        public FloatValueControlUI RelVelocityValue;
        public Text MassText;
        public Slider MassSlider;
        public float MaxMass = 10000;
        public Toggle IsKeplerMotionToggle;
        public Toggle IsDrawOrbitToggle;
        public Text InfoEccentricityText;
        public Text InfoPeriodText;
        public Text InfoDistanceText;
        public Text InfoPeriapsisText;
        public Text InfoMeanAnomalyText;

        private bool _isRefreshing;

        private CelestialBody _currentTarget;

        private void Start()
        {
            DemoSceneManager.Instance.OnActorSelectionChangedEvent += OnSelectActor;
            InitializeControls();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            RefreshView();
        }

        private void InitializeControls()
        {
            FollowToggle.onValueChanged.AddListener((b) =>
            {
                if (!_isRefreshing)
                {
                    CameraMovement.Instance.FollowTarget(b ? _currentTarget : null);
                    RefreshView();
                }
            });
            VelocityValue.OnValueChangedEvent += (f) =>
              {
                  if (!_isRefreshing)
                  {
                      if (_currentTarget != null)
                      {
                          if (_currentTarget.Velocity.sqrMagnitude > 1e-4f)
                          {
                              _currentTarget.AddExternalVelocity(_currentTarget.Velocity.normalized * (f - _currentTarget.Velocity.magnitude));
                              RefreshView();
                          }
                      }
                  }
              };
            RelVelocityValue.OnValueChangedEvent += (f) =>
            {
                if (!_isRefreshing)
                {
                    if (_currentTarget != null)
                    {
                        if (_currentTarget.Velocity.sqrMagnitude > 1e-4f)
                        {
                            _currentTarget.AddExternalVelocity(_currentTarget.Velocity.normalized * (f - _currentTarget.RelativeVelocity.magnitude));
                            RefreshView();
                        }
                    }
                }
            };
            MassSlider.minValue = 1;
            MassSlider.maxValue = MaxMass;
            MassSlider.onValueChanged.AddListener((f) =>
            {
                if (!_isRefreshing)
                {
                    if (_currentTarget != null)
                    {
                        _currentTarget.Mass = f;
                        RefreshView();
                    }
                }
            });
            IsKeplerMotionToggle.onValueChanged.AddListener((b) =>
            {
                if (!_isRefreshing)
                {
                    if (_currentTarget != null)
                    {
                        _currentTarget.UseKeplerMotion = b;
                        var drawOrbits = DemoSceneManager.Instance.IsShowOrbits;
                        DemoSceneManager.Instance.IsShowOrbits = drawOrbits;
                        RefreshView();
                    }
                }
            });
            IsDrawOrbitToggle.onValueChanged.AddListener((b) =>
            {
                if (!_isRefreshing)
                {
                    if (_currentTarget != null)
                    {
                        var actor = ActorsManager.Instance.FindCreatedActor(_currentTarget);
                        if (actor != null)
                        {
                            actor.OrbitDisplayRef.enabled = true;
                            if (actor.CelestialBodyRef.UseKeplerMotion)
                            {
                                if (actor.OrbitDisplayRef.LineRenderer != null)
                                {
                                    actor.OrbitDisplayRef.LineRenderer.enabled = b;
                                }
                                actor.PredictionDisplayRef.enabled = false;
                            }
                            else
                            {
                                if (actor.OrbitDisplayRef.LineRenderer != null)
                                {
                                    actor.OrbitDisplayRef.LineRenderer.enabled = false;
                                }
                                actor.PredictionDisplayRef.enabled = b;
                            }
                        }
                    }
                }
            });
        }

        private void OnSelectActor(ActorData actor)
        {
            if (actor != null && actor.CelestialBodyRef != null)
            {
                _currentTarget = actor.CelestialBodyRef;
            }
            else
            {
                _currentTarget = null;
            }
            RefreshView();
        }

        private void RefreshView()
        {
            _isRefreshing = true;
            gameObject.SetActive(_currentTarget != null);
            if (_currentTarget != null)
            {
                FollowToggle.isOn = IsFollowingCurrentTarget();
                NameText.text = _currentTarget.name;
                AttractorNameText.text = _currentTarget.AttractorRef == null ? "[null]" : _currentTarget.AttractorRef.name;
                VelocityValue.Value = (float)_currentTarget.Velocity.magnitude;
                RelVelocityValue.Value = (float)_currentTarget.RelativeVelocity.magnitude;
                MassText.text = _currentTarget.Mass.ToString(_currentTarget.Mass > 10 ? "0" : "0.00");
                MassSlider.value = (float)_currentTarget.Mass;
                IsKeplerMotionToggle.isOn = _currentTarget.UseKeplerMotion;
                IsDrawOrbitToggle.isOn = _currentTarget.GetComponent<OrbitDisplay>().isActiveAndEnabled;
                if (_currentTarget.OrbitData.IsValidOrbit)
                {
                    InfoEccentricityText.text = "Eccentricity      = " + _currentTarget.OrbitData.Eccentricity.ToString("0.00");
                    InfoPeriodText.text = "Period               = " + _currentTarget.OrbitData.Period.ToString("0.00");
                    InfoDistanceText.text = "Distance           = " + _currentTarget.OrbitData.Position.magnitude.ToString("0.00");
                    InfoPeriapsisText.text = "Periapsis          = " + _currentTarget.OrbitData.PeriapsisDistance.ToString("0.00");
                    InfoMeanAnomalyText.text = "Mean anomaly = " + (_currentTarget.OrbitData.MeanAnomaly * Mathd.Rad2Deg).ToString("0.00");
                }
                else
                {
                    InfoEccentricityText.text = "Eccentricity      = 0";
                    InfoPeriodText.text = "Period               = 0";
                    InfoDistanceText.text = "Distance           = 0";
                    InfoPeriapsisText.text = "Periapsis          = 0";
                    InfoMeanAnomalyText.text = "Mean anomaly = 0";
                }
            }
            _isRefreshing = false;
        }

        private bool IsFollowingCurrentTarget()
        {
            if (CameraMovement.Instance.transform.parent != null)
            {
                if (_currentTarget != null)
                {
                    return object.ReferenceEquals(CameraMovement.Instance.transform.parent, _currentTarget.transform);
                }
            }
            return false;
        }
    }
}
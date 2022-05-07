using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for displaying some float value in UI with ability to increase or decrease this value in some specified range.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class FloatValueControlUI : MonoBehaviour
    {
        [Header("Main parameters")]
        public float MinLimit;
        public float MaxLimit = float.PositiveInfinity;
        public string ValueFormat = "0.00";

        [Header("Main References")]
        public GameObject PlusButton;
        public GameObject MinusButton;
        public Text ValueOutputText;

        [Header("Secondary parameters")]
        public float SlowChangingTreshold = 10f;
        public float NullifyTreshold = 0.5f;

        [Range(1.01f, 2f)]
        public float IncreaseMultiplier = 1.1f;
        [Range(0.1f, 0.99f)]
        public float DecreaseMultiplier = 0.9f;
        public float LinearChangeSpeed = 0.1f;

        public event Action<float> OnValueChangedEvent;

        private bool _isPlusPressed;
        private bool _isMinusPressed;

        private float _value;

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueOutputText.text = value.ToString(ValueFormat);
                    if (OnValueChangedEvent != null)
                    {
                        OnValueChangedEvent(_value);
                    }
                }
            }
        }

        private void Awake()
        {
            ValueOutputText.text = _value.ToString(ValueFormat);
        }

        private void Update()
        {
            if (_isPlusPressed)
            {
                if (Value < MaxLimit)
                {
                    if (Value < SlowChangingTreshold)
                    {
                        Value += LinearChangeSpeed;
                    }
                    else
                    {
                        Value *= IncreaseMultiplier;
                    }
                }
                else
                {
                    Value = MaxLimit;
                }
            }
            else if (_isMinusPressed)
            {
                if (Value > MinLimit)
                {
                    if (Value < NullifyTreshold)
                    {
                        Value = 0;
                    }
                    else
                    if (Value < SlowChangingTreshold)
                    {
                        Value -= LinearChangeSpeed;
                    }
                    else
                    {
                        Value *= DecreaseMultiplier;
                    }
                }
                else
                {
                    Value = MinLimit;
                }
            }
        }

        private void Start()
        {
            RegisterPlusButtonControl();
            RegisterMinusButtonControl();
        }

        private void RegisterPlusButtonControl()
        {
            var plusTrigger = PlusButton.AddComponent<EventTrigger>();

            var pressEntry = new EventTrigger.Entry();
            pressEntry.eventID = EventTriggerType.PointerDown;
            pressEntry.callback.AddListener((evData) => OnPlusPressed());

            var releaseEntry = new EventTrigger.Entry();
            releaseEntry.eventID = EventTriggerType.PointerUp;
            releaseEntry.callback.AddListener((evData) => OnPlusReleased());

            var exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((evData) => OnPlusReleased());

            plusTrigger.triggers.Add(pressEntry);
            plusTrigger.triggers.Add(releaseEntry);
            plusTrigger.triggers.Add(exitEntry);
        }

        private void RegisterMinusButtonControl()
        {
            var minusTrigger = MinusButton.AddComponent<EventTrigger>();

            var pressEntry = new EventTrigger.Entry();
            pressEntry.eventID = EventTriggerType.PointerDown;
            pressEntry.callback.AddListener((evData) => OnMinusPressed());

            var releaseEntry = new EventTrigger.Entry();
            releaseEntry.eventID = EventTriggerType.PointerUp;
            releaseEntry.callback.AddListener((evData) => OnMinusReleased());

            var exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((evData) => OnMinusReleased());

            minusTrigger.triggers.Add(pressEntry);
            minusTrigger.triggers.Add(releaseEntry);
            minusTrigger.triggers.Add(exitEntry);
        }

        private void OnPlusPressed()
        {
            _isPlusPressed = true;
        }

        private void OnPlusReleased()
        {
            _isPlusPressed = false;
        }

        private void OnMinusPressed()
        {
            _isMinusPressed = true;
        }

        private void OnMinusReleased()
        {
            _isMinusPressed = false;
        }

    }
}
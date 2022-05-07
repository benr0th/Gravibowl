using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SpaceGravity2D.Demo
{
    public class InputProvider : MonoBehaviour
    {
        private class InputState
        {
            public bool IsPressed = false;
            public Vector2 LastPos = new Vector2();
            public int TapsCount = 0;
            public float LastPressedTime = 0;
            public float LastReleasedTime = 0;
        }

        private const int InputsCount = 3;
        public float ClickTime = 0.2f;

        public static InputProvider Instance;

        public delegate void PointerInputDelegate(Vector2 pointerScreenPosition, Vector2 lastPointerPosition, int buttonIndex);

        public event PointerInputDelegate OnPointerDownEvent;
        public event PointerInputDelegate OnPointerStayDownEvent;
        public event PointerInputDelegate OnPointerUpEvent;
        public event PointerInputDelegate OnClickEvent;
        public event Action<float> OnScrollEvent;


        private InputState[] InputStates = new InputState[] { new InputState(), new InputState(), new InputState() };

        private Dictionary<KeyCode, List<Action>> KeyDownInputListenners = new Dictionary<KeyCode, List<Action>>();
        private Dictionary<KeyCode, List<Action>> KeyUpInputListenners = new Dictionary<KeyCode, List<Action>>();
        private Dictionary<KeyCode, List<Action>> KeyStayDownInputListenners = new Dictionary<KeyCode, List<Action>>();

        public InputProvider()
        {
            Instance = this;
        }

        private void Update()
        {
            MouseInputHandler();
            KeyboardInputHandler();
        }
        public void RegisterKeyDownListenner(KeyCode key, Action callback)
        {
            if (callback != null)
            {
                if (!KeyDownInputListenners.ContainsKey(key))
                {
                    KeyDownInputListenners[key] = new List<Action>();
                }
                KeyDownInputListenners[key].Add(callback);
            }
        }

        public void RegisterKeyUpListenner(KeyCode key, Action callback)
        {
            if (callback != null)
            {
                if (!KeyUpInputListenners.ContainsKey(key))
                {
                    KeyUpInputListenners[key] = new List<Action>();
                }
                KeyUpInputListenners[key].Add(callback);
            }
        }

        public void RegisterKeyStayDownListenner(KeyCode key, Action callback)
        {
            if (callback != null)
            {
                if (!KeyStayDownInputListenners.ContainsKey(key))
                {
                    KeyStayDownInputListenners[key] = new List<Action>();
                }
                KeyStayDownInputListenners[key].Add(callback);
            }
        }

        private void KeyboardInputHandler()
        {
            foreach (var item in KeyDownInputListenners)
            {
                if (Input.GetKeyDown(item.Key))
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        item.Value[i]();
                    }
                }
            }
            foreach (var item in KeyUpInputListenners)
            {
                if (Input.GetKeyUp(item.Key))
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        item.Value[i]();
                    }
                }
            }
            foreach (var item in KeyStayDownInputListenners)
            {
                if (Input.GetKey(item.Key))
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        item.Value[i]();
                    }
                }
            }
        }

        private void MouseInputHandler()
        {
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            for (int i = 0; i < InputsCount; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    if (!overUI)
                    {
                        if (InputStates[i].IsPressed)
                        {
                            RaiseEvent(OnPointerUpEvent, Input.mousePosition, InputStates[i].LastPos, i);
                        }
                        InputStates[i].IsPressed = true;
                        InputStates[i].LastPos = Input.mousePosition;
                        InputStates[i].LastPressedTime = Time.time;
                        RaiseEvent(OnPointerDownEvent, InputStates[i].LastPos, InputStates[i].LastPos, i);
                        if (InputStates[i].TapsCount > 0 && InputStates[i].LastPressedTime - InputStates[i].LastReleasedTime >= ClickTime)
                        {
                            InputStates[i].TapsCount = 0;
                        }
                    }
                }
                else
                    if (Input.GetMouseButton(i))
                {
                    if (InputStates[i].IsPressed)
                    {
                        RaiseEvent(OnPointerStayDownEvent, Input.mousePosition, InputStates[i].LastPos, i);
                        InputStates[i].LastPos = Input.mousePosition;
                    }
                }
                if (Input.GetMouseButtonUp(i))
                {
                    if (InputStates[i].IsPressed)
                    {
                        RaiseEvent(OnPointerUpEvent, Input.mousePosition, InputStates[i].LastPos, i);
                        InputStates[i].LastPos = Input.mousePosition;
                        InputStates[i].IsPressed = false;
                        if (Time.time - InputStates[i].LastPressedTime < ClickTime)
                        {
                            InputStates[i].TapsCount++;
                            if (InputStates[i].TapsCount == 1)
                            {
                                RaiseEvent(OnClickEvent, Input.mousePosition, InputStates[i].LastPos, i);
                            }
                        }
                        InputStates[i].LastReleasedTime = Time.time;
                    }
                }
            }
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 1e-5f && OnScrollEvent != null)
            {
                OnScrollEvent(scroll);
            }
        }

        private void RaiseEvent(PointerInputDelegate handler, Vector2 pos, Vector2 lastPos, int button)
        {
            if (handler != null)
            {
                handler(pos, lastPos, button);
            }
        }
    }
}
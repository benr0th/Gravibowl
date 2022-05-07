using UnityEngine;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for making camera controlable by input.
    /// Should be attached to the root of camera hierarchy.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class CameraMovement : MonoBehaviour
    {
        public float RotationSpeedX = 1f;
        public float RotationSpeedY = 1f;
        public float RotationSpeedByKeyboard = 1f;
        public float MovingSpeed = 1f;
        public float ForsageSpeed = 5f;
        private bool _isForsageActive = false;

        public static CameraMovement Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            InputProvider.Instance.OnPointerDownEvent += OnMouseDown;
            InputProvider.Instance.OnPointerUpEvent += OnMouseUp;
            InputProvider.Instance.OnPointerStayDownEvent += OnMouseStay;
            InputProvider.Instance.OnScrollEvent += OnMouseScroll;

            InputProvider.Instance.RegisterKeyDownListenner(KeyCode.LeftShift, () => this._isForsageActive = true);
            InputProvider.Instance.RegisterKeyUpListenner(KeyCode.LeftShift, () => this._isForsageActive = false);

            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.W, MoveFormard);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.A, MoveLeft);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.S, MoveBackward);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.D, MoveRight);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.Q, RotateLeft);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.E, RotateRight);


            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.Space, MoveUp);
            InputProvider.Instance.RegisterKeyStayDownListenner(KeyCode.LeftControl, MoveDown);

            InputProvider.Instance.RegisterKeyDownListenner(KeyCode.F, ToggleFollowSelected);

            ActorsManager.Instance.OnBeforeDestroyActorEvent += (actor) =>
              {
                  if (transform.parent == actor.TransformRef)
                  {
                      FollowTarget(null);
                  }
              };
        }

        private void RotateRight()
        {
            transform.Rotate(xAngle: 0, yAngle: 0, zAngle: -RotationSpeedByKeyboard * 60 * Time.deltaTime, relativeTo: Space.Self);
        }

        private void RotateLeft()
        {
            transform.Rotate(xAngle: 0, yAngle: 0, zAngle: RotationSpeedByKeyboard * 60 * Time.deltaTime, relativeTo: Space.Self);
        }

        private void MoveUp()
        {
            transform.localPosition += transform.up * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void MoveDown()
        {
            transform.localPosition -= transform.up * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void MoveFormard()
        {
            transform.localPosition += transform.forward * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void MoveBackward()
        {
            transform.localPosition -= transform.forward * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void MoveRight()
        {
            transform.localPosition += transform.right * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void MoveLeft()
        {
            transform.localPosition -= transform.right * 60 * Time.deltaTime * (_isForsageActive ? ForsageSpeed : MovingSpeed);
        }

        private void OnMouseDown(Vector2 pos, Vector2 lastPos, int btn)
        {
        }

        private void OnMouseStay(Vector2 pos, Vector2 lastPos, int btn)
        {
            if (btn == 1)
            {
                if (pos != lastPos)
                {
                    var rotationX = (pos.x - lastPos.x) * RotationSpeedX;
                    var rotationY = (pos.y - lastPos.y) * RotationSpeedY;
                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
                    transform.localRotation = transform.localRotation * xQuaternion * yQuaternion;
                }
            }
        }

        private void OnMouseUp(Vector2 pos, Vector2 lastPos, int btn)
        {
        }

        private void OnMouseScroll(float x)
        {
        }

        public void FollowTarget(CelestialBody target)
        {
            if (target != null)
            {
                transform.SetParent(target.transform);
            }
            else
            {
                transform.SetParent(null);
            }
        }

        private void ToggleFollowSelected()
        {
            if (transform.parent == null)
            {
                if (DemoSceneManager.Instance.CurrentSelected != null)
                {
                    FollowTarget(DemoSceneManager.Instance.CurrentSelected.CelestialBodyRef);
                }
            }
            else
            {
                if (DemoSceneManager.Instance.CurrentSelected != null)
                {
                    if (DemoSceneManager.Instance.CurrentSelected.CelestialBodyRef.transform == transform.parent)
                    {
                        FollowTarget(null);
                    }
                    else
                    {
                        FollowTarget(DemoSceneManager.Instance.CurrentSelected.CelestialBodyRef);
                    }
                }
                else
                {
                    FollowTarget(null);
                }
            }
        }
    }
}
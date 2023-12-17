using Game.Managers;
using UnityEngine;

namespace Game.Core
{
    public class MovementHandler : MonoBehaviour
    {
        [Header("Control Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private float minigameSpeed;
        [SerializeField] private float sensitivity;
        [SerializeField] private float slippageSensitivity;
        [SerializeField] private float slippageCorrectionSpeed;
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private Vector3 rotationAxis;
        [SerializeField] private float rotationLimit;

        [Space] [Header("Components")]
        [SerializeField] private Transform gunObject;
        [SerializeField] private Transform maxBound;
        [SerializeField] private Transform minBound;

        private GameState _gameState = GameState.Start;
        private PlayerController _player;
        private Vector2 firstMousePos;
        private Vector2 secondMousePos;
        private float _speed;
        private float xPosition;
        private float xSlippage;
        private bool clicked;

        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += SetState;
        }

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= SetState;
        }

        private void Update()
        {
            if (_player.IsTrapped) return;

            if (_gameState == GameState.Running ||
                _gameState == GameState.MinigameRunning)
            {
                ForwardMovement();
                HorizontalMovement();
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(PlayerController player) => _player = player;

        #endregion

        #region PRIVATE METHODS

        private void ForwardMovement()
        {
            transform.Translate(moveDirection * (_speed * Time.deltaTime), Space.World);
        }

        private void HorizontalMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get mouse first position
                firstMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                xPosition = transform.position.x;
                clicked = true;
            }
            else if (Input.GetMouseButton(0) && clicked)
            {
                // Get mouse second position and calculate difference
                secondMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var difference = (secondMousePos - firstMousePos) * sensitivity;

                // Add and clamp X position
                xPosition += difference.x;
                xPosition = Mathf.Clamp(xPosition, minBound.position.x, maxBound.position.x);

                // Define target pos
                var targetPos = new Vector3(xPosition, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, turnSpeed * Time.deltaTime);
                // transform.Translate(targetPos - transform.position, Space.World);  //For harder input feeling

                // Give smooth rotation to gun model according to swerve direction
                xSlippage += slippageSensitivity * (secondMousePos - firstMousePos).x;
                xSlippage = Mathf.Clamp(xSlippage, -rotationLimit, rotationLimit);
                gunObject.eulerAngles = rotationAxis * xSlippage;

                firstMousePos = secondMousePos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                clicked = false;
            }

            // Smoothly correct rotation of gun model
            xSlippage = Mathf.Lerp(xSlippage, 0, slippageCorrectionSpeed);
            gunObject.eulerAngles = rotationAxis * xSlippage;
        }

        private void SetState(GameState state)
        {
            _gameState = state;
            _speed = state == GameState.MinigameRunning ? minigameSpeed : moveSpeed;
        }

        #endregion
    }
}
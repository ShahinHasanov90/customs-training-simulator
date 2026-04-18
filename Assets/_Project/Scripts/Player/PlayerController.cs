using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomsSim.Player
{
    /// <summary>First-person controller tuned for walking around an inspection hall.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float sprintSpeed = 5.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private Transform head;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float pitchLimit = 80f;

        private CharacterController _cc;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private Vector3 _velocity;
        private float _pitch;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            _lookAction = new InputAction("Look", binding: "<Mouse>/delta");
            _sprintAction = new InputAction("Sprint", binding: "<Keyboard>/leftShift");

            _moveAction.Enable();
            _lookAction.Enable();
            _sprintAction.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            _moveAction?.Disable();
            _lookAction?.Disable();
            _sprintAction?.Disable();
        }

        private void Update()
        {
            HandleLook();
            HandleMove();
        }

        private void HandleLook()
        {
            if (head == null) return;
            var delta = _lookAction.ReadValue<Vector2>() * lookSensitivity;
            transform.Rotate(0f, delta.x, 0f);
            _pitch = Mathf.Clamp(_pitch - delta.y, -pitchLimit, pitchLimit);
            head.localEulerAngles = new Vector3(_pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            var input = _moveAction.ReadValue<Vector2>();
            var speed = _sprintAction.IsPressed() ? sprintSpeed : walkSpeed;
            var move = transform.right * input.x + transform.forward * input.y;
            _cc.Move(move * speed * Time.deltaTime);

            if (_cc.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -1f;
            }

            _velocity.y += gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);
        }
    }
}

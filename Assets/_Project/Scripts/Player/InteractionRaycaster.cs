using CustomsSim.Core;
using CustomsSim.Inspection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomsSim.Player
{
    /// <summary>Shoots a ray from the camera each frame; surfaces the hovered interactable.</summary>
    public sealed class InteractionRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera sourceCamera;
        [SerializeField] private float maxDistance = 2.5f;
        [SerializeField] private LayerMask mask = ~0;

        private IInteractable _current;
        private InputAction _useAction;

        public IInteractable Current => _current;

        private void Awake()
        {
            if (sourceCamera == null) sourceCamera = Camera.main;
        }

        private void OnEnable()
        {
            _useAction = new InputAction("Use", binding: "<Keyboard>/e");
            _useAction.performed += OnUsePerformed;
            _useAction.Enable();
        }

        private void OnDisable()
        {
            if (_useAction != null)
            {
                _useAction.performed -= OnUsePerformed;
                _useAction.Disable();
                _useAction.Dispose();
                _useAction = null;
            }
        }

        private void Update()
        {
            if (sourceCamera == null) return;
            var ray = new Ray(sourceCamera.transform.position, sourceCamera.transform.forward);
            IInteractable hovered = null;
            if (Physics.Raycast(ray, out var hit, maxDistance, mask))
            {
                hovered = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (!ReferenceEquals(hovered, _current))
            {
                _current = hovered;
                EventBus.Raise(new InteractableHoverChanged(_current));
            }
        }

        private void OnUsePerformed(InputAction.CallbackContext ctx)
        {
            _current?.Interact();
        }
    }

    public readonly struct InteractableHoverChanged
    {
        public readonly IInteractable Target;
        public InteractableHoverChanged(IInteractable target) => Target = target;
    }
}

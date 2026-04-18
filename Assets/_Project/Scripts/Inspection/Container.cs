using System.Collections.Generic;
using CustomsSim.Core;
using UnityEngine;

namespace CustomsSim.Inspection
{
    /// <summary>A cargo container on the primary inspection lane.</summary>
    public sealed class Container : MonoBehaviour, IInteractable
    {
        [SerializeField] private string containerId = "CONT-0000";
        [SerializeField] private Seal seal;
        [SerializeField] private Document declaration;
        [SerializeField] private List<CargoItem> trueCargo = new();
        [SerializeField] private bool hasFalseCompartment;

        public string ContainerId => containerId;
        public Seal Seal => seal;
        public Document Declaration => declaration;
        public IReadOnlyList<CargoItem> TrueCargo => trueCargo;
        public bool HasFalseCompartment => hasFalseCompartment;
        public bool IsOpen { get; private set; }

        public string DisplayName => $"Container {containerId}";
        public bool CanInteract => !IsOpen;

        public void Interact()
        {
            TryOpen();
        }

        public bool TryOpen()
        {
            if (IsOpen) return false;
            if (seal != null && !seal.IsBroken)
            {
                EventBus.Raise(new ContainerBlockedBySeal(this));
                return false;
            }

            IsOpen = true;
            EventBus.Raise(new ContainerOpened(this));
            return true;
        }
    }

    [System.Serializable]
    public struct CargoItem
    {
        public string description;
        public string hsCode;
        public float declaredUnitValue;
        public int quantity;
    }

    public readonly struct ContainerOpened
    {
        public readonly Container Container;
        public ContainerOpened(Container container) => Container = container;
    }

    public readonly struct ContainerBlockedBySeal
    {
        public readonly Container Container;
        public ContainerBlockedBySeal(Container container) => Container = container;
    }
}

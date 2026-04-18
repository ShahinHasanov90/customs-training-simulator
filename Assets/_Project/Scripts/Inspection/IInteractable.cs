namespace CustomsSim.Inspection
{
    /// <summary>Anything the player can target with the interaction raycaster.</summary>
    public interface IInteractable
    {
        string DisplayName { get; }
        bool CanInteract { get; }
        void Interact();
    }
}

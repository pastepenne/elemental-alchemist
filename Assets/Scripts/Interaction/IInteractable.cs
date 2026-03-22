namespace ElementalAlchemist.Interaction
{
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact();
    }
}
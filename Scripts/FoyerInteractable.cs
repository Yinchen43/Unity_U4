using UnityEngine;

public enum FoyerInteractionType
{
    ElevatorButton,
    FloorSign,
    Hamburger
}

public class FoyerInteractable :
    MonoBehaviour,
    IInteractable
{
    [SerializeField]
    private FoyerInteractionType interactionType;

    [SerializeField]
    private FoyerSequenceManager sequenceManager;

    [SerializeField]
    private GameObject focusVisual;

    public string InteractionPrompt
    {
        get
        {
            if (sequenceManager == null)
            {
                return "Interact";
            }

            return sequenceManager.GetPrompt(
                interactionType
            );
        }
    }

    public bool CanInteract
    {
        get
        {
            return sequenceManager != null &&
                   sequenceManager.CanInteract(
                       interactionType
                   );
        }
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        sequenceManager.PerformInteraction(
            interactionType
        );
    }

    public void SetFocused(bool focused)
    {
        if (focusVisual != null)
        {
            focusVisual.SetActive(
                focused && CanInteract
            );
        }
    }
}
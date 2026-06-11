using UnityEngine;

public enum KitchenInteractionType
{
    Pillow,
    Baguette,
    Toaster,
    FrontDoor
}

public class KitchenInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Interaction")]
    [SerializeField]
    private KitchenInteractionType interactionType;

    [SerializeField]
    private KitchenSequenceManager sequenceManager;

    [Header("Focus Feedback")]
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
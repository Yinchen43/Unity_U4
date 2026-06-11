using UnityEngine;

public class BedSleepInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Interaction")]
    [SerializeField]
    private string interactionPrompt =
        "Sleep for five more minutes";

    [SerializeField]
    private string nextSceneName = "Bedroom2";

    [Header("References")]
    [SerializeField]
    private SleepConfirmationUI confirmationUI;

    [SerializeField]
    private GameObject focusVisual;

    public string InteractionPrompt =>
        interactionPrompt;

    public bool CanInteract =>
        confirmationUI != null;

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        confirmationUI.Open(nextSceneName);
    }

    public void SetFocused(bool focused)
    {
        if (focusVisual != null)
        {
            focusVisual.SetActive(focused);
        }
    }
}
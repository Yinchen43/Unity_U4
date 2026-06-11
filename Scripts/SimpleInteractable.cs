using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string interactionPrompt = "Interact";
    [SerializeField] private bool useOnlyOnce;

    [Header("Narration")]
    [SerializeField] private NarratorBeat[] narratorSequence;

    [Header("Focus Feedback")]
    [SerializeField] private GameObject focusVisual;

    [Header("Optional Additional Action")]
    [SerializeField] private UnityEvent onInteract;

    private bool hasBeenUsed;

    public string InteractionPrompt => interactionPrompt;

    public bool CanInteract =>
        !useOnlyOnce || !hasBeenUsed;

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (NarratorManager.Instance != null &&
            narratorSequence != null &&
            narratorSequence.Length > 0)
        {
            NarratorManager.Instance.PlayBeatSequence(
                narratorSequence
            );
        }

        onInteract?.Invoke();

        if (useOnlyOnce)
        {
            hasBeenUsed = true;
            SetFocused(false);
        }
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
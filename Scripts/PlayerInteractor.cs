using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("UI")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TextMeshProUGUI promptText;

    private IInteractable focusedInteractable;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    private void Start()
    {
        ClearFocus();
    }

    private void Update()
    {
        if (NarratorManager.Instance != null &&
            NarratorManager.Instance.IsSpeaking)
        {
            ClearFocus();
            return;
        }

        FindInteractable();

        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool ePressed =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        if (focusedInteractable != null &&
            focusedInteractable.CanInteract &&
            (mousePressed || ePressed))
        {
            focusedInteractable.Interact();
            ClearFocus();
        }
    }

    private void FindInteractable()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactableLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            IInteractable interactable =
                hit.collider
                   .GetComponentInParent<IInteractable>();

            if (interactable != null &&
                interactable.CanInteract)
            {
                SetFocus(interactable);
                return;
            }
        }

        ClearFocus();
    }

    private void SetFocus(IInteractable interactable)
    {
        if (ReferenceEquals(
            focusedInteractable,
            interactable))
        {
            UpdatePrompt();
            return;
        }

        focusedInteractable?.SetFocused(false);

        focusedInteractable = interactable;
        focusedInteractable.SetFocused(true);

        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (focusedInteractable == null)
        {
            promptObject.SetActive(false);
            return;
        }

        promptText.text =
            focusedInteractable.InteractionPrompt;

        promptObject.SetActive(true);
    }

    private void ClearFocus()
    {
        focusedInteractable?.SetFocused(false);
        focusedInteractable = null;

        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ClearFocus();
    }
}
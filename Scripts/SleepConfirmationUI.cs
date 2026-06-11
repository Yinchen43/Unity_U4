using UnityEngine;
using UnityEngine.UI;

public class SleepConfirmationUI : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject crosshair;

    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;

    private string targetSceneName;
    private bool isTransitioning;

    private void Awake()
    {
        // Hide the popup when the scene begins.
        popupRoot.SetActive(false);

        // The buttons call these methods automatically.
        yesButton.onClick.AddListener(ConfirmSleep);
        noButton.onClick.AddListener(CancelSleep);
    }

    private void OnDestroy()
    {
        yesButton.onClick.RemoveListener(ConfirmSleep);
        noButton.onClick.RemoveListener(CancelSleep);
    }

    public void Open(string nextSceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        targetSceneName = nextSceneName;

        // Stop player movement and interaction.
        playerController.SetControlsEnabled(false);
        playerInteractor.enabled = false;

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        // Show the UI and release the mouse cursor.
        popupRoot.SetActive(true);
        playerController.UnlockCursor();

        yesButton.interactable = true;
        noButton.interactable = true;
    }

    private void CancelSleep()
    {
        if (isTransitioning)
        {
            return;
        }

        popupRoot.SetActive(false);

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }

        playerInteractor.enabled = true;
        playerController.SetControlsEnabled(true);
        playerController.LockCursor();
    }

    private void ConfirmSleep()
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        yesButton.interactable = false;
        noButton.interactable = false;

        playerController.LockCursor();

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "No SceneLoader exists in this scene."
            );

            isTransitioning = false;
            return;
        }

        SceneLoader.Instance.LoadScene(targetSceneName);
    }
}
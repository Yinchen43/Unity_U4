using System.Collections;
using UnityEngine;

public class NormalEndingManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private GameObject crosshair;

    [Header("Ending UI")]
    [SerializeField] private GameObject endingLogo;

    [Header("Timing")]
    [SerializeField] private float delayAfterFade = 0.4f;
    [SerializeField] private float logoDisplayDuration = 4f;
    [SerializeField] private float delayAfterLogo = 0.3f;

    [Header("Narration Before Black Screen")]
    [SerializeField] private NarratorBeat[] computerReactionLines;

    [Header("Narration On Black Screen")]
    [SerializeField] private NarratorBeat[] finalLines;

    [Header("Menu Scene")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private bool endingStarted;

    private void Awake()
    {
        if (endingLogo != null)
        {
            endingLogo.SetActive(false);
        }
    }

    public void BeginEnding()
    {
        if (endingStarted)
        {
            return;
        }

        endingStarted = true;
        StartCoroutine(EndingRoutine());
    }

    private IEnumerator EndingRoutine()
    {
        // Prevent the player from moving or interacting again.
        if (playerController != null)
        {
            playerController.SetControlsEnabled(false);
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = false;
        }

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        // First reaction while the computer is still visible.
        yield return PlayNarrationAndWait(
            computerReactionLines
        );

        // Fade the scene completely to black.
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeToBlack();
        }

        if (delayAfterFade > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delayAfterFade
            );
        }

        // Logo appears above the black FadePanel.
        if (endingLogo != null)
        {
            endingLogo.SetActive(true);
        }

        // Final Narrator line while the logo is displayed.
        yield return PlayNarrationAndWait(finalLines);

        if (logoDisplayDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                logoDisplayDuration
            );
        }

        if (endingLogo != null)
        {
            endingLogo.SetActive(false);
        }

        if (delayAfterLogo > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delayAfterLogo
            );
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "No SceneLoader exists in the NormalEnd scene."
            );

            yield break;
        }

        SceneLoader.Instance.LoadScene(menuSceneName);
    }

    private IEnumerator PlayNarrationAndWait(
        NarratorBeat[] sequence)
    {
        if (sequence == null ||
            sequence.Length == 0 ||
            NarratorManager.Instance == null)
        {
            yield break;
        }

        while (NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }

        bool started =
            NarratorManager.Instance.PlayBeatSequence(
                sequence
            );

        if (!started)
        {
            yield break;
        }

        while (NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }
    }
}
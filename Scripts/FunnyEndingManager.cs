using System.Collections;
using UnityEngine;

public class FunnyEndingManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private GameObject crosshair;

    [Header("Objects That Appear")]
    [SerializeField] private GameObject alarmClockObject;
    [SerializeField] private GameObject toasterObject;
    [SerializeField] private GameObject liftObject;
    [SerializeField] private GameObject slowNPCObject;

    [Header("Ending UI")]
    [SerializeField] private GameObject endingLogo;

    [Header("Ending Music")]
    [SerializeField] private AudioSource endingMusic;
    [SerializeField] private float musicVolume = 0.18f;
    [SerializeField] private float musicFadeInDuration = 2f;
    [SerializeField] private float musicFadeOutDuration = 1.5f;

    [Header("Timing")]
    [SerializeField] private float objectAppearanceDelay = 0.3f;
    [SerializeField] private float delayAfterFade = 0.5f;
    [SerializeField] private float logoDisplayDuration = 4f;
    [SerializeField] private float delayAfterLogo = 0.3f;

    [Header("Narration - Beginning")]
    [SerializeField] private NarratorBeat[] openingLines;

    [Header("Narration - Alarm Clock")]
    [SerializeField] private NarratorBeat[] alarmClockLines;

    [Header("Narration - Toaster")]
    [SerializeField] private NarratorBeat[] toasterLines;

    [Header("Narration - Lift")]
    [SerializeField] private NarratorBeat[] liftLines;

    [Header("Narration - Slow Individual")]
    [SerializeField] private NarratorBeat[] slowNPCLines;

    [Header("Narration - Reflection")]
    [SerializeField] private NarratorBeat[] reflectionLines;

    [Header("Narration - Black Screen")]
    [SerializeField] private NarratorBeat[] blackScreenLines;

    [Header("Narration - Logo")]
    [SerializeField] private NarratorBeat[] logoLines;

    [Header("Menu Scene")]
    [SerializeField] private string menuSceneName = "MainMenu";

    private bool endingStarted;

    private void Awake()
    {
        SetObjectActive(alarmClockObject, false);
        SetObjectActive(toasterObject, false);
        SetObjectActive(liftObject, false);
        SetObjectActive(slowNPCObject, false);
        SetObjectActive(endingLogo, false);

        if (endingMusic != null)
        {
            endingMusic.playOnAwake = false;
            endingMusic.loop = true;
            endingMusic.volume = 0f;
            endingMusic.Stop();
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
        DisableMovementButAllowLooking();

        if (endingMusic != null)
        {
            StartCoroutine(FadeMusicIn());
        }

        yield return PlayNarrationAndWait(openingLines);

        yield return RevealObjectAndSpeak(
            alarmClockObject,
            alarmClockLines
        );

        yield return RevealObjectAndSpeak(
            toasterObject,
            toasterLines
        );

        yield return RevealObjectAndSpeak(
            liftObject,
            liftLines
        );

        yield return RevealObjectAndSpeak(
            slowNPCObject,
            slowNPCLines
        );

        yield return PlayNarrationAndWait(reflectionLines);

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

        yield return PlayNarrationAndWait(
            blackScreenLines
        );

        if (endingLogo != null)
        {
            endingLogo.SetActive(true);
        }

        yield return PlayNarrationAndWait(
            logoLines
        );

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

        if (endingMusic != null)
        {
            yield return FadeMusicOut();
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "No SceneLoader exists in FunnyEnd."
            );

            yield break;
        }

        SceneLoader.Instance.LoadScene(
            menuSceneName
        );
    }

    private IEnumerator RevealObjectAndSpeak(
        GameObject objectToReveal,
        NarratorBeat[] narration)
    {
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true);
        }

        if (objectAppearanceDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                objectAppearanceDelay
            );
        }

        yield return PlayNarrationAndWait(
            narration
        );
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

    private IEnumerator FadeMusicIn()
    {
        endingMusic.volume = 0f;
        endingMusic.loop = true;
        endingMusic.Play();

        if (musicFadeInDuration <= 0f)
        {
            endingMusic.volume = musicVolume;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < musicFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / musicFadeInDuration
            );

            endingMusic.volume = Mathf.Lerp(
                0f,
                musicVolume,
                progress
            );

            yield return null;
        }

        endingMusic.volume = musicVolume;
    }

    private IEnumerator FadeMusicOut()
    {
        float startingVolume =
            endingMusic.volume;

        if (musicFadeOutDuration <= 0f)
        {
            endingMusic.volume = 0f;
            endingMusic.Stop();
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < musicFadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / musicFadeOutDuration
            );

            endingMusic.volume = Mathf.Lerp(
                startingVolume,
                0f,
                progress
            );

            yield return null;
        }

        endingMusic.volume = 0f;
        endingMusic.Stop();
    }

    private void DisableMovementButAllowLooking()
    {
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
            playerController.SetLookEnabled(true);
            playerController.LockCursor();
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = false;
        }

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }
    }

    private static void SetObjectActive(
        GameObject target,
        bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}
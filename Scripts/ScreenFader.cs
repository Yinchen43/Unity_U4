using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.6f;

    public bool IsFading { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup =
                GetComponent<CanvasGroup>();
        }

        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator Start()
    {
        yield return FadeFromBlack();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public IEnumerator FadeToBlack()
    {
        yield return Fade(1f);
    }

    public IEnumerator FadeFromBlack()
    {
        yield return Fade(0f);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        IsFading = true;

        float startingAlpha =
            fadeCanvasGroup.alpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(elapsed / fadeDuration);

            fadeCanvasGroup.alpha =
                Mathf.Lerp(
                    startingAlpha,
                    targetAlpha,
                    progress
                );

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        IsFading = false;
    }
}
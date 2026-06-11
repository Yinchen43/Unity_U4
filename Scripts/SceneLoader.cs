using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private bool isLoading;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(
                LoadSceneRoutine(sceneName)
            );
        }
    }

    public void ReloadCurrentScene()
    {
        string currentScene =
            SceneManager.GetActiveScene().name;

        LoadScene(currentScene);
    }

    private IEnumerator LoadSceneRoutine(
        string sceneName)
    {
        isLoading = true;

        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance
                .FadeToBlack();
        }

        AsyncOperation loadingOperation =
            SceneManager.LoadSceneAsync(sceneName);

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}
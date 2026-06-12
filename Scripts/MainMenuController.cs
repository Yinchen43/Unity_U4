using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "Bedroom1";

    private bool gameStarting;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        if (gameStarting)
        {
            return;
        }

        gameStarting = true;

        Debug.Log("Play button pressed.");

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "SceneLoader is missing from the MainMenu scene."
            );

            gameStarting = false;
            return;
        }

        SceneLoader.Instance.LoadScene(firstSceneName);
    }
}
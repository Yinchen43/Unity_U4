using System.Collections;
using UnityEngine;

public enum BuildingTriggerType
{
    Arrival,
    FrontDoor,
    BackDoor
}

public class BuildingEndingTrigger : MonoBehaviour
{
    [Header("Trigger Type")]
    [SerializeField]
    private BuildingTriggerType triggerType;

    [Header("Arrival Narration")]
    [SerializeField]
    private NarratorBeat[] arrivalNarration;

    [Header("Ending Scenes")]
    [SerializeField]
    private string normalEndingScene = "NormalEnd";

    [SerializeField]
    private string funnyEndingScene = "FunnyEnd";

    private bool hasTriggered;
    private bool isLoading;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        switch (triggerType)
        {
            case BuildingTriggerType.Arrival:
                TriggerArrivalNarration();
                break;

            case BuildingTriggerType.FrontDoor:
                LoadEnding(normalEndingScene);
                break;

            case BuildingTriggerType.BackDoor:
                LoadEnding(funnyEndingScene);
                break;
        }
    }

    private void TriggerArrivalNarration()
    {
        if (hasTriggered)
        {
            return;
        }

        hasTriggered = true;
        StartCoroutine(PlayArrivalNarration());
    }

    private IEnumerator PlayArrivalNarration()
    {
        if (NarratorManager.Instance == null ||
            arrivalNarration == null ||
            arrivalNarration.Length == 0)
        {
            yield break;
        }

        while (NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }

        NarratorManager.Instance.PlayBeatSequence(
            arrivalNarration
        );
    }

    private void LoadEnding(string sceneName)
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "No SceneLoader exists in the Street scene."
            );

            isLoading = false;
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }
}
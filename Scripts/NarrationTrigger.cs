using System.Collections;
using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    [Header("Trigger Behaviour")]
    [SerializeField] private bool playOnSceneStart;
    [SerializeField] private bool playOnlyOnce = true;

    [Header("Narrator Lines")]
    [SerializeField] private NarratorLine[] lines;

    private bool hasPlayed;

    private IEnumerator Start()
    {
        if (playOnSceneStart)
        {
            yield return null;
            BeginNarration();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BeginNarration();
        }
    }

    public void BeginNarration()
    {
        if (playOnlyOnce && hasPlayed)
        {
            return;
        }

        hasPlayed = true;
        StartCoroutine(WaitThenPlay());
    }

    private IEnumerator WaitThenPlay()
    {
        while (NarratorManager.Instance == null ||
               NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }

        NarratorManager.Instance.PlaySequence(lines);
    }
}
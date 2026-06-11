using System.Collections;
using UnityEngine;

public class BedroomScene3Manager : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float doorOpenAngle = 90f;
    [SerializeField] private float doorOpenDuration = 0.8f;

    [Header("Outfit Objects")]
    [SerializeField] private GameObject sensibleOutfitObject;
    [SerializeField] private GameObject formalOutfitObject;
    [SerializeField] private GameObject ridiculousOutfitObject;

    [Header("Outfit Colliders")]
    [SerializeField] private Collider[] outfitColliders;

    [Header("Outfit Narration")]
    [SerializeField] private NarratorLine sensibleOutfitLine;
    [SerializeField] private NarratorLine formalOutfitLine;
    [SerializeField] private NarratorLine ridiculousOutfitLine;

    [Header("Next Instruction")]
    [SerializeField] private NarratorLine goToKitchenLine;

    private bool outfitChosen;

    public void ChooseSensibleOutfit()
    {
        ChooseOutfit(
            sensibleOutfitLine,
            sensibleOutfitObject
        );
    }

    public void ChooseFormalOutfit()
    {
        ChooseOutfit(
            formalOutfitLine,
            formalOutfitObject
        );
    }

    public void ChooseRidiculousOutfit()
    {
        ChooseOutfit(
            ridiculousOutfitLine,
            ridiculousOutfitObject
        );
    }

    private void ChooseOutfit(
        NarratorLine selectedLine,
        GameObject selectedOutfit)
    {
        if (outfitChosen)
        {
            return;
        }

        outfitChosen = true;

        // Prevent all three outfits from being selected again.
        foreach (Collider outfitCollider in outfitColliders)
        {
            if (outfitCollider != null)
            {
                outfitCollider.enabled = false;
            }
        }

        // The selected outfit disappears to imply it was worn.
        if (selectedOutfit != null)
        {
            selectedOutfit.SetActive(false);
        }

        StartCoroutine(
            OutfitSequenceRoutine(selectedLine)
        );
    }

    private IEnumerator OutfitSequenceRoutine(
        NarratorLine selectedLine)
    {
        yield return PlayNarratorLine(selectedLine);
        yield return PlayNarratorLine(goToKitchenLine);
        yield return OpenDoorRoutine();
    }

    private IEnumerator PlayNarratorLine(
        NarratorLine line)
    {
        if (line == null ||
            string.IsNullOrWhiteSpace(line.subtitle) ||
            NarratorManager.Instance == null)
        {
            yield break;
        }

        NarratorManager.Instance.PlayLine(line);

        while (NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (doorPivot == null)
        {
            Debug.LogError(
                "Bedroom door pivot has not been assigned."
            );

            yield break;
        }

        Quaternion startingRotation =
            doorPivot.localRotation;

        Quaternion targetRotation =
            startingRotation *
            Quaternion.Euler(
                0f,
                0f,
                -doorOpenAngle
            );

        float elapsed = 0f;

        while (elapsed < doorOpenDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / doorOpenDuration
            );

            doorPivot.localRotation =
                Quaternion.Slerp(
                    startingRotation,
                    targetRotation,
                    progress
                );

            yield return null;
        }

        doorPivot.localRotation = targetRotation;
    }
}
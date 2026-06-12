using System.Collections;
using TMPro;
using UnityEngine;

public class FoyerSequenceManager : MonoBehaviour
{
    private enum FoyerState
    {
        WaitingForButton,
        ElevatorCounting,
        WaitingForSign,
        ElevatorOpen,
        NeedsFood,
        ReadyToDescend,
        LoadingNextScene
    }

    [Header("Elevator")]
    [SerializeField] private TMP_Text floorDisplay;
    [SerializeField] private GameObject elevatorDoor;
    [SerializeField] private AudioSource dingAudio;

    [Header("Floor Sign Planes")]
    [SerializeField] private GameObject floor100Plane;
    [SerializeField] private GameObject floor99Plane;

    [Header("Food")]
    [SerializeField] private GameObject foodTable;
    [SerializeField] private GameObject hamburgerObject;

    [Header("Floor Speed")]
    [SerializeField] private float normalFloorDelay = 0.35f;
    [SerializeField] private float fastFloorDelay = 0.035f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName =
        "Stairwell";

    [Header("Narration - Button Pressed")]
    [SerializeField]
    private NarratorBeat[] buttonPressedLines;

    [Header("Narration - Lift Going Too High")]
    [SerializeField]
    private NarratorBeat[] fastRiseLines;

    [Header("Narration - Stopped At 99")]
    [SerializeField]
    private NarratorBeat[] floor99Lines;

    [Header("Narration - Too Light")]
    [SerializeField]
    private NarratorBeat[] tooLightLines;

    [Header("Narration - Hamburger Eaten")]
    [SerializeField]
    private NarratorBeat[] hamburgerLines;

    [Header("Narration - Descending")]
    [SerializeField]
    private NarratorBeat[] descendLines;

    private FoyerState state =
        FoyerState.WaitingForButton;

    private bool enteredLiftBefore;

    private void Awake()
    {
        if (floorDisplay != null)
        {
            floorDisplay.text = "1";
        }

        if (floor100Plane != null)
        {
            floor100Plane.SetActive(true);
        }

        if (floor99Plane != null)
        {
            floor99Plane.SetActive(false);
        }

        if (elevatorDoor != null)
        {
            elevatorDoor.SetActive(true);
        }

        if (foodTable != null)
        {
            foodTable.SetActive(false);
        }

        enteredLiftBefore = false;
    }

    public string GetPrompt(
        FoyerInteractionType interactionType)
    {
        switch (interactionType)
        {
            case FoyerInteractionType.ElevatorButton:
                return "Press elevator button";

            case FoyerInteractionType.FloorSign:
                return "Correct the floor sign";

            case FoyerInteractionType.Hamburger:
                return "Eat hamburger";

            default:
                return "Interact";
        }
    }

    public bool CanInteract(
        FoyerInteractionType interactionType)
    {
        switch (interactionType)
        {
            case FoyerInteractionType.ElevatorButton:
                return state ==
                       FoyerState.WaitingForButton;

            case FoyerInteractionType.FloorSign:
                return state ==
                       FoyerState.WaitingForSign;

            case FoyerInteractionType.Hamburger:
                return state ==
                       FoyerState.NeedsFood;

            default:
                return false;
        }
    }

    public void PerformInteraction(
        FoyerInteractionType interactionType)
    {
        if (NarratorManager.Instance != null &&
            NarratorManager.Instance.IsSpeaking)
        {
            return;
        }

        switch (interactionType)
        {
            case FoyerInteractionType.ElevatorButton:
                PressElevatorButton();
                break;

            case FoyerInteractionType.FloorSign:
                CorrectFloorSign();
                break;

            case FoyerInteractionType.Hamburger:
                EatHamburger();
                break;
        }
    }

    private void PressElevatorButton()
    {
        if (state != FoyerState.WaitingForButton)
        {
            return;
        }

        state = FoyerState.ElevatorCounting;

        StartCoroutine(ElevatorSequenceRoutine());
    }

    private IEnumerator ElevatorSequenceRoutine()
    {
        StartCoroutine(
            PlayBeatsWhenAvailable(
                buttonPressedLines
            )
        );

        for (int floor = 2; floor <= 10; floor++)
        {
            SetFloorDisplay(floor);

            yield return new WaitForSeconds(
                normalFloorDelay
            );
        }

        StartCoroutine(
            PlayBeatsWhenAvailable(
                fastRiseLines
            )
        );

        for (int floor = 11; floor <= 99; floor++)
        {
            SetFloorDisplay(floor);

            yield return new WaitForSeconds(
                fastFloorDelay
            );
        }

        SetFloorDisplay(99);

        yield return PlayBeatsWhenAvailable(
            floor99Lines
        );

        state = FoyerState.WaitingForSign;
    }

    private void CorrectFloorSign()
    {
        if (state != FoyerState.WaitingForSign)
        {
            return;
        }

        state = FoyerState.ElevatorOpen;

        if (floor100Plane != null)
        {
            floor100Plane.SetActive(false);
        }

        if (floor99Plane != null)
        {
            floor99Plane.SetActive(true);
        }

        if (dingAudio != null)
        {
            dingAudio.Play();
        }

        if (elevatorDoor != null)
        {
            elevatorDoor.SetActive(false);
        }
    }

    public void PlayerEnteredLift()
    {
        if (state == FoyerState.ElevatorOpen &&
            !enteredLiftBefore)
        {
            enteredLiftBefore = true;
            state = FoyerState.NeedsFood;

            if (floorDisplay != null)
            {
                floorDisplay.text =
                    "99";
            }

            if (foodTable != null)
            {
                foodTable.SetActive(true);
            }

            StartCoroutine(
                PlayBeatsWhenAvailable(
                    tooLightLines
                )
            );

            return;
        }

        if (state == FoyerState.ReadyToDescend)
        {
            state = FoyerState.LoadingNextScene;

            StartCoroutine(
                DescendAndLoadRoutine()
            );
        }
    }

    private void EatHamburger()
    {
        if (state != FoyerState.NeedsFood)
        {
            return;
        }

        state = FoyerState.ReadyToDescend;

        if (hamburgerObject != null)
        {
            hamburgerObject.SetActive(false);
        }

        StartCoroutine(
            PlayBeatsWhenAvailable(
                hamburgerLines
            )
        );
    }

    private IEnumerator DescendAndLoadRoutine()
    {
        yield return PlayBeatsWhenAvailable(
            descendLines
        );

        if (SceneLoader.Instance == null)
        {
            Debug.LogError(
                "No SceneLoader exists in this scene."
            );

            state = FoyerState.ReadyToDescend;
            yield break;
        }

        SceneLoader.Instance.LoadScene(
            nextSceneName
        );
    }

    private void SetFloorDisplay(int floor)
    {
        if (floorDisplay != null)
        {
            floorDisplay.text =
                floor.ToString();
        }
    }

    private IEnumerator PlayBeatsWhenAvailable(
        NarratorBeat[] beats)
    {
        if (beats == null ||
            beats.Length == 0 ||
            NarratorManager.Instance == null)
        {
            yield break;
        }

        while (NarratorManager.Instance.IsSpeaking)
        {
            yield return null;
        }

        bool started =
            NarratorManager.Instance
                .PlayBeatSequence(beats);

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
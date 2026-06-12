using System.Collections;
using TMPro;
using UnityEngine;

public enum StreetLaneType
{
    Sidewalk,
    BikeLane,
    Road
}

public enum StreetZoneType
{
    NPCEncounter,
    Sidewalk,
    BikeLane,
    Road
}

public class StreetSequenceManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float startingTimeSeconds =
        23f * 60f;

    [Header("Lane Walls")]
    [SerializeField] private GameObject bikeLaneWall;
    [SerializeField] private GameObject roadWall;

    [Header("NPC")]
    [SerializeField]
    private StreetNPCBlocker npcBlocker;

    [Header("Narration - Scene Opening")]
    [SerializeField]
    private NarratorBeat[] timerIntroductionLines;

    [Header("Narration - Slow NPC")]
    [SerializeField]
    private NarratorBeat[] npcEncounterLines;

    [Header("Narration - Bike Lane")]
    [SerializeField]
    private NarratorBeat[] bikeLaneLines;

    [Header("Narration - Road")]
    [SerializeField]
    private NarratorBeat[] roadLaneLines;

    private float remainingTime;

    private bool npcEncounterStarted;
    private bool bikeLaneUnlocked;
    private bool roadUnlocked;

    private bool bikeLaneSequencePlayed;
    private bool roadLaneSequencePlayed;

    private void Awake()
    {
        remainingTime = startingTimeSeconds;

        if (bikeLaneWall != null)
        {
            bikeLaneWall.SetActive(true);
        }

        if (roadWall != null)
        {
            roadWall.SetActive(true);
        }

        if (npcBlocker != null)
        {
            npcBlocker.SetLaneForm(
                StreetLaneType.Sidewalk
            );
        }

        UpdateTimerText();
    }

    private IEnumerator Start()
    {
        yield return null;

        yield return PlaySequenceAndWait(
            timerIntroductionLines
        );
    }

    private void Update()
    {
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0f)
            {
                remainingTime = 0f;
            }

            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes =
            Mathf.FloorToInt(
                remainingTime / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                remainingTime % 60f
            );

        timerText.text =
            "Time left\n" +
            $"00:{minutes:00}:{seconds:00}";
    }

    public void EnterZone(StreetZoneType zone)
    {
        switch (zone)
        {
            case StreetZoneType.NPCEncounter:
                StartNPCEncounter();
                break;

            case StreetZoneType.Sidewalk:
                EnterSidewalk();
                break;

            case StreetZoneType.BikeLane:
                EnterBikeLane();
                break;

            case StreetZoneType.Road:
                EnterRoad();
                break;
        }
    }

    private void StartNPCEncounter()
    {
        if (npcEncounterStarted)
        {
            return;
        }

        npcEncounterStarted = true;

        StartCoroutine(
            NPCEncounterRoutine()
        );
    }

    private IEnumerator NPCEncounterRoutine()
    {
        yield return PlaySequenceAndWait(
            npcEncounterLines
        );

        bikeLaneUnlocked = true;

        if (bikeLaneWall != null)
        {
            bikeLaneWall.SetActive(false);
        }
    }

    private void EnterSidewalk()
    {
        if (npcBlocker != null)
        {
            npcBlocker.SetLaneForm(
                StreetLaneType.Sidewalk
            );
        }
    }

    private void EnterBikeLane()
    {
        if (!bikeLaneUnlocked)
        {
            return;
        }

        if (npcBlocker != null)
        {
            npcBlocker.SetLaneForm(
                StreetLaneType.BikeLane
            );
        }

        if (!bikeLaneSequencePlayed)
        {
            bikeLaneSequencePlayed = true;

            StartCoroutine(
                BikeLaneRoutine()
            );
        }
    }

    private IEnumerator BikeLaneRoutine()
    {
        yield return PlaySequenceAndWait(
            bikeLaneLines
        );

        roadUnlocked = true;

        if (roadWall != null)
        {
            roadWall.SetActive(false);
        }
    }

    private void EnterRoad()
    {
        if (!roadUnlocked)
        {
            return;
        }

        if (npcBlocker != null)
        {
            npcBlocker.SetLaneForm(
                StreetLaneType.Road
            );
        }

        if (!roadLaneSequencePlayed)
        {
            roadLaneSequencePlayed = true;

            StartCoroutine(
                PlaySequenceAndWait(
                    roadLaneLines
                )
            );
        }
    }

    private IEnumerator PlaySequenceAndWait(
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
            NarratorManager.Instance
                .PlayBeatSequence(sequence);

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
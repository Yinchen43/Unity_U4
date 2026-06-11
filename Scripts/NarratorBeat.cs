using UnityEngine;

[System.Serializable]
public class NarratorBeat
{
    public NarratorLine line;

    [Min(0f)]
    public float pauseAfter = 0.2f;
}
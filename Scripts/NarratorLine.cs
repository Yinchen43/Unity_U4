using UnityEngine;

[System.Serializable]
public class NarratorLine
{
    [TextArea(2, 6)]
    public string subtitle;

    public AudioClip audioClip;

    [Min(1f)]
    public float charactersPerSecond = 35.0f;

    [Min(0f)]
    public float holdAfterLine = 1.0f;
}
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Audio Clip Data", order = 1)]
public class AudioClipData : ScriptableObject
{
    [Space(1f), Header("Button music"), Space(1f)]
    public AudioClip aud_touch;
    [Space(1f), Header("BG music"), Space(1f)]
    public AudioClip aud_bg;
    [Space(1f), Header("Wrong answer"), Space(1f)]
    public AudioClip aud_wrong;
    [Space(1f), Header("Correct answer"), Space(1f)]
    public AudioClip aud_correct;
    [Space(1f), Header("Win game"), Space(1f)]
    public AudioClip aud_win;
    [Space(1f), Header("Submit answer"), Space(1f)]
    public AudioClip aud_submit;
}
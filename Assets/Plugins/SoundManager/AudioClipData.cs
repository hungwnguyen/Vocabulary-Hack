using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Audio Clip Data", order = 1)]
public class AudioClipData : ScriptableObject
{
    [Space(1f), Header("Button music"), Space(1f)] 
    public AudioClip aud_touch;
}
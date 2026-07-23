using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "SO/Audio/AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    public AudioClip buttonClick;
    public AudioClip swap;
    public AudioClip noSwap;
    public AudioClip matchGem;
    public AudioClip win;
    public AudioClip lose;

    public AudioClip menu;
    public AudioClip gaming;
}
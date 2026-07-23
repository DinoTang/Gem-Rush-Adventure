using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "SO/Audio/AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    public AudioClip buttonClick;
    public AudioClip swapGem;
    public AudioClip matchGem;
    public AudioClip win;
    public AudioClip lose;

    public AudioClip homeBGM;
    public AudioClip gameplayBGM;
}
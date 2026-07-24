using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "SO/Audio/AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    [Header("Sound")]
    public AudioClip buttonClick;
    public AudioClip swap;
    public AudioClip noSwap;
    public AudioClip create;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip progressStar;
    [Header("Music")]
    public AudioClip menu;
    public AudioClip gaming;

    [Header("Clear")]
    public AudioClip matchGem;
    public AudioClip clearBomb;
    public AudioClip clearRocket;
    public AudioClip clearBombAndRocket;

    [Header("Combo Sound")]
    public List<AudioClip> comboSounds;

    [Header("Combo Voice Sound")]
    public List<AudioClip> comboVoices;
}
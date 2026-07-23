using System.IO;
using UnityEngine;

public class AudioManager : BaseBehaviour
{
    public static AudioManager instance;
    protected static AudioManager Instance => instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioDataSO audioDataSO;

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void SetMusicVolume(float value)
    {
        this.bgmSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        this.sfxSource.volume = value;
    }
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        if (this.bgmSource.clip == clip)
            return;

        this.bgmSource.clip = clip;
        this.bgmSource.loop = true;
        this.bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        this.sfxSource.PlayOneShot(clip);
    }
}
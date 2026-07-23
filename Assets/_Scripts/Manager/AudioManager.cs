using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : BaseBehaviour
{
    protected static AudioManager instance;
    public static AudioManager Instance => instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioDataSO audioDataSO;
    public AudioDataSO AudioDataSO => audioDataSO;
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
    protected override void OnEnable()
    {
        SceneManager.activeSceneChanged +=
            this.OnSceneChanged;
    }

    protected override void OnDisable()
    {
        SceneManager.activeSceneChanged -=
            this.OnSceneChanged;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBGMSource();
        this.LoadSFXSource();
        this.LoadAudioDataSO();
    }
    public void SetMusicVolume(float value)
    {
        this.bgmSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        this.sfxSource.volume = value;
    }

    protected void LoadBGMSource()
    {
        if (this.bgmSource != null) return;
        this.bgmSource = transform.Find("BGMSource").GetComponent<AudioSource>();
        Debug.Log(transform.name + ": LoadBGMSource");
    }

    protected void LoadSFXSource()
    {
        if (this.sfxSource != null) return;
        this.sfxSource = transform.Find("SFXSource").GetComponent<AudioSource>();
        Debug.Log(transform.name + ": LoadSFXSource");
    }

    protected void LoadAudioDataSO()
    {
        if (this.audioDataSO != null) return;
        this.audioDataSO = Resources.Load<AudioDataSO>("AudioDataSO");
        Debug.Log(transform.name + ": LoadAudioDataSO");
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        this.bgmSource.Stop();
        this.bgmSource.clip = clip;
        this.bgmSource.loop = true;
        this.bgmSource.time = 0f;
        this.bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        this.sfxSource.PlayOneShot(clip);
    }

    public void ToggleMusic(bool isOn)
    {
        this.bgmSource.mute = !isOn;
    }

    public void ToggleSound(bool isOn)
    {
        this.sfxSource.mute = !isOn;
    }

    private void OnSceneChanged(
    Scene current,
    Scene next)
    {
        switch (next.name)
        {
            case "BootScene":
                this.PlayBGM(
                    this.audioDataSO.menu
                );
                break;

            case "HomeScene":
                this.PlayBGM(
                    this.audioDataSO.menu
                );
                break;

            case "GamePlayScene":
                this.PlayBGM(
                    this.audioDataSO.gaming
                );
                break;
        }
    }
}
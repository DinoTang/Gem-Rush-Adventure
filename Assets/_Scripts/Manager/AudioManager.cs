using System.Collections.Generic;
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

    public void StopBGM()
    {
        this.bgmSource.Stop();
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
        SaveManager.Instance.SaveData.musicEnabled = isOn;
        SaveManager.Instance.Save();

        if (isOn)
        {
            this.PlayCurrentBGM();
        }
        else
        {
            this.bgmSource.Stop();
        }
    }

    public void ToggleSound(bool isOn)
    {
        this.sfxSource.mute = !isOn;

        SaveManager.Instance.SaveData.soundEnabled = isOn;
        SaveManager.Instance.Save();
    }

    private void OnSceneChanged(
    Scene current,
    Scene next)
    {
        AudioClip targetClip = this.GetBGMForScene(next.name);

        if (targetClip == null)
        {
            this.StopBGM();
            return;
        }

        this.SetCurrentBGM(targetClip);

        if (SaveManager.Instance.SaveData.musicEnabled)
            this.PlayCurrentBGM();
        else
            this.bgmSource.Stop();
    }

    public void PlayMatchClearSound(List<MatchResult> matches, int comboIndex)
    {
        this.PlayClearSound(matches);
        this.PlayComboSound(comboIndex);
        this.PlayComboVoice(comboIndex);
    }

    private void PlayClearSound(
    List<MatchResult> matches)
    {
        PlaySFX(this.audioDataSO.matchGem);
    }

    private void PlayComboSound(
    int combo)
    {
        combo = Mathf.Clamp(combo, 1, 5);

        PlaySFX(
            audioDataSO.comboSounds[combo - 1]
        );
    }

    private void PlayComboVoice(
    int combo)
    {
        switch (combo)
        {
            case 1:
                return;

            case 2:
                PlaySFX(this.audioDataSO.comboVoices[1]); //good
                break;

            case 3:
                PlaySFX(this.audioDataSO.comboVoices[2]); //cool
                break;

            case 4:
                PlaySFX(this.audioDataSO.comboVoices[3]); //sweet
                break;

            case 5:
                PlaySFX(this.audioDataSO.comboVoices[4]); //amazing
                break;

            case 6:
                PlaySFX(this.audioDataSO.comboVoices[5]); //wonderful
                break;

            default:
                PlaySFX(this.audioDataSO.comboVoices[0]); //incredible
                break;
        }
    }

    private bool IsRocket(GemSpecialType type)
    {
        return type == GemSpecialType.HorizontalRocket ||
               type == GemSpecialType.VerticalRocket;
    }

    public void PlaySpecialClearSound(GemCtrl gemA, GemCtrl gemB)
    {
        GemSpecialType typeA = gemA.GemData.GemSpecialType;
        GemSpecialType typeB = gemB.GemData.GemSpecialType;

        bool hasCube = typeA == GemSpecialType.Cube || typeB == GemSpecialType.Cube;
        bool hasBomb = typeA == GemSpecialType.Bomb || typeB == GemSpecialType.Bomb;
        bool hasRocket = this.IsRocket(typeA) || this.IsRocket(typeB);

        if (typeA == GemSpecialType.Cube || typeB == GemSpecialType.Cube)
        {
            // this.PlaySFX(this.audioDataSO.clearCube);
            return;
        }

        if (hasBomb && hasRocket)
        {
            this.PlaySFX(this.audioDataSO.clearBombAndRocket);
            return;
        }

        if (hasBomb)
        {
            this.PlaySFX(this.audioDataSO.clearBomb);
            return;
        }

        if (hasRocket)
        {
            this.PlaySFX(this.audioDataSO.clearRocket);
        }

    }

    public void PlaySpecialClearSound(
    GemSpecialType type)
    {
        switch (type)
        {
            case GemSpecialType.Bomb:
                PlaySFX(audioDataSO.clearBomb);
                break;

            case GemSpecialType.HorizontalRocket:
            case GemSpecialType.VerticalRocket:
                PlaySFX(audioDataSO.clearRocket);
                break;

            case GemSpecialType.Cube:
                // PlaySFX(audioDataSO.clearCube);
                break;
        }
    }

    private AudioClip GetBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "BootScene":
            case "HomeScene":
                return this.audioDataSO.menu;

            case "GamePlayScene":
                return this.audioDataSO.gaming;

            default:
                return null;
        }
    }

    private void SetCurrentBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        if (this.bgmSource.clip == clip)
            return;

        this.bgmSource.Stop();
        this.bgmSource.clip = clip;
        this.bgmSource.loop = true;
        this.bgmSource.time = 0f;
    }

    private void PlayCurrentBGM()
    {
        if (this.bgmSource.clip == null)
            return;

        this.bgmSource.mute = false;

        if (!this.bgmSource.isPlaying)
            this.bgmSource.Play();
    }
}
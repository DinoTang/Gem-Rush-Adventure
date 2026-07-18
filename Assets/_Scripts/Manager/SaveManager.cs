using System.IO;
using UnityEngine;

public class SaveManager : BaseBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance => instance;

    private PlayerSaveData saveData;
    public PlayerSaveData SaveData => saveData;

    private string savePath;

    protected override void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "player_save.json"
        );

        Load();
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            saveData = new PlayerSaveData();
            Save();
            return;
        }

        string json = File.ReadAllText(savePath);

        saveData =
            JsonUtility.FromJson<PlayerSaveData>(json);

        if (saveData == null)
        {
            saveData = new PlayerSaveData();
        }
    }

    public void Save()
    {
        string json =
            JsonUtility.ToJson(saveData, true);

        File.WriteAllText(savePath, json);

        Debug.Log($"Saved at: {savePath}");
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0)
            return;

        saveData.totalCoin += amount;
        Save();
    }

    public LevelProgressData GetLevelProgress(
    int levelId)
    {
        foreach (LevelProgressData progress
            in this.saveData.levels)
        {
            if (progress.levelId == levelId)
                return progress;
        }

        LevelProgressData newProgress =
            new()
            {
                levelId = levelId,
                bestScore = 0,
                starCount = 0,
                isUnlocked = levelId == 1
            };

        this.saveData.levels.Add(newProgress);

        this.Save();

        return newProgress;
    }

    public void CompleteLevel(
    int levelId,
    int finalScore,
    int earnedStars,
    int earnedCoin)
    {
        LevelProgressData progress =
            this.GetLevelProgress(levelId);

        progress.bestScore =
            Mathf.Max(
                progress.bestScore,
                finalScore
            );

        progress.starCount =
            Mathf.Max(
                progress.starCount,
                earnedStars
            );

        if (earnedCoin > 0)
        {
            this.saveData.totalCoin += earnedCoin;
        }

        int nextLevelId = levelId + 1;

        LevelProgressData nextLevel =
            this.GetLevelProgress(nextLevelId);

        nextLevel.isUnlocked = true;

        this.saveData.highestUnlockedLevel =
            Mathf.Max(
                this.saveData.highestUnlockedLevel,
                nextLevelId
            );

        this.Save();

        Debug.Log(
            $"Complete Level {levelId} | " +
            $"Score: {finalScore} | " +
            $"Stars: {earnedStars} | " +
            $"Coin: {earnedCoin}"
        );
    }
}
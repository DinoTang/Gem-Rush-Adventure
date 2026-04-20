using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : BaseBehaviour
{
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;

    public GameObject[] gemPrefabs;

    protected Transform[,] grid;
    private static GridModel<GemCtrl> gridModel;
    private static GemType[] types =
    {
        GemType.Red,
        GemType.Blue,
        GemType.Green,
        GemType.Yellow,
        GemType.Purple
    };
    protected override void Start()
    {
        this.grid = new Transform[this.width, this.height];
        gridModel = new GridModel<GemCtrl>(width, height);

        this.SpawnGrid();
    }
    protected void SpawnGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = new Vector2(x, -y);

                GemType type = GetSafeRandomGemType(x, y);
                GemCtrl prefab = GetGemBy(type);

                Transform gem = Instantiate(prefab.transform, pos, Quaternion.identity);

                this.grid[x, y] = gem;
                gridModel.Set(x, y, gem.GetComponent<GemCtrl>());
            }
        }
    }
    protected GemCtrl GetGemBy(GemType gemType)
    {
        foreach (var gem in this.gemPrefabs)
        {
            GemCtrl gemCtrl = gem.GetComponent<GemCtrl>();
            if (gemCtrl.GemModel.GemType != gemType) continue;
            return gemCtrl;
        }
        return null;
    }
    protected GemType GetSafeRandomGemType(int x, int y)
    {
        List<GemType> availableTypes = new List<GemType>(types);

        RemoveHorizontalMatchCandidate(x, y, availableTypes);
        RemoveVerticalMatchCandidate(x, y, availableTypes);

        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
    protected void RemoveHorizontalMatchCandidate(int x, int y, List<GemType> availableTypes)
    {
        if (x < 2)
            return;

        GemType left1 = gridModel.Get(x - 1, y).GemModel.GemType;
        GemType left2 = gridModel.Get(x - 2, y).GemModel.GemType;

        if (left1 == left2 && left1 != GemType.None)
            availableTypes.Remove(left1);
    }

    protected void RemoveVerticalMatchCandidate(int x, int y, List<GemType> availableTypes)
    {
        if (y < 2)
            return;

        GemType up1 = gridModel.Get(x, y - 1).GemModel.GemType;
        GemType up2 = gridModel.Get(x, y - 2).GemModel.GemType;

        if (up1 == up2 && up1 != GemType.None)
            availableTypes.Remove(up1);
    }
}

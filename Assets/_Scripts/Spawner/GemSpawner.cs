using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemSpawner : Spawner<GemCtrl>
{
    [Header("GemSpawner")]
    [SerializeField] public Transform gemVisualPrefab;
    [SerializeField] public List<GemVisual> gemVisuals;
    private GemType[] types =
    {
        GemType.Red,
        GemType.Blue,
        GemType.Green,
        GemType.Yellow,
        // GemType.Purple,
        // GemType.Special
    };
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadGemVisuals();
    }
    protected void LoadGemVisualPrefab()
    {
        if (this.gemVisualPrefab != null) return;
        this.gemVisualPrefab = transform.Find("GemVisuals");
        Debug.Log(transform.name + ": LoadGemVisualPrefab", gameObject);
    }
    protected void LoadGemVisuals()
    {
        LoadGemVisualPrefab();

        if (gemVisuals.Count > 0) return;

        foreach (Transform child in gemVisualPrefab)
        {
            GemVisual visual = child.GetComponent<GemVisual>();
            if (visual != null)
                gemVisuals.Add(visual);
        }

        Debug.Log(transform.name + ": LoadGemVisuals", gameObject);
    }
    public GemCtrl Spawn(GemType type, Vector2 pos)
    {
        GemCtrl prefab = this.GetGemPrefabByType(type);
        return Spawn(prefab, pos);
    }
    protected GemCtrl GetGemPrefabByType(GemType gemType)
    {
        foreach (var child in this.prefabs)
        {
            GemCtrl gem = child.GetComponent<GemCtrl>();
            if (gem.GemModel.GemType != gemType) continue;
            return gem;
        }
        return null;
    }
    public GemType GetSafeRandomGemType(int x, int y, GridModel<GemCtrl> grid)
    {
        List<GemType> availableTypes = new List<GemType>(types);

        RemoveHorizontalMatchCandidate(x, y, grid, availableTypes);
        RemoveVerticalMatchCandidate(x, y, grid, availableTypes);

        return availableTypes[Random.Range(0, availableTypes.Count)];
    }
    protected void RemoveHorizontalMatchCandidate(int x, int y, GridModel<GemCtrl> grid, List<GemType> availableTypes)
    {
        if (x < 2)
            return;

        GemType left1 = grid.Get(x - 1, y).GemModel.GemType;
        GemType left2 = grid.Get(x - 2, y).GemModel.GemType;

        if (left1 == left2 && left1 != GemType.None)
            availableTypes.Remove(left1);
    }

    protected void RemoveVerticalMatchCandidate(int x, int y, GridModel<GemCtrl> grid, List<GemType> availableTypes)
    {
        if (y < 2)
            return;

        GemType up1 = grid.Get(x, y - 1).GemModel.GemType;
        GemType up2 = grid.Get(x, y - 2).GemModel.GemType;

        if (up1 == up2 && up1 != GemType.None)
            availableTypes.Remove(up1);
    }

    public List<FallMove> FillEmptyCells(GridModel<GemCtrl> grid)
    {
        List<FallMove> fallMoves = new();
        for (int x = 0; x < grid.Width; x++)
        {
            int stackIndex = 1;
            for (int y = grid.Height - 1; y >= 0; y--)
            {
                if (grid.Get(x, y) != null) continue;


                GemType type = GetRandomPieceType();

                Vector2 spawnPointPos = new Vector3(x, -1 - stackIndex);
                Vector2 targetPointPos = new Vector3(x, y);
                GemCtrl gem = Spawn(type, spawnPointPos);

                gem.Init(type, x, y);

                grid.Set(x, y, gem);

                FallMove fallMove = new()
                {
                    gem = gem,
                    currentPos = spawnPointPos,
                    targetPos = targetPointPos
                };
                fallMoves.Add(fallMove);
                stackIndex++;
            }
        }
        return fallMoves;
    }
    protected GemType GetRandomPieceType()
    {
        return types[Random.Range(0, this.types.Length)];
    }

    public void ReturnAllGemsToPool(GridModel<GemCtrl> grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GemCtrl gem = grid.Get(x, y);
                gem.GemDespawn.DoDespawn();
            }
        }
    }

    public Sprite GetGemVisual(GemType gemType)
    {
        foreach (GemVisual gemVisual in this.gemVisuals)
        {
            if (gemVisual.gemType != gemType) continue;
            return gemVisual.sprite;
        }
        return null;
    }
}

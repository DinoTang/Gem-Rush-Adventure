using System.Collections.Generic;
using UnityEngine;
public enum CommonVFXType
{
    None,
    Shockwave_Clear,
    Shockwave_Merge,
    Sparkle,
    Flash,
    Beam,
    Bomb,
    ElectrolyticCapacitor,
    Cube_1,
    Cube_2,
    Cube_3,
    Cube_Lightning,
}
public class VFXSpawner : Spawner<VFXCtrl>
{
    protected static VFXSpawner instance;
    public static VFXSpawner Instance => instance;
    protected override void Awake()
    {
        base.Awake();
        if (instance != null) Debug.LogWarning("Only 1 VFXSpawner allows to exist");
        instance = this;
    }

    public VFXCtrl SpawnGemVFX(GemType type, Vector2 pos)
    {
        VFXCtrl prefab = GetGemVFXPrefab(type);

        if (prefab == null) return null;

        return Spawn(prefab, pos);
    }
    public VFXCtrl SpawnGemVFXCommon(GemType type, CommonVFXType vfxCommonType, Vector2 pos)
    {
        VFXCtrl prefab = GetGemVFXCommonPrefab(type, vfxCommonType, pos);

        if (prefab == null) return null;

        return Spawn(prefab, pos);
    }
    protected VFXCtrl GetGemVFXPrefab(GemType type)
    {
        foreach (var child in this.prefabs)
        {
            VFXCtrl vfx = child.GetComponent<VFXCtrl>();

            if (vfx.Category != VFXCategory.GemSpecific)
                continue;

            if (vfx.Type != type)
                continue;

            return vfx;
        }

        return null;
    }
    public VFXCtrl GetGemVFXCommonPrefab(GemType type, CommonVFXType vfxCommonType, Vector2 pos)
    {
        foreach (var child in this.prefabs)
        {
            VFXCtrl vfx = child.GetComponent<VFXCtrl>();

            if (vfx.Category != VFXCategory.Common)
                continue;

            if (vfx.Type != type)
                continue;

            if (vfx.CommonType != vfxCommonType)
                continue;

            return vfx;
        }

        return null;
    }
    public void SpawnClearVFX_Normal(GemCtrl gem)
    {
        this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Shockwave_Clear, gem.transform.position);
        this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Sparkle, gem.transform.position);
        this.SpawnGemVFX(gem.GemData.GemType, gem.transform.position);
    }

    public void SpawnTransformVFX(Vector2 pos)
    {
        this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Shockwave_Merge, pos);
    }

    public void SpawnBeamVFX(GemCtrl gem, Vector3 direction, Vector3 rotation)
    {
        VFXCtrl vfx = this.SpawnGemVFXCommon(gem.GemData.GemType, CommonVFXType.Beam, gem.transform.position);
        VFXBeamCtrl vfXBeamCtrl = vfx.GetComponent<VFXBeamCtrl>();
        vfXBeamCtrl.VfxMove.SetDirection(direction);
        vfXBeamCtrl.VfxMove.SetRotation(rotation);
    }

    public void SpawnSpecialVFX(GemCtrl gem)
    {
        switch (gem.GemData.GemSpecialType)
        {
            case GemSpecialType.HorizontalRocket:
                this.SpawnBeamVFX(gem, Vector3.right, Vector3.zero);
                this.SpawnBeamVFX(gem, Vector3.left, new Vector3(0, 0, 180));
                this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Flash, gem.transform.position);
                break;
            case GemSpecialType.VerticalRocket:
                this.SpawnBeamVFX(gem, Vector3.up, new Vector3(0, 0, 90));
                this.SpawnBeamVFX(gem, Vector3.down, new Vector3(0, 0, -90));
                this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Flash, gem.transform.position);
                break;
            case GemSpecialType.Bomb:
                this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Bomb, gem.transform.position);
                break;
            case GemSpecialType.Cube:
                this.SpawnGemVFXCommon(GemType.Cube, CommonVFXType.Cube_2, gem.transform.position);
                break;
        }
    }

    public VFXCtrl SpawnSpecialVFXCubeGem(GemCtrl gem)
    {
        return this.SpawnGemVFXCommon(GemType.Cube, CommonVFXType.Cube_1, gem.transform.position);
    }

    public void SpawnCubeLightningVFX(GemCtrl cubeGem, List<Vector2Int> targetCells, GridModel<GemCtrl> grid)
    {
        if (cubeGem == null || targetCells == null || targetCells.Count == 0)
            return;
        Debug.LogWarning(targetCells.Count + " target cells for cube lightning VFX at " + cubeGem.GemData.GridPos);
        targetCells.RemoveAll(cell => cell.x == cubeGem.GemData.GridPos.x && cell.y == cubeGem.GemData.GridPos.y);

        if (targetCells.Count == 0)
            return;

        List<Vector3> targets = new();
        foreach (var cell in targetCells)
        {
            GemCtrl targetGem = grid.Get(cell.x, cell.y);
            if (targetGem != null)
                targets.Add(targetGem.transform.position);
        }

        if (targets.Count == 0)
            return;

        VFXCtrl spawnedVfx = this.SpawnGemVFXCommon(GemType.Cube, CommonVFXType.Cube_Lightning, cubeGem.transform.position);
        if (spawnedVfx == null)
            spawnedVfx = this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Cube_Lightning, cubeGem.transform.position);

        if (spawnedVfx == null)
        {
            Debug.LogWarning("Cube lightning VFX prefab was not found for type Cube or None.");
            return;
        }

        if (spawnedVfx is not VFXCubeLightningCtrl lightningVfx)
        {
            Debug.LogWarning("Spawned VFX is not VFXCubeLightningCtrl: " + spawnedVfx.GetType().Name);
            return;
        }

        lightningVfx.Play(cubeGem.transform.position, targets);
    }

    public void SpawnGemWasActiveByCubeVFX(GemCtrl cubeGem, List<Vector2Int> finalCells)
    {
        finalCells.RemoveAll(cell => cell.x == cubeGem.GemData.GridPos.x && cell.y == cubeGem.GemData.GridPos.y);
        foreach (var cell in finalCells)
        {
            Vector3 worldPos = BoardManager.Instance.GetWorldPos(cell.x, cell.y);
            this.SpawnGemVFXCommon(GemType.None, CommonVFXType.ElectrolyticCapacitor, new Vector2(worldPos.x, worldPos.y));
        }
    }
}

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
    Electronic,
    Cube_1,
    Cube_2,
    Cube_3,
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

    public void SpawnGemWasActiveByCubeVFX(GemCtrl cubeGem, List<Vector2Int> finalCells)
    {
        finalCells.RemoveAll(cell => cell.x == cubeGem.GemData.GridPos.x && cell.y == cubeGem.GemData.GridPos.y);
        foreach (var cell in finalCells)
        {
            Vector3 worldPos = BoardManager.Instance.GetWorldPos(cell.x, cell.y);
            this.SpawnGemVFXCommon(GemType.None, CommonVFXType.ElectrolyticCapacitor, new Vector2(worldPos.x, worldPos.y));
        }
    }

    public void SpawnCubeClearVFX(GemCtrl gem)
    {
        this.SpawnGemVFXCommon(GemType.Cube, CommonVFXType.Cube_3, gem.transform.position);
    }

    public void SpawnCubeLightningVFX(GemCtrl cubeGem, List<Vector2Int> targetCells)
    {
        List<Vector3> targets = new();

        foreach (var cell in targetCells)
        {
            Vector3 worldPos = BoardManager.Instance.GetWorldPos(cell.x, cell.y);
            targets.Add(worldPos);
        }

        this.SpawnCubeLightningVFX(cubeGem.transform.position, targets);
    }

    public void SpawnCubeLightningVFX(Vector3 from, List<Vector3> targets)
    {
        CubeLightningCtrl vfx = this.SpawnGemVFXCommon(GemType.None, CommonVFXType.Electronic, from).GetComponent<CubeLightningCtrl>();
        vfx.Play(from, targets);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCubeLightningCtrl : VFXCtrl
{
    [Header("Line")]
    [SerializeField] private Material[] lightningMaterials;
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float width = 0.12f;

    [Header("Shape")]
    [SerializeField] private int segmentCount = 8;
    [SerializeField] private float noiseStrength = 0.18f;
    [SerializeField] private float refreshRate = 0.04f;

    private readonly List<LineRenderer> lines = new();

    public void Play(Vector3 from, List<Vector3> targets)
    {
        StopAllCoroutines();
        ClearLines();

        if (targets == null || targets.Count <= 0)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            LineRenderer line = CreateLine();
            lines.Add(line);
        }

        StartCoroutine(PlayRoutine(from, targets));
    }

    private IEnumerator PlayRoutine(Vector3 from, List<Vector3> targets)
    {
        float timer = 0f;
        float refreshTimer = 0f;
        int materialIndex = 0;

        DrawAllLines(from, targets, materialIndex);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            refreshTimer += Time.deltaTime;

            if (refreshTimer >= refreshRate)
            {
                refreshTimer = 0f;
                materialIndex++;

                DrawAllLines(from, targets, materialIndex);
            }

            ApplyConstantWidth();

            yield return null;
        }

        ClearLines();
        // gameObject.SetActive(false);
    }

    private LineRenderer CreateLine()
    {
        GameObject lineObj = new GameObject("CubeLightningLine");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = segmentCount + 1;

        line.startWidth = width;
        line.endWidth = width;

        line.numCapVertices = 2;
        line.numCornerVertices = 2;

        if (lightningMaterials != null && lightningMaterials.Length > 0)
        {
            line.material = lightningMaterials[0];
            ForceMaterialAlpha(line);
        }

        return line;
    }

    private void DrawAllLines(Vector3 from, List<Vector3> targets, int materialIndex)
    {
        int count = Mathf.Min(lines.Count, targets.Count);

        for (int i = 0; i < count; i++)
        {
            if (lines[i] == null)
                continue;

            UpdateMaterial(lines[i], materialIndex);
            DrawLightning(lines[i], from, targets[i]);
        }
    }

    private void DrawLightning(LineRenderer line, Vector3 from, Vector3 to)
    {
        if (line == null)
            return;

        line.positionCount = segmentCount + 1;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 pos = Vector3.Lerp(from, to, t);

            if (i != 0 && i != segmentCount)
            {
                Vector2 offset = Random.insideUnitCircle * noiseStrength;
                pos += new Vector3(offset.x, offset.y, 0f);
            }

            line.SetPosition(i, pos);
        }
    }

    private void ApplyConstantWidth()
    {
        foreach (var line in lines)
        {
            if (line == null)
                continue;

            line.startWidth = width;
            line.endWidth = width;
        }
    }

    private void UpdateMaterial(LineRenderer line, int index)
    {
        if (line == null)
            return;

        if (lightningMaterials == null || lightningMaterials.Length == 0)
            return;

        int materialIndex = index % lightningMaterials.Length;
        line.material = lightningMaterials[materialIndex];

        ForceMaterialAlpha(line);
    }

    private void ForceMaterialAlpha(LineRenderer line)
    {
        if (line == null || line.material == null)
            return;

        if (!line.material.HasProperty("_Color"))
            return;

        Color color = line.material.color;
        color.a = 1f;
        line.material.color = color;
    }

    private void ClearLines()
    {
        foreach (var line in lines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        lines.Clear();
    }
}
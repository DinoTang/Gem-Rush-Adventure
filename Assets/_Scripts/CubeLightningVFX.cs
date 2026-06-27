using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLightningCtrl : VFXCtrl
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

    public float Duration => duration;

    public void Play(Vector3 from, List<Vector3> targets)
    {
        StopAllCoroutines();
        ClearLines();

        foreach (var target in targets)
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

        while (timer < duration)
        {
            timer += Time.deltaTime;
            refreshTimer += Time.deltaTime;

            if (refreshTimer >= refreshRate)
            {
                refreshTimer = 0f;
                materialIndex++;

                for (int i = 0; i < lines.Count; i++)
                {
                    UpdateMaterial(lines[i], materialIndex);
                    DrawLightning(lines[i], from, targets[i]);
                }
            }

            float fade = 1f - (timer / duration);
            // float currentWidth = width * fade;

            foreach (var line in lines)
            {
                line.startWidth = width;
                line.endWidth = width * 0.6f;
            }

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
        line.endWidth = width * 0.6f;
        line.numCapVertices = 2;
        line.numCornerVertices = 2;

        if (lightningMaterials != null && lightningMaterials.Length > 0)
            line.material = lightningMaterials[0];

        return line;
    }

    private void DrawLightning(LineRenderer line, Vector3 from, Vector3 to)
    {
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

    private void UpdateMaterial(LineRenderer line, int index)
    {
        if (lightningMaterials == null || lightningMaterials.Length == 0)
            return;

        int materialIndex = index % lightningMaterials.Length;
        line.material = lightningMaterials[materialIndex];
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
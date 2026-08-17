using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PortraitCamera : BaseBehaviour
{
    private const float TargetAspect = 9f / 16f;

    private Camera cam;

    protected override void Awake()
    {
        cam = GetComponent<Camera>();
        this.UpdateViewport();
    }

    private void OnPreCull()
    {
        this.UpdateViewport();
    }

    private void UpdateViewport()
    {
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > TargetAspect)
        {
            float width = TargetAspect / screenAspect;

            cam.rect = new Rect(
                (1f - width) / 2f,
                0f,
                width,
                1f
            );
        }
        else
        {
            float height = screenAspect / TargetAspect;

            cam.rect = new Rect(
                0f,
                (1f - height) / 2f,
                1f,
                height
            );
        }
    }
}
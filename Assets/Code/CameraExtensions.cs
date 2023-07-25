using UnityEngine;

public static class CameraExtensions
{
    public static Vector3 BottomLeft(this Camera camera)
    {
        float planeHeight = camera.nearClipPlane + Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * camera.aspect;

        return camera.transform.localToWorldMatrix * new Vector4()
        {
            x = -planeWidth / 2,
            y = -planeHeight / 2,
            z = camera.nearClipPlane,
            w = 1
        };
    }
}
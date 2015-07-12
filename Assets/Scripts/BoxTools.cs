using UnityEngine;
using System.Collections;

/*
 * Static class that can be used to create Rect (with Border).
 */
public static class BoxTools {

    private static Texture2D _whiteTexture;
    public static Texture2D whiteTexture {
        get {
            if (_whiteTexture == null) {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }
            return _whiteTexture;
        }
    }


    public static void DrawRect(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawRectBorder(Rect rect, float thickness, Color color) {
        // Top
        DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Bounds GetViewportBounds(Vector3 originViewportPoint, Camera currentCamera, Vector3 currentScreenPosition) {
        var v1 = originViewportPoint;
        var v2 = currentCamera.ScreenToViewportPoint(currentScreenPosition);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = currentCamera.nearClipPlane;
        max.z = currentCamera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}

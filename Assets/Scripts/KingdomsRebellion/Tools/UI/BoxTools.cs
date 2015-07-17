using UnityEngine;

namespace KingdomsRebellion.Tools.UI {

	/// <summary>
	/// Static class that can be used to create Rect (with Border).
	/// </summary>
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
			DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color); // Top
			DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color); // Left
			DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color); // Right
			DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color); // Bottom
		}

		public static Bounds GetViewportBounds(Vector3 originViewportPoint, Camera currentCamera, Vector3 currentScreenPosition) {
			Vector3 v1 = originViewportPoint;
			Vector3 v2 = currentCamera.ScreenToViewportPoint(currentScreenPosition);

			Vector3 min = Vector3.Min(v1, v2);
			Vector3 max = Vector3.Max(v1, v2);
			min.z = currentCamera.nearClipPlane;
			max.z = currentCamera.farClipPlane;

			Bounds bounds = new Bounds();
			bounds.SetMinMax(min, max);
			return bounds;
		}
	}
}
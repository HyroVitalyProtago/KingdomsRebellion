using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;
using KingdomsRebellion.UI;
using UnityEngine;

namespace KingdomsRebellion.Core.Player {

	// This class Select all selectable unit
	public class GenericSelection : KRBehaviour {

		Ray ray;
		RaycastHit hit;

		protected IList<GameObject>[] selectedObjects;
		protected IList<GameObject> selectableObjects;
		protected Vector3? originWorldMousePoint;

		protected virtual void OnEnable() {
			On("OnModelSelect");
			On("OnModelDrag");

			On("OnLeftClickDown");
			On("OnLeftClickUp");
		}

		protected virtual void OnDisable() {
			Off("OnModelSelect");
			Off("OnModelDrag");

			Off("OnLeftClickDown");
			Off("OnLeftClickUp");
		}

		protected virtual void Start() {
			selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < selectedObjects.Length; ++i) {
				selectedObjects[i] = new List<GameObject>();
			}

			selectableObjects = new List<GameObject>();
			foreach (Transform child in transform) {
				selectableObjects.Add(child.gameObject);
			}
		}

		static Color BoxColor = new Color(.8f, .8f, .8f, .25f);
		void OnGUI() {
			if (!originWorldMousePoint.HasValue) return;

			Vector3 originScreenPoint = Camera.main.WorldToScreenPoint(originWorldMousePoint.Value);
			Vector2 pos = new Vector2(originScreenPoint.x, Screen.height - originScreenPoint.y);
			Vector2 size = new Vector2(Input.mousePosition.x - originScreenPoint.x, originScreenPoint.y - Input.mousePosition.y);
			Rect rect = new Rect(pos, size);
			BoxTools.DrawRect(rect, BoxColor);
			BoxTools.DrawRectBorder(rect, 1f, Color.blue);
		}

		void Update() {
			if (!originWorldMousePoint.HasValue) return;

			// for each unit selected not in rect : deselect
			// for each unit not selected in rect : select
			// selectedObjects[NetworkAPI.playerID]
		}

		protected virtual void SelectUnits(int playerID, Vector3 originWorldPoint) {
			foreach (var unit in selectableObjects) {
//				if (IsInRect(unit, originWorldPoint)) {
					selectedObjects[playerID].Add(unit);
//				}
			}
			ApplySelection(playerID);
		}

		void DeselectUnits(int playerID) {
			ApplyDeselection(playerID);
			selectedObjects[playerID].Clear();
		}

//		protected bool IsInRect(GameObject gameObject, Vector3 originWorldPoint) {
//			var originViewportPoint = originCamera.WorldToViewportPoint(originWorldPoint);
//			var camera = Camera.main;
//			var viewportBounds = BoxTools.GetViewportBounds(originViewportPoint, camera, Input.mousePosition);
//			return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
//		}

		protected virtual void ApplySelection(int playerID) {}
		protected virtual void ApplyDeselection(int playerID) {}

		protected virtual void OnModelSelect(int player, Vec2 modelPosition) {
			DeselectUnits(player);

			GameObject go = KRFacade.Find(modelPosition);
			if (go != null) {
				selectedObjects[player].Add(go);
				ApplySelection(player);
			}
		}

		protected virtual void OnModelDrag(int player, Vec2 beginModelPosition, Vec2 endModelPosition, Vec2 z) {
			DeselectUnits(player);

			IEnumerable<GameObject> gos = KRFacade.Find(beginModelPosition, endModelPosition, z);
			foreach (GameObject go in gos) {
				selectedObjects[player].Add(go);
			}
			ApplySelection(player);
		}

		protected virtual void OnLeftClickDown(Vector3 mousePosition) {
			originWorldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
		}

		protected virtual void OnLeftClickUp(Vector3 mousePosition) {
			originWorldMousePoint = null;
		}
	}
}
using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;
using KingdomsRebellion.Network.Link;
using KingdomsRebellion.Inputs;
using KingdomsRebellion.Tools.UI;
using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.Core.Player {

	// TODO: Make this class more generic.
	// This class Select all selectable unit
	public class GenericSelection : KRBehaviour {

		Ray ray;
		private RaycastHit hit;
		protected IList<GameObject>[] selectedObjects;
		// Don't forget to notify (SendMessage) when new Selectable object is created.
		protected IList<GameObject> selectableObjects;
		protected Vector3 originWorldMousePoint;
		Camera originCamera;
		protected bool isDragging;
		protected float timeLeftBeforeDragging;

		protected virtual void OnEnable() {
			On("OnModelSelect");
			On("OnModelMove");
		}

		protected virtual void OnDisable() {
			Off("OnModelSelect");
			Off("OnModelMove");
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
			timeLeftBeforeDragging = .15f;
			isDragging = false;
		}

		public void OnGUI() {
			if (isDragging) {
				Vector2 pos = new Vector2(Camera.main.WorldToScreenPoint(originWorldMousePoint).x, Screen.height - Camera.main.WorldToScreenPoint(originWorldMousePoint).y);
				Vector2 size = new Vector2(Input.mousePosition.x - Camera.main.WorldToScreenPoint(originWorldMousePoint).x, Camera.main.WorldToScreenPoint(originWorldMousePoint).y - Input.mousePosition.y);
				Rect rect = new Rect(pos, size);
				BoxTools.DrawRect(rect, new Color(0.8f, 0.8f, 0.8f, 0.25f));
				BoxTools.DrawRectBorder(rect, 1f, Color.blue);
			}
		}

		protected virtual void SelectUnits(int playerID, Vector3 originWorldPoint) {
			foreach (var unit in selectableObjects) {
				if (IsInRect(unit, originWorldPoint)) {
					selectedObjects[playerID].Add(unit);
				}
			}
			ApplySelection(playerID);
		}

		void DeselectUnits(int playerID) {
			ApplyDeselection(playerID);
			selectedObjects[playerID].Clear();
		}

		protected bool IsInRect(GameObject gameObject, Vector3 originWorldPoint) {
			if (!isDragging) {
				return false;
			}
			var originViewportPoint = originCamera.WorldToViewportPoint(originWorldPoint);
			var camera = Camera.main;
			var viewportBounds = BoxTools.GetViewportBounds(originViewportPoint, camera, Input.mousePosition);
			return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
		}

		protected virtual void ApplySelection(int playerID) {}

		protected virtual void ApplyDeselection(int playerID) {}

		// TODO get unit on modelPoint position and select it
		// if there is no unit on modelPoint, deselect current units
		void OnSelect() {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			if (playerID == NetworkAPI.PlayerId) {
//				this.originWorldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			}
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
				originCamera = GetComponent<Camera>();
				if (!Input.GetKey(KeyCode.LeftControl)) {
					DeselectUnits(NetworkAPI.PlayerId);
				}
			}
			if (hit.collider != null) {
				var colliderGameObject = hit.collider.gameObject;
				if (selectableObjects.Contains(colliderGameObject)) {
					if (!selectedObjects[NetworkAPI.PlayerId].Contains(colliderGameObject)) {
						selectedObjects[NetworkAPI.PlayerId].Add(colliderGameObject);
						// ApplySelection(NetworkAPI.PlayerId);
					} else {
						// ApplyDeselection(NetworkAPI.PlayerId);
						selectedObjects[NetworkAPI.PlayerId].Remove(colliderGameObject);
					}
				}
			}
		}

		protected virtual void OnModelSelect(int player, Vec3 modelPosition) {
			DeselectUnits(player);

			GameObject go = KRFacade.Find(new Vec2(modelPosition.X, modelPosition.Z));
			if (go != null) {
				selectedObjects[player].Add(go);
				ApplySelection(player);
			}
		}

		protected virtual void OnModelMove(int player, Vec3 modelPosition) {
			DeselectUnits(player);
			
			GameObject go = KRFacade.Find(new Vec2(modelPosition.X, modelPosition.Z));
			if (go != null) {
				selectedObjects[player].Add(go);
				ApplySelection(player);
			}
		}

		protected virtual void OnUpdateDrag(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
			if (!isDragging) {
				timeLeftBeforeDragging -= Time.deltaTime;
			}
			if (timeLeftBeforeDragging <= 0f) {
				isDragging = true;
			}
		}

		void OnDrag(int playerID, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
			SelectUnits(playerID, originWorldPoint);
			isDragging = false;
			timeLeftBeforeDragging = .15f;
		}
	}
}
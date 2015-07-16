using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Network;
using KingdomsRebellion.Network.Link;
using KingdomsRebellion.Inputs;
using KingdomsRebellion.Tools.UI;

namespace KingdomsRebellion.Core {

//TODO: Make this class more generic.
//This class Select all selectable unit
	public class GenericSelection : MonoBehaviour {

		Ray ray;
		private RaycastHit hit;
		protected IList<GameObject>[] selectedObjects;
		//Don't forget to notify (SendMessage) when new Selectable object is created.
		protected IList<GameObject> selectableObjects;
		protected Vector3 originWorldMousePoint;
		Camera originCamera;
		protected bool isDragging;
		protected float timeLeftBeforeDragging;

		protected virtual void OnEnable() {
			Mouse.OnUpdateDrag += OnUpdateDrag;
			SelectAction.OnSelect += OnSelect;
			Mouse.OnDrag += OnDrag;
		}

		protected virtual void OnDisable() {
			Mouse.OnUpdateDrag -= OnUpdateDrag;
			SelectAction.OnSelect -= OnSelect;
			Mouse.OnDrag -= OnDrag;
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

		protected virtual void ApplySelection(int playerID) {
		}

		protected virtual void ApplyDeselection(int playerID) {
		}

		void OnSelect(int playerID, Camera camera, Vector3 mousePosition) {
			ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(mousePosition));
			if (playerID == NetworkAPI.PlayerId) {
				this.originWorldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
				originCamera = camera;
				if (!Input.GetKey(KeyCode.LeftControl)) {
					DeselectUnits(playerID);
				}
			}
			if (hit.collider != null) {
				var colliderGameObject = hit.collider.gameObject;
				// FIXME Resolve problem when holding LeftControl to forbid to select unit of different color.
				Debug.Log("mousePosition = " + mousePosition);
				Debug.Log("[1] " + playerID + " ; collider : " + hit.collider.gameObject.name);
				if (selectableObjects.Contains(colliderGameObject)) {
					Debug.Log("[2] " + playerID);
					if (!selectedObjects[playerID].Contains(colliderGameObject)) {
						Debug.Log("add something for player " + playerID);
						selectedObjects[playerID].Add(colliderGameObject);
						ApplySelection(playerID); // TEST network
					} else {
						ApplyDeselection(playerID);
						selectedObjects[playerID].Remove(colliderGameObject);
					}
				}
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
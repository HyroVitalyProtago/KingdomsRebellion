using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Network;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.Core.Player {
	
	public class Selection : GenericSelection {

		protected Color playerColor;
		IList<GameObject> playerPreSelected;
		IList<GameObject> ennemyPreSelected;

		event Action< int, IList<GameObject> > OnSelection;

		protected override void OnEnable() {
			base.OnEnable();
            On("OnUnitDeath");

			Offer("OnSelection");
		}

		void PreSelected(GameObject go) {
			var unit = go.GetComponent<Unit>();
			if (unit == null)
				return;

            if (unit.color == playerColor) {
				if (!playerPreSelected.Contains(go)) {
					playerPreSelected.Add(go);
				}
			} else {
				if (!ennemyPreSelected.Contains(go) && playerPreSelected.Count == 0) {
					ennemyPreSelected.Add(go);
				}
			}
		}

		protected override void OnUpdateDrag(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
			if (!isDragging) {
				timeLeftBeforeDragging -= Time.deltaTime;
			} else {
				for (int i = 0; i < selectableObjects.Count; ++i) {
					if (IsInRect(selectableObjects[i], originWorldPoint)) {
						PreSelected(selectableObjects[i]);
					} else if (playerPreSelected.Remove(selectableObjects[i]) || ennemyPreSelected.Remove(selectableObjects[i])) {
						selectableObjects[i].GetComponent<HealthBar>().HideHealthBar();
					}
				}
				if (playerPreSelected.Count > 0) {
					foreach (var unit in playerPreSelected) {
						unit.GetComponent<HealthBar>().ShowHealthBar();
					}
					foreach (var unit in ennemyPreSelected) {
						unit.GetComponent<HealthBar>().HideHealthBar();
					}
					ennemyPreSelected.Clear();
				} else if (ennemyPreSelected.Count > 0) {
					foreach (var unit in ennemyPreSelected) {
						unit.GetComponent<HealthBar>().ShowHealthBar();
					}
					foreach (var unit in playerPreSelected) {
						unit.GetComponent<HealthBar>().HideHealthBar();
					}
					playerPreSelected.Clear();
				}
			}
			if (timeLeftBeforeDragging <= 0f) {
				isDragging = true;
			}
		}

		protected override void Start() {
			selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < selectedObjects.Length; ++i) {
				selectedObjects[i] = new List<GameObject>();
			}

			selectableObjects = new List<GameObject>();
			foreach (Transform child in transform) {
				selectableObjects.Add(child.gameObject);

				// TEST add game objects in FlatGrid
				if (child.position.x >= 0 && child.position.z >= 0) {
//					Vec2 modelPosition = Vec2.FromVector3(child.position);
//					Debug.Log("Add unit on (" + modelPosition.X + ", " + modelPosition.Y + ")");
//					KRFacade.GetMap().Add(child.gameObject.GetComponent<Unit>());
				}
			}
			timeLeftBeforeDragging = .15f;
			isDragging = false;
			playerColor = Color.blue;
			playerPreSelected = new List<GameObject>();
			ennemyPreSelected = new List<GameObject>();
		}

		protected override void SelectUnits(int playerID, Vector3 originWorldPoint) {
			if (playerPreSelected.Count > 0) {
				selectedObjects[playerID] = playerPreSelected;
			} else if (ennemyPreSelected.Count > 0) {
				selectedObjects[playerID] = ennemyPreSelected;
			}
			if (!isDragging) {
				foreach (var unit in selectedObjects[playerID]) {
					unit.GetComponent<HealthBar>().ShowHealthBar();
				}
			}
			ApplySelection(playerID);
		}

		protected override void ApplySelection(int playerID) {
			if (playerID == NetworkAPI.PlayerId && selectedObjects[playerID].Count == 1) { // show healthbar for selection of one unit
				selectedObjects[playerID][0].GetComponent<HealthBar>().ShowHealthBar();
			}
			if (OnSelection != null) {
				OnSelection(playerID, selectedObjects[playerID]);
			}
		}

		protected override void ApplyDeselection(int playerID) {
			foreach (var go in selectedObjects[playerID]) {
                // ATTENTION: if testing on one computer, selected units died only on one side.
				go.GetComponent<HealthBar>().HideHealthBar();
			}
		}

        void OnUnitDeath(GameObject go) {
			selectableObjects.Remove(go);
            for (int player = 0; player < selectedObjects.Length; ++player) {
                selectedObjects[player].Remove(go);
            }
            if (!playerPreSelected.Remove(go)) {
				ennemyPreSelected.Remove(go);
			}
		}


		// TODO EventCondutor inheritance check
		protected override void OnModelSelect(int player, Vec3 modelPosition) {
			base.OnModelSelect(player, modelPosition);
		}

	}
}
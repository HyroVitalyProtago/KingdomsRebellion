using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.UI;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Inputs;

namespace KingdomsRebellion.Core.Player {
	
	public sealed class PSelection : KRBehaviour {

		static Color _BoxColor = new Color(.8f, .8f, .8f, .25f);

		IList<GameObject>[] _selectedObjects;
		IList<GameObject> _selectableObjects;
		Vector3? _originWorldMousePoint;

		IList<GameObject> playerPreSelected;
		IList<GameObject> ennemyPreSelected;
		
		event Action<int, IList<GameObject>> OnSelection;
		
		void Awake() {
			On("OnModelSelect");
			On("OnModelDrag");
			
			On("OnLeftClickDown");
			On("OnLeftClickUp");

			On("OnBirth");
			On("OnDeath");

			Offer("OnSelection");

			_selectableObjects = new List<GameObject>();
		}
		
		void Start() {
			_selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < _selectedObjects.Length; ++i) {
				_selectedObjects[i] = new List<GameObject>();
			}
			
			playerPreSelected = new List<GameObject>();
			ennemyPreSelected = new List<GameObject>();
		}

		void OnGUI() {
			if (!_originWorldMousePoint.HasValue || _originWorldMousePoint.Value == Camera.main.ScreenToWorldPoint(Input.mousePosition)) return;
			
			Vector3 originScreenPoint = Camera.main.WorldToScreenPoint(_originWorldMousePoint.Value);
			Vector2 pos = new Vector2(originScreenPoint.x, Screen.height - originScreenPoint.y);
			Vector2 size = new Vector2(Input.mousePosition.x - originScreenPoint.x, originScreenPoint.y - Input.mousePosition.y);
			Rect rect = new Rect(pos, size);
			BoxTools.DrawRect(rect, _BoxColor);
			BoxTools.DrawRectBorder(rect, 1f, Color.blue);
		}
		
		void Update() {
			if (!_originWorldMousePoint.HasValue || _originWorldMousePoint.Value == Camera.main.ScreenToWorldPoint(Input.mousePosition)) return;

			OnUpdateDrag();
		}
		
		void OnUpdateDrag() {
			for (int i = 0; i < _selectableObjects.Count; ++i) {
				if (IsInRect(_selectableObjects[i])) {
					PreSelected(_selectableObjects[i]);
				} else if (playerPreSelected.Remove(_selectableObjects[i]) || ennemyPreSelected.Remove(_selectableObjects[i])) {
					_selectableObjects[i].GetComponent<HealthBar>().Hide();
				}
			}
			if (playerPreSelected.Count > 0) {
				foreach (var go in playerPreSelected) {
					go.GetComponent<HealthBar>().Show();
				}
				foreach (var go in ennemyPreSelected) {
					go.GetComponent<HealthBar>().Hide();
				}
				ennemyPreSelected.Clear();
			} else if (ennemyPreSelected.Count > 0) {
				foreach (var go in ennemyPreSelected) {
					go.GetComponent<HealthBar>().Show();
				}
				foreach (var go in playerPreSelected) {
					go.GetComponent<HealthBar>().Hide();
				}
				playerPreSelected.Clear();
			}
		}

		bool IsInRect(GameObject go) {
			return new List<GameObject>(
				KRFacade.Find(
					InputNetworkAdapter.BeginDrag,
					Vec2.FromVector3(InputNetworkAdapter.WorldPosition(Input.mousePosition)),
					Vec2.FromVector3(
						InputNetworkAdapter.WorldPosition(
							new Vector3(
								Input.mousePosition.x,
								Camera.main.WorldToScreenPoint(InputNetworkAdapter.BeginDrag.ToVector3()).y,
								Input.mousePosition.z
							)
						)
					)
				)
			).Contains(go);
		}

		void PreSelected(GameObject go) {
			if (go.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId) {
				if (!playerPreSelected.Contains(go)) {
					playerPreSelected.Add(go);
				}
			} else if (!ennemyPreSelected.Contains(go) && playerPreSelected.Count == 0) {
				ennemyPreSelected.Add(go);
			}
		}
		
		void SelectUnits(int playerID, Vector3 originWorldPoint) {
			if (playerPreSelected.Count > 0) {
				_selectedObjects[playerID] = playerPreSelected;
			} else if (ennemyPreSelected.Count > 0) {
				_selectedObjects[playerID] = ennemyPreSelected;
			}

			foreach (var unit in _selectedObjects[playerID]) {
				unit.GetComponent<HealthBar>().Show();
			}

			ApplySelection(playerID);
		}

		void DeselectUnits(int playerID) {
			ApplyDeselection(playerID);
			_selectedObjects[playerID].Clear();
		}
		
		void ApplySelection(int playerID) {
			foreach (var o in _selectedObjects[playerID]) {
				o.GetComponent<KRSelection>().Select();
			}
			if (OnSelection != null) {
				OnSelection(playerID, _selectedObjects[playerID]);
			}
		}
		
		void ApplyDeselection(int playerID) {
			foreach (var go in _selectedObjects[playerID]) {
				go.GetComponent<KRSelection>().Deselect();
			}
		}

		void OnModelSelect(int player, Vec2 modelPosition) {
			DeselectUnits(player);
			
			GameObject go = KRFacade.Find(modelPosition);
			if (go != null) {
				_selectedObjects[player].Add(go);
				ApplySelection(player);
			}
		}
		
		void OnModelDrag(int player, Vec2 beginModelPosition, Vec2 endModelPosition, Vec2 z) {
			DeselectUnits(player);
			
			IEnumerable<GameObject> gos = KRFacade.Find(beginModelPosition, endModelPosition, z);
			foreach (GameObject go in gos) {
				_selectedObjects[player].Add(go);
			}
			ApplySelection(player);
		}
		
		void OnLeftClickDown(Vector3 mousePosition) {
			_originWorldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
		}
		
		void OnLeftClickUp(Vector3 mousePosition) {
			_originWorldMousePoint = null;
		}

		void OnBirth(GameObject go) {
			_selectableObjects.Add(go);
		}

		void OnDeath(GameObject go) {
			if (!_selectableObjects.Remove(go)) { return; }
			for (int player = 0; player < _selectedObjects.Length; ++player) {
				_selectedObjects[player].Remove(go);
			}
			if (!playerPreSelected.Remove(go)) {
				ennemyPreSelected.Remove(go);
			}
		}
	}
}
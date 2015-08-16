using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.UI;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Inputs;
using System.Linq;

namespace KingdomsRebellion.Core.Player {
	
	public sealed class PSelection : KRBehaviour {

		static Color _BoxColor = new Color(.8f, .8f, .8f, .25f);

		IList<GameObject>[] _selectedObjects;
		IList<GameObject> _selectableObjects;
		Vector3? _originWorldMousePoint;

		IList<GameObject> _playerPreSelected;
		IList<GameObject> _ennemyPreSelected;
		
		event Action<int, IList<GameObject>> OnSelection;
		
		void Awake() {
			On("OnSelectAction");
			On("OnDragAction");
			
			On("OnLeftClickDown");
			On("OnLeftClickUp");

			On("OnBirth");
			On("OnDeath");

			On("OnGameAction");

			Offer("OnSelection");

			_selectableObjects = new List<GameObject>();
		}
		
		void Start() {
			_selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < _selectedObjects.Length; ++i) {
				_selectedObjects[i] = new List<GameObject>();
			}
			
			_playerPreSelected = new List<GameObject>();
			_ennemyPreSelected = new List<GameObject>();
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
				} else if (_playerPreSelected.Remove(_selectableObjects[i]) || _ennemyPreSelected.Remove(_selectableObjects[i])) {
					_selectableObjects[i].GetComponent<HealthBar>().Hide();
				}
			}
			if (_playerPreSelected.Count > 0) {
				foreach (var go in _playerPreSelected) {
					go.GetComponent<HealthBar>().Show();
				}
				foreach (var go in _ennemyPreSelected) {
					go.GetComponent<HealthBar>().Hide();
				}
				_ennemyPreSelected.Clear();
			} else if (_ennemyPreSelected.Count > 0) {
				foreach (var go in _ennemyPreSelected) {
					go.GetComponent<HealthBar>().Show();
				}
				foreach (var go in _playerPreSelected) {
					go.GetComponent<HealthBar>().Hide();
				}
				_playerPreSelected.Clear();
			}
		}

		bool IsInRect(GameObject go) {
			return new List<GameObject>(
				KRFacade.Find(
					InputModelAdapter.BeginDrag,
					Vec2.FromVector3(InputModelAdapter.WorldPosition(Input.mousePosition)),
					Vec2.FromVector3(
						InputModelAdapter.WorldPosition(
							new Vector3(
								Input.mousePosition.x,
								Camera.main.WorldToScreenPoint(InputModelAdapter.BeginDrag.ToVector3()).y,
								Input.mousePosition.z
							)
						)
					)
				).Where(g => g.GetComponent<KRTransform>().PlayerID != -1)
			).Contains(go);
		}

		void PreSelected(GameObject go) {
			if (go.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId) {
				if (!_playerPreSelected.Contains(go)) {
					_playerPreSelected.Add(go);
				}
			} else if (!_ennemyPreSelected.Contains(go) && _playerPreSelected.Count == 0) {
				_ennemyPreSelected.Add(go);
			}
		}
		
		void SelectUnits(int playerID, Vector3 originWorldPoint) {
			if (_playerPreSelected.Count > 0) {
				_selectedObjects[playerID] = _playerPreSelected;
			} else if (_ennemyPreSelected.Count > 0) {
				_selectedObjects[playerID] = _ennemyPreSelected;
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

		void OnSelectAction(int player, Vec2 modelPosition) {
			DeselectUnits(player);
			
			GameObject go = KRFacade.Find(modelPosition);
			if (go != null && go.GetComponent<KRTransform>().PlayerID != -1) {
				_selectedObjects[player].Add(go);
				ApplySelection(player);
			}
		}
		
		void OnDragAction(int player, Vec2 beginModelPosition, Vec2 endModelPosition, Vec2 z) {
			DeselectUnits(player);
			
			IEnumerable<GameObject> gos = KRFacade.Find(beginModelPosition, endModelPosition, z).Where(go => go.GetComponent<KRTransform>().PlayerID != -1);
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
			if (!_playerPreSelected.Remove(go)) {
				_ennemyPreSelected.Remove(go);
			}
		}

		void OnGameAction(int playerID, Action<GameObject> f) {
			for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
				f(_selectedObjects[playerID][i]);
			}
		}

		public bool IsMines() {
			return _selectedObjects[NetworkAPI.PlayerId].Any(u => u.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId);
		}
		
		public bool IsBuilding() {
			return _selectedObjects[NetworkAPI.PlayerId].All(u => u.GetComponent<KRSpawn>() != null);
		}

	    public List<KRBuild> GetWorkers() {
            List<KRBuild> workers = new List<KRBuild>();
	        foreach (var unit in _selectedObjects[NetworkAPI.PlayerId]) {
	            KRBuild krBuild = unit.GetComponent<KRBuild>();
	            if (krBuild != null) {
	                workers.Add(krBuild);
	            }
	        }
            return workers;
	    }  
	}
}
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Network.Link;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.Core.Player {

	/// <summary>
	/// Receive "model" inputs, filter and convert it in actions
	/// </summary>
	public class PInput : KRBehaviour {

		event Action<GameAction> OnDemand;

		PSelection _selection;
        bool buildMode;
        bool _repareMode;
        List<KRBuild> _workers;
        KeyCode _key;
		void Awake() {
            _selection = GetComponent<PSelection>();

			On("OnModelSelect");
			On("OnModelDrag");
			On("OnModelAction");
			On("OnKeyPress");

			Offer("OnDemand");
		}

	    void Start() {
            buildMode = false;
	    }

		void OnModelSelect(Vec2 v) {
            if (OnDemand != null) {
                if (buildMode) {
                    if (_workers.First().CanBuild(v)) {
                        OnDemand(new BuildAction(_key, v));
                    }
                    _workers.First().DisableBuildMode();
                    buildMode = false;
                } else if (_repareMode) {
                    OnDemand(new BuildAction(_key, v));
                    _repareMode = false;
                } else {
                    OnDemand(new SelectAction(v));
                }
		    }
		}

		void OnModelDrag(Vec2 v1, Vec2 v2, Vec2 v3) {
			if (OnDemand != null) {
				OnDemand(new DragAction(v1, v2, v3));
			}
		}

		void OnModelAction(Vec2 v) {
			if (!KRFacade.IsInBounds(v)) return;
			if (!_selection.IsMines()) return;

			if (_selection.IsBuilding()) {
				if (OnDemand != null) {
					OnDemand(new RallyAction(v));
				}
			} else { // units
				var go = KRFacade.Find(v);
				if (go != null && go.GetComponent<KRTransform>().PlayerID != -1 && go.GetComponent<KRTransform>().PlayerID != NetworkAPI.PlayerId) {
					if (OnDemand != null) {
						OnDemand(new AttackAction(v));
					}
				} else {
					if (OnDemand != null) {
						OnDemand(new MoveAction(v));
					}
				}
			}
		}

		void OnKeyPress(KeyCode k) {
			if (!_selection.IsMines()) return;
		    if (_selection.IsBuilding()) {
		        if (OnDemand != null) {
		            OnDemand(new SpawnAction(k));
		        }
		    } else {
		        _workers = _selection.GetWorkers();
                if (_workers.Count > 0) {
                    if (_workers.First().OnBuild(k)) {
                        buildMode = true;
                    } else {
                        _repareMode = true;
                    }
                    _key = k;
		        }
		    }
		}

	}
}
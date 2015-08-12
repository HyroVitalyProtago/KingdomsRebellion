using UnityEngine;
using System;
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

		void Awake() {
			_selection = GetComponent<PSelection>();

			On("OnModelSelect");
			On("OnModelDrag");
			On("OnModelAction");
			On("OnKeyPress");

			Offer("OnDemand");
		}

		void OnModelSelect(Vec2 v) {
			if (OnDemand != null) {
				OnDemand(new SelectAction(v));
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
				if (go != null && go.GetComponent<KRTransform>().PlayerID != NetworkAPI.PlayerId) {
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
		        if (OnDemand != null) {
		            OnDemand(new BuildAction(k));
		        }
		    }
		}

	}
}
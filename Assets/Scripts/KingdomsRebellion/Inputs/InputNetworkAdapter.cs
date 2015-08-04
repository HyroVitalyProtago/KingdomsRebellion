using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Network.Link;

namespace KingdomsRebellion.Inputs {
	public class InputNetworkAdapter : KRObject {

		static event Action<GameAction> OnDemand;

		static bool Instatiated;

		static Vec2 beginDrag;

		public InputNetworkAdapter() {
			Debug.Assert(!Instatiated);
			Instatiated = true;

			On("OnLeftClickDown");
			On("OnLeftClickUp");
			On("OnRightClick");
			On("OnKeyPress");

			Offer("OnDemand");
		}

		static Vector3 WorldPosition(Vector3 mousePosition) {
			Vector3 worldPosition = new Vector3(-1, -1, -1);
			
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
				if (hit.collider.gameObject.tag == "Selectable") {
					worldPosition = hit.collider.gameObject.transform.position;
				} else {
					worldPosition = hit.point;
				}
			}

			return worldPosition;
		}

		static void OnLeftClickDown(Vector3 mousePosition) {
			beginDrag = Vec2.FromVector3(WorldPosition(mousePosition));
		}

		static void OnLeftClickUp(Vector3 mousePosition) {
			Vector3 worldPosition = WorldPosition(mousePosition);
			Vec2 modelPoint = Vec2.FromVector3(worldPosition);

			if (beginDrag.Dist(modelPoint) < 1) {
				if (OnDemand != null) {
					OnDemand(new SelectAction(modelPoint));
				}
			} else {
				if (OnDemand != null) {
					Vec2 additionalPoint = Vec2.FromVector3(
						WorldPosition(
						new Vector3(mousePosition.x, Camera.main.WorldToScreenPoint(beginDrag.ToVector3()).y, mousePosition.z)
						)
					);
					OnDemand(new DragAction(beginDrag, modelPoint, additionalPoint));
				}
			}
		}

		static void OnRightClick(Vector3 mousePosition) {
		    if (!PlayerActions.IsMines()) return;

			Vector3 worldPosition = new Vector3(-1, -1, -1);
			
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
			    GameObject go = hit.collider.gameObject;
				if (go.CompareTag("Selectable")) {
                    worldPosition = go.transform.position;
					if (go.GetComponent<KRTransform>().PlayerID != NetworkAPI.PlayerId) {
						if (OnDemand != null) {
							OnDemand(new AttackAction(Vec2.FromVector3(worldPosition)));
				            return;
				        }
				    }
				} else {
					worldPosition = hit.point;
				}
			}
			
			if (OnDemand != null) {
				OnDemand(new MoveAction(Vec2.FromVector3(worldPosition)));
			}
		}

		static void OnKeyPress(KeyCode k) {
			if (!PlayerActions.IsMines()) return;
			if (!PlayerActions.IsBuilding()) return;

			if (OnDemand != null) {
				OnDemand(new SpawnAction(k));
			}
		}

	}
}
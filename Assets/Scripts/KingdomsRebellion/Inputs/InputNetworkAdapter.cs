using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Inputs {
	public class InputNetworkAdapter : KRObject {

		event Action<Vec2> OnModelSelectDemand; // modelPosition
		event Action<Vec2, Vec2, Vec2> OnModelDragDemand; // begin:modelPosition, end:modelPosition
		event Action<Vec2> OnModelMoveDemand; // modelPosition
        event Action<Vec2> OnModelAttackDemand; // modelPosition

		static bool Instatiated;

		Vec2 beginDrag;

		public InputNetworkAdapter() {
			Debug.Assert(!Instatiated);
			Instatiated = true;

			On("OnLeftClickDown");
			On("OnLeftClickUp");
			On("OnRightClick");

			Offer("OnModelSelectDemand");
			Offer("OnModelMoveDemand");
            Offer("OnModelAttackDemand");
			Offer("OnModelDragDemand");
		}

		Vector3 WorldPosition(Vector3 mousePosition) {
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

		void OnLeftClickDown(Vector3 mousePosition) {
			beginDrag = Vec2.FromVector3(WorldPosition(mousePosition));
		}

		void OnLeftClickUp(Vector3 mousePosition) {
			Vector3 worldPosition = WorldPosition(mousePosition);
			Vec2 modelPoint = Vec2.FromVector3(worldPosition);

			if (beginDrag.Dist(modelPoint) < 1) {
				if (OnModelSelectDemand != null) {
					OnModelSelectDemand(modelPoint);
				}
			} else {
				if (OnModelDragDemand != null) {
					Vec2 additionalPoint = Vec2.FromVector3(
						WorldPosition(
						new Vector3(mousePosition.x, Camera.main.WorldToScreenPoint(beginDrag.ToVector3()).y, mousePosition.z)
						)
					);
					OnModelDragDemand(beginDrag, modelPoint, additionalPoint);
				}
			}
		}

		void OnRightClick(Vector3 mousePosition) {
		    if (!PlayerActions.IsMines()) return;

			Vector3 worldPosition = new Vector3(-1, -1, -1);
			
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
			    GameObject go = hit.collider.gameObject;
				if (go.CompareTag("Selectable")) {
                    worldPosition = go.transform.position;
					if (go.GetComponent<KRTransform>().PlayerID != NetworkAPI.PlayerId) {
				        if (OnModelAttackDemand != null) {
							OnModelAttackDemand(Vec2.FromVector3(worldPosition));
				            return;
				        }
				    }
				} else {
					worldPosition = hit.point;
				}
			}
			
			if (OnModelMoveDemand != null) {
				OnModelMoveDemand(Vec2.FromVector3(worldPosition));
			}
		}

	}
}
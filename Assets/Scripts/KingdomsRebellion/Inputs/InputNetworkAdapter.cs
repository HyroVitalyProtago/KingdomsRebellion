using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Network;
using UnityEngine;

namespace KingdomsRebellion.Inputs {
	public class InputNetworkAdapter : KRObject {

		event Action<Vec3> OnModelSelectDemand; // modelPosition
		event Action<Vec3, Vec3, Vec3> OnModelDragDemand; // begin:modelPosition, end:modelPosition
		event Action<Vec3> OnModelMoveDemand; // modelPosition
        event Action<Vec3> OnModelAttackDemand; // modelPosition

		static bool Instatiated;

		Vec3 beginDrag;

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
			beginDrag = Vec3.FromVector3(WorldPosition(mousePosition));
		}

		void OnLeftClickUp(Vector3 mousePosition) {
			Vector3 worldPosition = WorldPosition(mousePosition);
			Vec3 modelPoint = Vec3.FromVector3(worldPosition);

			if (beginDrag.Dist(modelPoint) < 1) {
				if (OnModelSelectDemand != null) {
					OnModelSelectDemand(modelPoint);
				}
			} else {
				if (OnModelDragDemand != null) {
					Vec3 additionalPoint = Vec3.FromVector3(
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
				    if (go.GetComponent<KRGameObject>().PlayerId != NetworkAPI.PlayerId) {
				        if (OnModelAttackDemand != null) {
				            OnModelAttackDemand(Vec3.FromVector3(worldPosition));
				            return;
				        }
				    }
				} else {
					worldPosition = hit.point;
				}
			}
			
			if (OnModelMoveDemand != null) {
				OnModelMoveDemand(Vec3.FromVector3(worldPosition));
			}
		}

	}
}
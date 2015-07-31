using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Network;
using UnityEngine;

namespace KingdomsRebellion.Inputs {
	public class InputNetworkAdapter : KRObject {

		event Action<Vec3> OnModelSelectDemand; // modelPosition
		event Action<Vec3> OnModelMoveDemand; // modelPosition
        event Action<Vec3> OnModelAttackDemand; // modelPosition
		static bool Instatiated = false;

		public InputNetworkAdapter() {
			Debug.Assert(!Instatiated);
			Instatiated = true;

			On("OnLeftClick");
			On("OnRightClick");
			Offer("OnModelSelectDemand");
			Offer("OnModelMoveDemand");
            Offer("OnModelAttackDemand");
		}

		void OnLeftClick(Vector3 mousePosition) {
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

			if (OnModelSelectDemand != null) {
				OnModelSelectDemand(Vec3.FromVector3(worldPosition));
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
				    if (go.GetComponent<Unit>().PlayerId != NetworkAPI.PlayerId) {
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
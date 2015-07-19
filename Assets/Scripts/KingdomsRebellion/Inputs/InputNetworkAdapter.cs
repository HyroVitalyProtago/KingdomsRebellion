using UnityEngine;
using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Inputs {
	public class InputNetworkAdapter : KRObject {

		event Action<Vec3> OnModelSelectDemand; // modelPosition
		event Action<Vec3> OnModelMoveDemand; // modelPosition

		static bool Instatiated = false;

		public InputNetworkAdapter() {
			Debug.Assert(!Instatiated);
			Instatiated = true;

			On("OnLeftClick");
			On("OnRightClick");
			Offer("OnModelSelectDemand");
			Offer("OnModelMoveDemand");
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
			
			if (OnModelMoveDemand != null) {
				OnModelMoveDemand(Vec3.FromVector3(worldPosition));
			}
		}

	}
}
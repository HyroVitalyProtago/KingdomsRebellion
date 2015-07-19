using UnityEngine;
using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.Inputs {
	public class Mouse : KRBehaviour {

		event Action<Vector3> OnLeftClick; // mousePosition
		event Action<Vector3> OnRightClick; // mousePosition

		void OnEnable() {
			Offer("OnLeftClick");
			Offer("OnRightClick");
		}

		void Update() {
			if (Input.GetMouseButtonDown(0)) {
				if (OnLeftClick != null) {
					OnLeftClick(Input.mousePosition);
				}
			} else if (Input.GetMouseButtonDown(1)) {
				if (OnRightClick != null) {
					OnRightClick(Input.mousePosition);
				}
			}
		}

	}
}
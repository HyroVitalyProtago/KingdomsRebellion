using UnityEngine;
using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.Inputs {
	public class Mouse : KRBehaviour {

		event Action<Vector3> OnLeftClickDown; // mousePosition
		event Action<Vector3> OnLeftClickUp; // mousePosition
		event Action<Vector3> OnRightClick; // mousePosition

		bool clickDown = false;

		void OnEnable() {
			Offer("OnLeftClickDown");
			Offer("OnLeftClickUp");
			Offer("OnRightClick");
		}

		void Update() {
			if (Input.GetMouseButtonDown(0)) {
				if (OnLeftClickDown != null) {
					OnLeftClickDown(Input.mousePosition);
				}
				clickDown = true;
			} else if (clickDown && (!Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))) {
				if (OnLeftClickUp != null) {
					OnLeftClickUp(Input.mousePosition);
				}
				clickDown = false;
			} else if (Input.GetMouseButtonDown(1)) {
				if (OnRightClick != null) {
					OnRightClick(Input.mousePosition);
				}
			}
		}
	}
}
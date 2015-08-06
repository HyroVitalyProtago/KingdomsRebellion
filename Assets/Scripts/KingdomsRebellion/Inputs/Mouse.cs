using System;
using KingdomsRebellion.Core;
using UnityEngine;

namespace KingdomsRebellion.Inputs {
	public class Mouse : KRBehaviour {

		event Action<Vector3> OnLeftClickDown; // mousePosition
		event Action<Vector3> OnLeftClickUp; // mousePosition
		event Action<Vector3> OnRightClick; // mousePosition
		event Action<KeyCode> OnKeyPress; // keycode

		bool clickDown = false;

		void OnEnable() {
			Offer("OnLeftClickDown");
			Offer("OnLeftClickUp");
			Offer("OnRightClick");
			Offer("OnKeyPress");
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

			// TODO clean
			if (Input.GetKeyUp(KeyCode.C)) {
				if (OnKeyPress != null) {
					OnKeyPress(KeyCode.C);
				}
            } else if (Input.GetKeyUp(KeyCode.V)) {
                if (OnKeyPress != null) {
                    OnKeyPress(KeyCode.V);
                }
            }
		}
	}
}
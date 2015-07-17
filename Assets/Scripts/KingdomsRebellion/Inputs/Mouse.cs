using UnityEngine;
using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.Inputs {
	public class Mouse : KRBehaviour {

		public static event Action<int, Camera, Vector3> OnLeftClick;
		public static event Action<int, Camera, Vector3> OnRightClick;
		public static event Action<int, Vector3, Camera, Vector3> OnUpdateDrag;
		public static event Action<int, Vector3, Camera, Vector3> OnDrag;

		// Vector3 originWorldPoint;

		void Update() {
			if (Input.GetMouseButtonDown(0)) {
				// originWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (OnLeftClick != null)
					OnLeftClick(NetworkAPI.PlayerId, Camera.main, Camera.main.ScreenToWorldPoint(Input.mousePosition));
			} else if (Input.GetMouseButton(0)) {
//			if (OnUpdateDrag != null) OnUpdateDrag(NetworkAPI.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
			} else if (Input.GetMouseButtonUp(0)) {
//			if (OnDrag != null) OnDrag(NetworkAPI.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
			} else if (Input.GetMouseButtonDown(1)) {
				if (OnRightClick != null)
					OnRightClick(NetworkAPI.PlayerId, Camera.main, Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
		}

	}
}
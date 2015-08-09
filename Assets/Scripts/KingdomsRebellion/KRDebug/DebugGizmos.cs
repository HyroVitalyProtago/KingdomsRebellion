using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Map;
using UnityEngine;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.KRDebug {

	public class DebugGizmos : KRBehaviour {

		static event Action OnKRDrawGizmos;

        void Awake() {
        	Offer("OnKRDrawGizmos");
        }

		void OnDrawGizmos() {
			if (OnKRDrawGizmos != null) {
				OnKRDrawGizmos();
			}
		}
	}
}
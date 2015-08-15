using System;
using KingdomsRebellion.Core;

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
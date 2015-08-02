using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Map;
using UnityEngine;

namespace KingdomsRebellion.KRDebug {

	public class DebugGizmos : KRBehaviour {

		static event Action OnKRDrawGizmos;

        static IMap<QuadTreeNode<KRGameObject>, KRGameObject> _map = KRFacade.GetMap();

        void Awake() {
        	Offer("OnKRDrawGizmos");
        }

		void OnDrawGizmos() {
			if (OnKRDrawGizmos != null) { OnKRDrawGizmos(); }
			_map.Walk(DrawNode);
		}

        void DrawNode(QuadTreeNode<KRGameObject> n) {
			Gizmos.color = n.IsFree() ? Color.blue : Color.green;
			Vector3 p0 = n.BottomLeft.ToVector3(), p1 = n.TopRight.ToVector3();
			Gizmos.DrawLine(p0, p0 + new Vector3(n.Width-.2f, 0, .1f));
			Gizmos.DrawLine(p0, p0 + new Vector3(.1f, 0, n.Height-.2f));
			Gizmos.DrawLine(p1, p1 - new Vector3(n.Width-.2f, 0, .1f));
			Gizmos.DrawLine(p1, p1 - new Vector3(.1f, 0, n.Height-.2f));
		}
	}
}
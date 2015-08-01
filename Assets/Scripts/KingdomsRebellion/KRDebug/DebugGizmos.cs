using UnityEngine;
using System.Collections;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace KingdomsRebellion.KRDebug {

	[ExecuteInEditMode]
	public class DebugGizmos : KRBehaviour {

		static IMap<QuadTreeNode<Unit>,Unit> _map = KRFacade.GetMap();

		void OnDrawGizmos() {
			_map.Walk(DrawNode);
			DrawWalkedNodes();
		}

		void DrawNode(QuadTreeNode<Unit> node) {
			if (node.IsFree()) {
				Gizmos.color = Color.blue;
			} else {
				Gizmos.color = Color.green;
			}
			Vector3 p0 = node.BottomLeft.ToVector3(), p1 = node.TopRight.ToVector3();
			Gizmos.DrawLine(p0, p0 + new Vector3(node.Width-.2f, 0, .1f));
			Gizmos.DrawLine(p0, p0 + new Vector3(.1f, 0, node.Height-.2f));
			Gizmos.DrawLine(p1, p1 - new Vector3(node.Width-.2f, 0, .1f));
			Gizmos.DrawLine(p1, p1 - new Vector3(.1f, 0, node.Height-.2f));
		}

		void DrawWalkedNodes() {
			Gizmos.color = Color.magenta;
			foreach (var x in KRFacade.walkedNode) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one*.5f, Vector3.one);
			}

			Gizmos.color = Color.red;
			foreach (var x in KRFacade.walkedFind) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one*.5f, Vector3.one);
			}
		}
	}
}
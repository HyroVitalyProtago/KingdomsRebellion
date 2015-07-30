using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Inputs;
using UnityEngine;

namespace KingdomsRebellion.Core {
	public class KRFacade : KRObject {

		static InputNetworkAdapter InputNetworkAdapter;
		static IMap<QuadTreeNode<Unit>,Unit> Map;

		static KRFacade() {
			InputNetworkAdapter = new InputNetworkAdapter();
			Map = new QuadTree<Unit>(256, 256) as IMap<QuadTreeNode<Unit>,Unit>;
		}

		public static IMap<QuadTreeNode<Unit>,Unit> GetMap() { return Map; }

		public static IEnumerable<QuadTreeNode<Unit>> FindPath(Vec2 start, Vec2 target) {
			return Pathfinding<QuadTreeNode<Unit>>.FindPath(Map.FindNode(start), Map.FindNode(target))
				.Select(abstractNode => abstractNode.WrappedNode());
		}

		public static void UpdateGame() {
			foreach (Unit unit in Map) {
                unit.GetComponent<FiniteStateMachine>().UpdateGame();
		    }
		}

		public static GameObject Find(Vec2 v) {
			Unit u = Map.Find(v);
			return (u == null) ? null : u.gameObject;
		}

		public static IEnumerable<GameObject> Around(Vec2 v, int maxDist) {
			return Map.ToList().Where(u => u.Pos.Dist(v) <= maxDist).Select(u => u.gameObject);
		}
	}
}
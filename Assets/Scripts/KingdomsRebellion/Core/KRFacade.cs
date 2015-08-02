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
		static IMap<QuadTreeNode<KRGameObject>,KRGameObject> Map;

		static KRFacade() {
			InputNetworkAdapter = new InputNetworkAdapter();
			Map = new QuadTree<KRGameObject>(256, 256) as IMap<QuadTreeNode<KRGameObject>,KRGameObject>;
		}

		public static IMap<QuadTreeNode<KRGameObject>,KRGameObject> GetMap() { return Map; }

        public static IEnumerable<QuadTreeNode<KRGameObject>> FindPath(Vec2 start, Vec2 target) {
            return Pathfinding<QuadTreeNode<KRGameObject>>.FindPath(Map.FindNode(start), Map.FindNode(target))
				.Select(abstractNode => abstractNode.WrappedNode());
		}

		public static void UpdateGame() {
            foreach (KRGameObject unit in Map) {
                FiniteStateMachine fsm = unit.GetComponent<FiniteStateMachine>();
                if (fsm != null) {
                    fsm.UpdateGame();
                }
            }
		}

		public static GameObject Find(Vec2 v) {
            KRGameObject u = Map.Find(v);
			return (u == null) ? null : u.gameObject;
		}

		public static IEnumerable<GameObject> Around(Vec2 v, int maxDist) {
			return Map.ToList().Where(u => u.Pos.Dist(v) <= maxDist).Select(u => u.gameObject);
		}
	}
}
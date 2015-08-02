using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Inputs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KingdomsRebellion.Core {
	public static class KRFacade {

		static readonly InputNetworkAdapter _InputNetworkAdapter;
		static readonly IMap<QuadTreeNode<KRGameObject>,KRGameObject> _Map;

#if !UNITY_EDITOR
		static IList<Vec2> __walkedNode = new List<Vec2>();
		static IList<Vec2> __walkedFind = new List<Vec2>();
#endif

		static KRFacade() {
			_InputNetworkAdapter = new InputNetworkAdapter();
			_Map = new QuadTree<KRGameObject>(256, 256);

#if !UNITY_EDITOR
			EventConductor.On(typeof(KRFacade), "OnKRDrawGizmos");
#endif
		}

		public static IMap<QuadTreeNode<KRGameObject>,KRGameObject> GetMap() { return _Map; }

        public static IEnumerable<QuadTreeNode<KRGameObject>> FindPath(Vec2 start, Vec2 target) {
            return Pathfinding<QuadTreeNode<KRGameObject>>.FindPath(_Map.FindNode(start), _Map.FindNode(target))
				.Select(abstractNode => abstractNode.WrappedNode());
		}

		public static void UpdateGame() {
            foreach (KRGameObject unit in _Map) {
                FiniteStateMachine fsm = unit.GetComponent<FiniteStateMachine>();
                if (fsm != null) {
                    fsm.UpdateGame();
                }
            }
		}

		public static GameObject Find(Vec2 v) {
            KRGameObject u = _Map.Find(v);
			return (u == null) ? null : u.gameObject;
		}

		/// <summary>
		/// Find all gameObjects in the rect define by v1 and v2 and v3.
		/// </summary>
		public static IEnumerable<GameObject> Find(Vec2 v1, Vec2 v2, Vec2 v3) {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Vec2 v4 = v2 - (v3 - v1);

#if !UNITY_EDITOR
			__walkedNode.Clear();
			__walkedFind.Clear();
			__walkedFind.Add(v1);
			__walkedFind.Add(v2);
			__walkedFind.Add(v3);
			__walkedFind.Add(v4);
#endif

			// bottomLeft, bottomRight, topLeft, topRight
			Vec2[] vecs = { v1,v2,v3,v4 };
			Array.Sort<Vec2>(vecs);
			Vec2[] verts = { vecs[3], vecs[1], vecs[2], vecs[0] };
			Vec2 bottomLeft = vecs[0], bottomRight = vecs[2], topLeft = vecs[1], topRight = vecs[3];

			// boundBottomLeft, boundBottomRight, boundTopLeft, boundTopRight
			IEnumerable<int> xs = vecs.Select(v => v.X);
			IEnumerable<int> ys = vecs.Select(v => v.Y);

			int minXS = xs.Min();
			int maxXS = xs.Max();
			int minYS = ys.Min();
			int maxYS = ys.Max();

			Func<Vec2,Vec2,IList<Vec2>> _getLine = delegate(Vec2 orig, Vec2 dest) {
				IList<Vec2> d = new List<Vec2>();
				d.Add(orig);
				Bresenham.Line(orig.X, orig.Y, dest.X, dest.Y, delegate(int x, int y) {
					d.Add(new Vec2(x, y));
					return true;
				});
				return d;
			};

			IList<Vec2> bltp = _getLine(bottomLeft, topLeft);
			IList<Vec2> tltr = _getLine(topLeft, topRight);
			IList<Vec2> trbr = _getLine(topRight, bottomRight);
			IList<Vec2> brbl = _getLine(bottomRight, bottomLeft);

			// FIXME
			Func<Vec2,bool> _in = delegate(Vec2 v) {
				int x = v.X, y = v.Y;
				int ax = bottomLeft.X, bx = topLeft.X, dx = bottomRight.X;
				int ay = bottomLeft.Y, by = topLeft.Y, dy = bottomRight.Y;
				int ex=bx-ax, ey=by-ay, fx=dx-ax, fy=dy-ay;
				
				if ((x-ax) * ex + (y-ay) * ey < 0) return false;
				if ((x-bx) * ex + (y-by) * ey > 0) return false;
				if ((x-ax) * fx + (y-ay) * fy < 0) return false;
				if ((x-dx) * fx + (y-dy) * fy > 0) return false;
				
				return true;
			};
			Func<Vec2,bool> _on = delegate(Vec2 v) {
				return bltp.Contains(v) || tltr.Contains(v) || trbr.Contains(v) || brbl.Contains(v);
			};

			List<GameObject> gos = new List<GameObject>();

			KRGameObject u;
			for (int x = minXS; x < maxXS; ++x) {
				for (int y = minYS; y < maxYS; ++y) {
					Vec2 c = new Vec2(x,y);
					if (_in(c) || _on(c)) {
#if !UNITY_EDITOR
						__walkedNode.Add(c);
#endif
						u = _Map.Find(c);
						if (u != null) { gos.Add(u.gameObject); }
					}
				}
			}

			stopwatch.Stop();
			Debug.Log(String.Format("DragFind :: time elapsed: {0}", stopwatch.Elapsed));

			return gos;
		}

		public static IEnumerable<GameObject> Around(Vec2 v, int maxDist) {
			return _Map.ToList().Where(u => u.Pos.Dist(v) <= maxDist).Select(u => u.gameObject);
		}

#if !UNITY_EDITOR
		static void OnKRDrawGizmos() {
			Gizmos.color = Color.magenta;
			foreach (var x in __walkedNode) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one*.5f, Vector3.one);
			}

			Gizmos.color = Color.red;
			foreach (var x in __walkedFind) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one*.5f, Vector3.one);
			}
		}
#endif
	}
}
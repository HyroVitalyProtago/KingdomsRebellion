using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Inputs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KingdomsRebellion.KRDebug {

	public class DebugPathfinding : KRBehaviour {

		Stopwatch _stopwatch;

		Vec2 _begin, _end;
		QuadTreeNode<KRTransform> _beginNode, _endNode;
		IEnumerable<QuadTreeNode<KRTransform>> _waynodes;
		List<Vec2> _waypoints;
		List<Vec2> _realPath;

		void Awake() {
			On("OnLeftClickUp");
			On("OnRightClick");

			_stopwatch = new Stopwatch();
			_waypoints = new List<Vec2>();
			_realPath = new List<Vec2>();
		}

		static Vec2 PointBetweenQuadTreeNode(QuadTreeNode<KRTransform> src, QuadTreeNode<KRTransform> dst) {
			int x = -1, y = -1;

			switch (src.SideOfNeighbour(dst)) {
				case ESide.SOUTH: y = src.BottomLeft.Y; break;
				case ESide.NORTH: y = src.TopRight.Y-1; break;
				case ESide.EAST: x = src.TopRight.X-1; break;
				case ESide.WEST: x = src.BottomLeft.X; break;
			}

			if (x == -1) {
				x = (src.Width > dst.Width) ? dst.Pos.X : src.Pos.X;
			} else {
				y = (src.Height > dst.Height) ? dst.Pos.Y : src.Pos.Y;
			}

			return new Vec2(x,y);
		}

		List<Vec2> SimplifyPath(List<Vec2> waypoints) {
			List<Vec2> simplifiedPath = new List<Vec2>(waypoints);

			if (simplifiedPath.Count == 2) {
				return simplifiedPath;
			}
				
			bool canTraceStraightLine = true;
			do {
				Vec2 current = simplifiedPath[2];
				Bresenham.Line(_begin.X, _begin.Y, current.X, current.Y, delegate(int x, int y) {
					if (!KRFacade.IsEmpty(new Vec2(x,y))) {
						canTraceStraightLine = false;
						return false;
					}
					return true;
				});
				if (canTraceStraightLine) {
					simplifiedPath.RemoveAt(1);
				} else {
					return simplifiedPath;
				}
			} while(canTraceStraightLine && simplifiedPath.Count > 2);

			return simplifiedPath;
		}

		public void FindPath() {
			Debug.ClearDeveloperConsole();

			_stopwatch.Reset();
			_stopwatch.Start();
			_waynodes = KRFacade.FindPath(_begin, _end);

			_waypoints.Clear();
			_waypoints.Add(_begin);
			if (_waynodes.Any()) {
				_waypoints.Add(PointBetweenQuadTreeNode(_beginNode, _waynodes.ElementAt(0)));
				for (int i = 0; i < _waynodes.Count() - 1; ++i) {
					_waypoints.Add(PointBetweenQuadTreeNode(_waynodes.ElementAt(i), _waynodes.ElementAt(i + 1)));
				}
			}
			_waypoints.Add(_end);

			_realPath = SimplifyPath(_waypoints);

			_stopwatch.Stop();
			Debug.Log(String.Format("DebugPathfinding::FindPath | time elapsed: {0}", _stopwatch.Elapsed));
		}

		static void DrawNode(QuadTreeNode<KRTransform> n) {
			if (n.Width == 1) {
				Gizmos.DrawCube((new Vector3(n.Pos.X,0,n.Pos.Y)).Adjusted(), new Vector3(n.Width,.01f,n.Height));
			} else {
				Gizmos.DrawCube(new Vector3(n.Pos.X,0,n.Pos.Y), new Vector3(n.Width,.01f,n.Height));
			}
		}

		static void DrawPoint(Vec2 p) {
			Gizmos.DrawSphere(p.ToVector3().Adjusted(), .5f);
		}

		void DrawSelectedQuadTreeNodes() {
			Gizmos.color = new Color(0f,0f,1f,.5f);
			DrawNode(_beginNode);

			Gizmos.color = new Color(0f,0f,0f,.5f);
			_waynodes.ToList().ForEach(DrawNode);

			Gizmos.color = new Color(1f,0f,0f,.5f);
			DrawNode(_endNode);
		}

		void DrawWaypoints() {
			Gizmos.color = new Color(0f,1f,0f,.7f);
			_waypoints.ForEach(DrawPoint);
		}

		void DrawRealPath() {
			Gizmos.color = new Color(0f,1f,1f,1f);
			_realPath.ForEach(DrawPoint);

			Gizmos.color = Color.red;
			for (int i = 0; i < _realPath.Count-1; ++i)  {
				Gizmos.DrawLine(_realPath[i].ToVector3().Adjusted(), _realPath[i+1].ToVector3().Adjusted());
			}
		}

		void OnDrawGizmos() {
			if (_waynodes == null) {
				return;
			}

			DrawSelectedQuadTreeNodes();
			DrawWaypoints();
			DrawRealPath();
		}

		void OnLeftClickUp(Vector3 mousePosition) {
			_begin = InputModelAdapter.ModelPosition(mousePosition);
			_beginNode = KRFacade.FindNode(_begin);
			if (_begin != null && _end != null) { FindPath(); }
		}
		void OnRightClick(Vector3 mousePosition) {
			_end = InputModelAdapter.ModelPosition(mousePosition);
			_endNode = KRFacade.FindNode(_end);
			if (_begin != null && _end != null) { FindPath(); }
		}
	}
}
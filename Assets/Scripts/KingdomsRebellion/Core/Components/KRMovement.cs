using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
	public class KRMovement : KRBehaviour {

		[SerializeField]
		int _speed;
		public KRTransform Followee { get; private set; }

		public Vec2 Target { get; private set; }
		public int Speed { get { return _speed; } private set { _speed = value; } }

		KRTransform _krtransform;
		IList<Vec2> _waypoints;
		int _currentFrame;


		IList<Vec2> __way = new List<Vec2>();
		IEnumerable<QuadTreeNode<KRTransform>> __waynodes = new List<QuadTreeNode<KRTransform>>();


		void Awake() {
			_krtransform = GetComponent<KRTransform>();
			On("OnKRDrawGizmos");
		}

		void Start() {
			_currentFrame = 0;
			_waypoints = new List<Vec2>();
		}

		static Vec2 PointBetweenQuadTreeNode(QuadTreeNode<KRTransform> src, QuadTreeNode<KRTransform> dst) {
			int x = -1, y = -1;

			switch (src.SideOfNeighbour(dst)) {
			case ESide.SOUTH:
				y = src.BottomLeft.Y;
				break;
			case ESide.NORTH:
				y = src.TopRight.Y - 1;
				break;
			case ESide.EAST:
				x = src.TopRight.X - 1;
				break;
			case ESide.WEST:
				x = src.BottomLeft.X;
				break;
			}

			if (x == -1) {
				x = (src.Width > dst.Width) ? dst.Pos.X : src.Pos.X;
			} else {
				y = (src.Height > dst.Height) ? dst.Pos.Y : src.Pos.Y;
			}

			return new Vec2(x, y);
		}

		static List<Vec2> SimplifyPath(IList<Vec2> waypoints) {
			List<Vec2> simplifiedPath = new List<Vec2>(waypoints);

			if (simplifiedPath.Count <= 2) {
				return simplifiedPath;
			}

			var begin = waypoints[0];
			bool canTraceStraightLine = true;
			do {
				Vec2 current = simplifiedPath[2];
				Bresenham.Line(begin.X, begin.Y, current.X, current.Y, delegate(int x, int y) {
					if (!KRFacade.IsEmpty(new Vec2(x, y))) {
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

		public void UpdateGame() {
			if (Followee != null) {
				Target = Followee.Pos;
			}
			if (Target == null || _krtransform.Pos == Target) {
				Target = null;
				return;
			}

			if (_currentFrame > 0) {
				--_currentFrame;
				return;
			} else {
				_currentFrame = Speed;
			}

			__waynodes = KRFacade.FindPath(_krtransform.Pos, Target);

			_waypoints.Clear();
			_waypoints.Add(_krtransform.Pos);

			var beginNode = KRFacade.FindNode(_krtransform.Pos);
			QuadTreeNode<KRTransform> prev = null;
			foreach (var node in __waynodes) {
				if (prev != null) {
					_waypoints.Add(PointBetweenQuadTreeNode(prev, node));
					_waypoints.Add(PointBetweenQuadTreeNode(node, prev));
				} else {
					var p = PointBetweenQuadTreeNode(beginNode, node);
					if (!p.Equals(_krtransform.Pos)) {
						_waypoints.Add(p);
					}
					_waypoints.Add(PointBetweenQuadTreeNode(node, beginNode));
				}
				prev = node;
			}
			_waypoints.Add(Target);

			var _simplifiedPath = SimplifyPath(_waypoints);


			_simplifiedPath.RemoveAt(0);

			Vec2 nextPos = _krtransform.Pos;
			Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, _simplifiedPath[0].X, _simplifiedPath[0].Y, delegate(int x, int y) {
				nextPos = new Vec2(x, y);
				return false;
			});

			if (nextPos != _krtransform.Pos && KRFacade.IsEmpty(nextPos)) {
				KRFacade.Remove(_krtransform);
				_krtransform.Pos = nextPos;
				KRFacade.Add(_krtransform);

				__way.Add(_krtransform.Pos);
			} else {
//				_waypoints = null;
				Target = null;
			}
		}

		public bool HaveTarget() {
			return Target != null;
		}

		public void Move(Vec2 target) {
			if (_krtransform.Pos == target) {
				return;
			}
			if (Target == null) {
				_currentFrame = Speed;
			}
			Target = target;

			__way.Clear();
			__way.Add(_krtransform.Pos);
//			__waynodes.Clear();
		}

		public void Follow(GameObject kgo) {
			Follow(kgo.GetComponent<KRTransform>());
		}

		public void Follow(KRTransform kt) {
			Followee = kt;
			Move(kt.Pos);
		}

		void OnKRDrawGizmos() {
			__waynodes.ToList().ForEach(delegate(QuadTreeNode<KRTransform> n) {
				Gizmos.color = new Color(((float)n.Width) / 10f, .2f, ((float)n.Height) / 10f, .6f);
				if (n.Width == 1)
					Gizmos.DrawCube((new Vector3(n.Pos.X, 0, n.Pos.Y)).Adjusted(), new Vector3(n.Width, .01f, n.Height));
				else
					Gizmos.DrawCube(new Vector3(n.Pos.X, 0, n.Pos.Y), new Vector3(n.Width, .01f, n.Height));
			});

			foreach (var v in _waypoints) {
				Gizmos.color = new Color(0f,1f,1f,1f);
				Gizmos.DrawSphere(v.ToVector3().Adjusted(), .5f);
			}

			for (int i = 0; i < __way.Count - 1; ++i) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine(__way[i].ToVector3().Adjusted(), __way[i + 1].ToVector3().Adjusted());
			}
		}
	}
}

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
		IEnumerable<QuadTreeNode<KRTransform>> _waypoints;
		int _currentFrame;


		IList<Vec2> __way = new List<Vec2>();
		List<QuadTreeNode<KRTransform>> __waynodes = new List<QuadTreeNode<KRTransform>>();


		void Awake() {
			_krtransform = GetComponent<KRTransform>();
//			Speed = __speed;

			On("OnKRDrawGizmos");
		}

		void Start() {
			_currentFrame = 0;
		}

		Vec2 PointBetweenQuadTreeNode(QuadTreeNode<KRTransform> src, QuadTreeNode<KRTransform> dst) {

			UnityEngine.Debug.Log("PointBetweenQuadTreeNode");
			UnityEngine.Debug.Log(src.BottomLeft+" ; "+src.TopRight);
			UnityEngine.Debug.Log(dst.BottomLeft+" ; "+dst.TopRight);

			int x = -1, y = -1;
			if (dst.BottomLeft.X < src.BottomLeft.X) {
				x = src.BottomLeft.X-1;
			} else if (dst.TopRight.X >= src.TopRight.X) {
				x = src.TopRight.X;
			} else if (dst.BottomLeft.Y < src.BottomLeft.Y) {
				y = src.BottomLeft.Y-1;
			} else if (dst.TopRight.Y >= src.TopRight.Y) {
				y = src.TopRight.Y;
			}

			if (x == -1) {
				x = (src.Width > dst.Width) ? dst.Pos.X : src.Pos.X;
			} else {
				y = (src.Height > dst.Height) ? dst.Pos.Y : src.Pos.Y;
			}

			UnityEngine.Debug.Log("Result: ("+x+", "+y+")");

			return new Vec2(x,y);
		}

		Vec2 SimplifyPath() {
			QuadTreeNode<KRTransform> currentNode = KRFacade.FindNode(_krtransform.Pos);
			QuadTreeNode<KRTransform> node;
			Vec2 nextWaypoint, waypoint;

			bool canTraceStraightLine = true;
			do {
				node = _waypoints.ElementAtOrDefault(1);
//				nextWaypoint = (node == null) ? Target : node.Pos;
				nextWaypoint = (node == null) ? Target : PointBetweenQuadTreeNode(currentNode, node);
				Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextWaypoint.X, nextWaypoint.Y, delegate(int x, int y) {
					if (!KRFacade.IsEmpty(new Vec2(x,y))) {
						canTraceStraightLine = false;
//						if (nextWaypoint == Target) { nextWaypoint = _waypoints.First().Pos; }
						return false;
					}
					return true;
				});
				if (canTraceStraightLine) {
					if (nextWaypoint != Target) { _waypoints = _waypoints.Skip(1); }
				} else {
					node = _waypoints.FirstOrDefault();
//					nextWaypoint = (node == null) ? Target : node.Pos;
					nextWaypoint = (node == null) ? Target : PointBetweenQuadTreeNode(currentNode, node);
				}
			} while(canTraceStraightLine && nextWaypoint != Target);

			return nextWaypoint;
		}

		public void UpdateGame() {
			if (Followee != null) { Target = Followee.Pos; }
			if (Target == null) { return; }
			if (_krtransform.Pos == Target) {
				Target = null;
//				Followee = null; // TEST
				return;
			}

			if (_currentFrame > 0) {
				--_currentFrame;
				return;
			} else {
				_currentFrame = Speed;
			}

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			if (_waypoints == null) {
				_waypoints = KRFacade.FindPath(_krtransform.Pos, Target);
				__waynodes.AddRange(_waypoints.Where (w => !__waynodes.Contains (w)));
			}

			Vec2 nextWaypoint;
			if (!_waypoints.Any()) {
				nextWaypoint = Target;
			} else {
				nextWaypoint = SimplifyPath();
				UnityEngine.Debug.Log(nextWaypoint);
			}

			Vec2 nextPos = _krtransform.Pos;
			Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextWaypoint.X, nextWaypoint.Y, delegate(int x, int y) {
				nextPos = new Vec2(x,y);
				return false;
			});

			stopwatch.Stop();
//			UnityEngine.Debug.Log(String.Format("KRMovement :: time elapsed: {0}", stopwatch.Elapsed));

			if (nextPos != _krtransform.Pos && KRFacade.IsEmpty(nextPos)) {
				KRFacade.Remove(_krtransform);
				_krtransform.Pos = nextPos;
				KRFacade.Add(_krtransform);

				__way.Add(_krtransform.Pos);
			} else {
				_waypoints = null;
				Target = null;
			}
		}

		public bool HaveTarget() {
			return Target != null;
		}

		public void Move(Vec2 target) {
			if (_krtransform.Pos == target) { return; }
			if (Target == null) { _currentFrame = Speed; }
			Target = target;

			__way.Clear();
			__way.Add(_krtransform.Pos);
			__waynodes.Clear();
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
				Gizmos.color = new Color(((float)n.Width)/10f, .2f, ((float)n.Height)/10f, .1f);
				if (n.Width == 1)
					Gizmos.DrawCube((new Vector3(n.Pos.X,0,n.Pos.Y)).Adjusted(), new Vector3(n.Width,.01f,n.Height));
				else
					Gizmos.DrawCube(new Vector3(n.Pos.X,0,n.Pos.Y), new Vector3(n.Width,.01f,n.Height));
			});

			Gizmos.color = Color.red;
			for (int i = 0; i < __way.Count-1; ++i)  {
				Gizmos.DrawLine(__way[i].ToVector3().Adjusted(), __way[i+1].ToVector3().Adjusted());
			}
		}
	}
}

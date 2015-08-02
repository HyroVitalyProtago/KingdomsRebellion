using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
	public class KRMovement : KRBehaviour {

		public int __speed;
		
		public KRTransform Followee { get; private set; }
		public Vec2 Target { get; private set; }
		public int Speed { get; private set; }

		KRTransform _krtransform;
		IEnumerable<QuadTreeNode<KRTransform>> _waypoints;
		int _currentFrame;

#if UNITY_EDITOR
		IList<Vec2> __way = new List<Vec2>();
		List<QuadTreeNode<KRTransform>> __waynodes = new List<QuadTreeNode<KRTransform>>();
#endif

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
			Speed = __speed;
		}

		void Start() {
			_currentFrame = 0;
		}

		public void UpdateGame() {
			if (Followee != null) { Target = Followee.Pos; }
			if (Target == null || _krtransform.Pos == Target) { return; }

			if (_currentFrame > 0) {
				--_currentFrame;
				return;
			} else {
				_currentFrame = Speed;
			}

			_waypoints = KRFacade.FindPath(_krtransform.Pos, Target);
			__waynodes.AddRange(_waypoints.Where(w => !__waynodes.Contains(w)));

			if (_waypoints == null) {
				Target = null;
			} else {
				QuadTreeNode<KRTransform> node;
				Vec2 nextPos = _krtransform.Pos;

				if (!_waypoints.Any()) {
					nextPos = Target;
				} else {
					bool canTraceStraightLine = true;
					do {
						node = _waypoints.ElementAtOrDefault(1);
						nextPos = (node == null) ? Target : node.Pos;
						Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextPos.X, nextPos.Y, delegate(int x, int y) {
							if (!KRFacade.GetMap().IsEmpty(new Vec2(x,y))) {
								canTraceStraightLine = false;
								if (nextPos == Target) { nextPos = _waypoints.First().Pos; }
								return false;
							}
							return true;
						});
						if (canTraceStraightLine) {
							if (nextPos != Target) { _waypoints = _waypoints.Skip(1); }
						} else {
							node = _waypoints.FirstOrDefault();
							nextPos = (node == null) ? Target : node.Pos;
						}
					} while(canTraceStraightLine && nextPos != Target);

				}

				var lt = new List<Vec2>();

				Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextPos.X, nextPos.Y, delegate(int x, int y) {
					if (lt.Count == 0) {
						nextPos = new Vec2(x,y);
					}
					lt.Add(new Vec2(x,y));
					return true;
				});
				if (nextPos != _krtransform.Pos && KRFacade.GetMap().IsEmpty(nextPos)) {
					KRFacade.GetMap().Remove(GetComponent<KRTransform>());
					_krtransform.Pos = nextPos;
					KRFacade.GetMap().Add(GetComponent<KRTransform>());
#if UNITY_EDITOR
					__way.Add(_krtransform.Pos);
#endif
				} else {
					_waypoints = null;
					Target = null;
					Followee = null;
				}
			}
		}

		public bool HaveTarget() {
			return Target != null;
		}

		public void Move(Vec2 target) {
			if (_krtransform.Pos == target) { return; }
			if (Target == null) { _currentFrame = Speed; }
			Target = target;

#if UNITY_EDITOR
			__way.Clear();
			__way.Add(_krtransform.Pos);
			__waynodes.Clear();
#endif
		}

		public void Follow(GameObject kgo) {
			Follow(kgo.GetComponent<KRTransform>());
		}

		public void Follow(KRTransform kt) {
			Followee = kt;
			Move(kt.Pos);
		}

#if UNITY_EDITOR
		void OnDrawGizmos() {
			__waynodes.ToList().ForEach(delegate(QuadTreeNode<KRTransform> n) {
				Gizmos.color = new Color(((float)n.Width)/10f, .2f, ((float)n.Height)/10f);
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
#endif
	}
}

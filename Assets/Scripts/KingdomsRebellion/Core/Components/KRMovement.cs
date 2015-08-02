using KingdomsRebellion.Core;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;
using UnityEngine;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
	public class KRMovement : KRBehaviour {

		public int __speed;
		
		public GameObject Follow { get; set; }
		public Vec2 Target { get; set; }
		public int Speed { get; private set; }

		KRTransform _krtransform;
		IEnumerable<QuadTreeNode<KRGameObject>> _waypoints;
		int _currentFrame;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
			Follow = null;
			Target = null;
			Speed = __speed;
		}

		void Start() {
			_currentFrame = 0;
		}

		int _test;
		public void UpdateGame() {
			if (Target == null || _krtransform.Pos == Target) { return; }

			if (_currentFrame > 0) {
				--_currentFrame;
				return;
			} else {
				_currentFrame = Speed;
			}

			_waypoints = KRFacade.FindPath(_krtransform.Pos, Target);
			_waypoints.ToList().ForEach(delegate(QuadTreeNode<KRGameObject> w) { if (!test3.Contains(w)) test3.Add(w); });
			if (_waypoints == null) {
				Target = null;
			} else {
				QuadTreeNode<KRGameObject> node;
				Vec2 nextPos = _krtransform.Pos;

				if (!_waypoints.Any()) {
					nextPos = Target;
				} else {

					int guard = 1000;
					// TEST
					bool canTraceStraightLine = true;
					do {
						if (--guard < 0) { Debug.LogError("Infinite loop"); break; }
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
				test2.Add(lt);

				Bresenham.Line(_krtransform.Pos.X, _krtransform.Pos.Y, nextPos.X, nextPos.Y, delegate(int x, int y) {
					if (lt.Count == 0) {
						nextPos = new Vec2(x,y);
					}
					lt.Add(new Vec2(x,y));
					return true;
				});
				if (nextPos != _krtransform.Pos && KRFacade.GetMap().IsEmpty(nextPos)) {
					KRFacade.GetMap().Remove(GetComponent<KRGameObject>());
					_krtransform.Pos = nextPos;
					KRFacade.GetMap().Add(GetComponent<KRGameObject>());
					test.Add(_krtransform.Pos);
				} else {
					_waypoints = null;
					Target = null;
					Follow = null;
				}
			}
		}

		IList<Vec2> test = new List<Vec2>();
		List<List<Vec2>> test2 = new List<List<Vec2>>();
		List<QuadTreeNode<KRGameObject>> test3 = new List<QuadTreeNode<KRGameObject>>();

		public void Move(int player, Vec3 targetv3) {
			Vec2 target = targetv3.ToVec2();

			if (_krtransform.PlayerID != player || _krtransform.Pos == target) { return; }
			if (Target == null) { _test = 8; }
			Target = target;

			test.Clear();
			test.Add(_krtransform.Pos);
			test2.Clear();
			test3.Clear();
		}

		void OnDrawGizmos() {
			test3.ToList().ForEach(delegate(QuadTreeNode<KRGameObject> n) {
				Gizmos.color = new Color(((float)n.Width)/10f, .2f, ((float)n.Height)/10f);
				if (n.Width == 1)
					Gizmos.DrawCube((new Vector3(n.Pos.X,0,n.Pos.Y)).Adjusted(), new Vector3(n.Width,.01f,n.Height));
				else
					Gizmos.DrawCube(new Vector3(n.Pos.X,0,n.Pos.Y), new Vector3(n.Width,.01f,n.Height));
			});

			Gizmos.color = Color.gray;
			test2.ToList().ForEach(delegate(List<Vec2> l) {
				for (int i = 0; i < l.Count()-1; ++i) {
					Gizmos.DrawLine(l[i].ToVector3().Adjusted(), l[i+1].ToVector3().Adjusted());
				}
			});

			Gizmos.color = Color.red;
			for (int i = 0; i < test.Count()-1; ++i)  {
				Gizmos.DrawLine(test[i].ToVector3().Adjusted(), test[i+1].ToVector3().Adjusted());
			}
		}
	}
}

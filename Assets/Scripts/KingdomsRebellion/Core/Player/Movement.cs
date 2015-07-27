using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.AI;
using System.Linq;
using KingdomsRebellion.Core.Map;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.Core.Player {

	static class Tools {
		static Vector3 offset = new Vector3(.5f,0,.5f);
		public static Vector3 Adjusted(this Vector3 v) {
			return v + offset;
		}
	}

	public class Movement : KRBehaviour, IPos {

		Unit _unit;
		Vec2 _target;
		Vec2 _pos;
		public Vec2 Pos {
			get {
				return _pos;
			}
			private set {
				_pos = value;
				transform.position = _pos.ToVector3().Adjusted();
			}
		}
		IEnumerable<QuadTreeNode<Unit>> _waypoints;

		void Awake() {
			Pos = Vec2.FromVector3(transform.position);
		}

		void Start() {
			_unit = GetComponent<Unit>();
			_target = null;

			_test = -1;
		}

		int _test;
		public void UpdateGame() {
			if (_target == null || Pos == _target) { return; }

			if (_test > 0) {
				--_test;
				return;
			} else {
				_test = 8;
			}

			_waypoints = KRFacade.FindPath(Pos, _target);
			_waypoints.ToList().ForEach(delegate(QuadTreeNode<Unit> w) { if (!test3.Contains(w)) test3.Add(w); });
			if (_waypoints == null) {
				_target = null;
			} else {
				QuadTreeNode<Unit> node;
				Vec2 nextPos = Pos;

				if (_waypoints.Count() == 0) {
					nextPos = _target;
				} else {

					int guard = 1000;
					// TEST
					bool canTraceStraightLine = true;
					do {
						if (--guard < 0) { Debug.LogError("Infinite loop"); break; }
						node = _waypoints.ElementAtOrDefault(1);
						nextPos = (node == null) ? _target : node.Pos;
						Bresenham.Line(Pos.X, Pos.Y, nextPos.X, nextPos.Y, delegate(int x, int y) {
							if (!KRFacade.GetMap().IsEmpty(new Vec2(x,y))) {
								canTraceStraightLine = false;
								if (nextPos == _target) { nextPos = _waypoints.First().Pos; }
								return false;
							}
							return true;
						});
						if (canTraceStraightLine) {
							if (nextPos != _target) { _waypoints = _waypoints.Skip(1); }
						} else {
							node = _waypoints.FirstOrDefault();
							nextPos = (node == null) ? _target : node.Pos;
						}
					} while(canTraceStraightLine && nextPos != _target);

				}

				var lt = new List<Vec2>();
				test2.Add(lt);

				Bresenham.Line(Pos.X, Pos.Y, nextPos.X, nextPos.Y, delegate(int x, int y) {
					if (lt.Count == 0) {
						nextPos = new Vec2(x,y);
					}
					lt.Add(new Vec2(x,y));
					return true;
				});
				if (nextPos != Pos && KRFacade.GetMap().IsEmpty(nextPos)) {
					KRFacade.GetMap().Remove(GetComponent<Unit>());
					Pos = nextPos;
					KRFacade.GetMap().Add(GetComponent<Unit>());
					test.Add(Pos);
				} else {
					_waypoints = null;
				}
			}
		}

		IList<Vec2> test = new List<Vec2>();
		List<List<Vec2>> test2 = new List<List<Vec2>>();
		List<QuadTreeNode<Unit>> test3 = new List<QuadTreeNode<Unit>>();

	    public void Move(int player, Vec3 targetv3) {
			Vec2 target = targetv3.ToVec2();
			if (_unit.playerId != player || Pos == target) { return; }
			if (_target == null) { _test = 8; }
			_target = target;

			test.Clear();
			test.Add(Pos);
			test2.Clear();
			test3.Clear();
		}

		[ExecuteInEditMode]
		void OnDrawGizmos() {
			test3.ToList().ForEach(delegate(QuadTreeNode<Unit> n) {
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
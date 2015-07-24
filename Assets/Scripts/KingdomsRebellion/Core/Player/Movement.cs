using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.AI;
using System.Linq;

namespace KingdomsRebellion.Core.Player {

	public class Movement : KRBehaviour {

		AbstractGrid _grid;
		Unit _unit;
		Vec2 _target;
		Vec2 _pos;
		IEnumerable<Node> _waypoints;

		void Start() {
			_unit = GetComponent<Unit>();
			_grid = KRFacade.GetGrid();
			_pos = _grid.GetPositionOf(gameObject);
			_target = null;

			_test = -1;
		}
	
		void Update() {
			transform.position = _pos.ToVector3();
		}

		int _test;
		public void UpdateGame() {
			if (_target == null || _pos == _target) { return; }

//			if (_waypoints == null) {
//				Debug.Log("FindPath from " + _pos + " to " + _target);
				_waypoints = Pathfinding.FindPath(_pos, _target);
//				Debug.Log("Path found !");
//				if (_waypoints.Count() <= 1) {
//					_target = null;
//				}
//			}

			if (_waypoints.Count() > 0) {
				if (_test > 0) {
					--_test;
					return;
				} else {
					_test = 8;
				}
				Vec2 nextPos = _waypoints.First().Pos;
				Debug.Log("current " + _pos);
				Debug.Log("next" + nextPos);
				if (_grid.Move(gameObject, nextPos)) {
					_pos = nextPos;
				} else {
					_waypoints = null;
//					Debug.Assert(false);
				}
			} else {
				_target = null;
			}
		}

	    public void Move(int player, Vec3 targetv3) {
			Vec2 target = targetv3.ToVec2();
			if (_unit.playerId != player || _pos == target) { return; }
			_target = target;
			_test = 8;
		}
	}
}
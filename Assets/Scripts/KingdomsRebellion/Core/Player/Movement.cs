using UnityEngine;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.Core.Player {

	public class Movement : KRBehaviour {

		AbstractGrid _grid;
		Unit _unit;
		Vec2 _target;
        Vec2 _pos;

		void Start() {
			_unit = GetComponent<Unit>();
			_grid = KRFacade.GetGrid();
			_pos = _grid.GetPositionOf(gameObject);
			_target = null;
		}
	
		void Update() {
			transform.position = _pos.ToVector3();
		}

		public void UpdateGame() {
			if (_target == null || _pos == _target) { return; }

			// TODO Calculate path

			Vec2 nextPos = _pos;
			int dx = _target.X - _pos.X;
			int dy = _target.Y - _pos.Y;
			dx = dx == 0 ? 0 : dx > 0 ? 1 : -1;
			dy = dy == 0 ? 0 : dy > 0 ? 1 : -1;
			nextPos += new Vec2(dx, dy);
			if (_grid.Move(gameObject, nextPos)) {
				_pos = nextPos;
			}
		}

	    public void Move(int player, Vec3 targetv3) {
			Vec2 target = targetv3.ToVec2();
			if (_unit.playerId != player || _pos == target) {
				return;
			}

			GameObject go = _grid.GetGameObjectByPosition(target);
			Unit ennemy = (go == null) ? null : go.GetComponent<Unit>();
			if (ennemy != null && ennemy.playerId != player) {
				Debug.Log("@NotImplemented attack...");
			} else {
				_target = target;
			}
		}
	}
}
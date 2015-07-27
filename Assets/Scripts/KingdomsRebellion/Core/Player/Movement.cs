using UnityEngine;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.Core.Player {

	public class Movement : KRBehaviour {

		AbstractGrid _grid;
		Unit _unit;
        public Vec2 Target { get; private set; }
        public Vec2 Pos { get; private set; }

		void Start() {
			_unit = GetComponent<Unit>();
			_grid = KRFacade.GetGrid();
			Pos = _grid.GetPositionOf(gameObject);
			Target = null;
		}
	
		void Update() {
			transform.position = Pos.ToVector3();
		}

		public void UpdateGame() {
			//if (Target == null || Pos == Target) { return; }

			// TODO Calculate path

			Vec2 nextPos = Pos;
			int dx = Target.X - Pos.X;
			int dy = Target.Y - Pos.Y;
			dx = dx == 0 ? 0 : dx > 0 ? 1 : -1;
			dy = dy == 0 ? 0 : dy > 0 ? 1 : -1;
			nextPos += new Vec2(dx, dy);
		    if (_grid.Move(gameObject, nextPos)) {
		        Pos = nextPos;
		    } else {
		        Target = null;
		    }
		}

	    public void Move(int player, Vec3 targetv3) {
			Vec2 target = targetv3.ToVec2();
			if (_unit.PlayerId != player || Pos == target) {
				return;
			}
			Target = target;
		}

	    public void Follow(GameObject target) {
	        Target = Vec2.FromVector3(target.transform.position);
	    }
	}
}
using UnityEngine;
using System;
using System.Collections.Generic;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Grid {
	public class FlatGrid : AbstractGrid {

		private readonly GameObject[] _grid;
		private int _xSquareNumber;
		private int _ySquareNumber;

		public FlatGrid(int xSquareNumber, int ySquareNumber) {
			_xSquareNumber = xSquareNumber;
			_ySquareNumber = ySquareNumber;
			_grid = new GameObject[_xSquareNumber * _ySquareNumber];
		}

		public override void Add(GameObject go, Vec2 pos) {
			Add(go.GetComponent<Unit>(), pos);
		}

		void Add(Unit unit, Vec2 pos) {
			unit.SetProperty("position", pos);
			_grid[ValidPosition(pos.Y * _xSquareNumber + pos.X)] = unit.gameObject;
		}

		public override bool Remove(GameObject go) {
			return Remove(go.GetComponent<Unit>());
		}

		bool Remove(Unit unit) {
			if (unit == null) return false;
			Vec2 unitPos = unit.GetProperty<Vec2>("position");
			_grid[ValidPosition(unitPos.Y * _xSquareNumber + unitPos.X)] = null;
			return true;
		}

		public override bool Move(GameObject go, Vec2 newPosition) {
			return Move(go.GetComponent<Unit>(), newPosition);
		}

		bool Move(Unit unit, Vec2 newPosition) {
			if (unit != null && IsEmpty(unit.GetProperty<Vec2>("position")) && Remove(unit.gameObject)) {
				Add(unit.gameObject, newPosition);
				return true;
			}
			return false;
		}

		public override GameObject GetGameObjectByPosition(Vec2 position) {
			return _grid[ValidPosition(position.Y * _xSquareNumber + position.X)];
		}

		public override bool IsInBounds(int position) {
			return position < _grid.Length;
		}

		public override List<GameObject> GetNearGameObjects(Vec2 position) {
		    List<Vec2> list = GetVec2Between(position + new Vec2(1, 1), position - new Vec2(1, 1));
            List<GameObject> goList = new List<GameObject>();
		    foreach (var pos in list) {
		        try {
		            goList.Add(GetGameObjectByPosition(pos));
                } catch (IndexOutOfRangeException) {}
		    }
		    return goList;
		}

	    public Vec2 GetVec2FromGridPosition(int position) {
	        return new Vec2(position % _xSquareNumber, position / _xSquareNumber);
	    }
	}
}

using UnityEngine;
using System;
using System.Collections.Generic;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Grid {
	public class FlatGrid : AbstractGrid {

		readonly GameObject[] _grid;
	    Dictionary<Unit, int> _objects; 
		int _xSquareNumber;
		int _ySquareNumber;

		public FlatGrid(int xSquareNumber, int ySquareNumber) {
			_xSquareNumber = xSquareNumber;
			_ySquareNumber = ySquareNumber;
			_grid = new GameObject[_xSquareNumber * _ySquareNumber];
            _objects = new Dictionary<Unit, int>();
		}

		public override void Add(GameObject go, Vec2 pos) {
			Add(go.GetComponent<Unit>(), pos, false);
		}

		void Add(Unit unit, Vec2 modelPos, bool isIntern) {
		    int pos = ValidPosition(modelPos.Y*_xSquareNumber + modelPos.X);
			_grid[pos] = unit.gameObject;
		    bool keyExist = _objects.ContainsKey(unit);
		    if (!keyExist && !isIntern) {
		        _objects.Add(unit, pos);
		    } else if (keyExist && isIntern) {
		        _objects[unit] = pos;
		    }
		}

		public override bool Remove(GameObject go) {
			return Remove(go.GetComponent<Unit>(), false);
		}

		bool Remove(Unit unit, bool isIntern) {
			if (unit == null) return false;
			Vec2 unitPos = GetPositionOf(unit.gameObject);
			_grid[ValidPosition(unitPos.Y * _xSquareNumber + unitPos.X)] = null;
            bool keyExist = _objects.ContainsKey(unit);
		    if (!keyExist) {
		        return false;
		    }
		    if (!isIntern) {
		        _objects.Remove(unit);
		    }
			return true;
		}

		public override bool Move(GameObject go, Vec2 newPosition) {
			return Move(go.GetComponent<Unit>(), newPosition);
		}

		bool Move(Unit unit, Vec2 newPosition) {
			if (unit != null && IsEmpty(newPosition) && Remove(unit, true)) {
				Add(unit, newPosition, true);
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

		public override List<GameObject> GetNearGameObjects(Vec2 position, int range) {
		    List<Vec2> list = GetVec2Between(position + new Vec2(range, range), position - new Vec2(range, range));
            List<GameObject> goList = new List<GameObject>();
		    foreach (var pos in list) {
		        if (pos == position) continue;
		        try {
		            GameObject go = GetGameObjectByPosition(pos);
		            if (go == null) continue;
		            goList.Add(go);
                } catch (IndexOutOfRangeException) {}
		    }
		    return goList;
		}

	    public Vec2 GetVec2FromGridPosition(int position) {
	        return new Vec2(position % _xSquareNumber, position / _xSquareNumber);
	    }

	    public override Dictionary<Unit, int> GetGameObjects() {
	        return _objects;
	    }

	    public override Vec2 GetPositionOf(GameObject go) {
	        Unit unit = go.GetComponent<Unit>();
	        if (_objects.ContainsKey(unit)) {
	            return GetVec2FromGridPosition(_objects[unit]);
	        }
	        return null;
	    }
	}
}

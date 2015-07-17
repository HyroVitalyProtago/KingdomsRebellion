using UnityEngine;
using System;
using System.Collections.Generic;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Grid {
	public class FlatGrid : AbstractGrid {

		private readonly GameObject[] _grid;
		private int _xSquareNumber;
		private int _ySquareNumber;

		public FlatGrid() {
			_xSquareNumber = 256;
			_ySquareNumber = 256;
			_grid = new GameObject[_xSquareNumber * _ySquareNumber];
		}

		public FlatGrid(int xSquareNumber, int ySquareNumber) {
			_xSquareNumber = xSquareNumber;
			_ySquareNumber = ySquareNumber;
			_grid = new GameObject[_xSquareNumber * _ySquareNumber];
		}

		public override void Add(GameObject go, Vec2 position) {
			_grid[ValidPosition(position.Y * _xSquareNumber + position.X)] = go;
		}

		public override bool Remove(GameObject go) {
			Unit unit = go.GetComponent<Unit>();
			if (unit != null) {
				_grid[ValidPosition(unit.Position.Y * _xSquareNumber + unit.Position.X)] = null;
				return true;
			}
			return false;
		}

		public override bool Move(GameObject go, Vec2 newPosition) {
			Unit unit = go.GetComponent<Unit>();
			if (unit != null) {
				if (IsEmpty(unit.Position) && Remove(go)) {
					Add(go, newPosition);
					return true;
				} 
			}
			return false;
		}

		public override GameObject GetGameObjectByPosition(Vec2 position) {
			return _grid[ValidPosition(position.Y * _xSquareNumber + position.X)];
		}

		public override bool IsInBounds(int position) {
			return position < _grid.Length;
		}

		public override Vec2[] GetNearGameObjects(Vec2 position) {
			int square = _xSquareNumber * position.Y + position.X;
			if (IsInBounds(square)) {
			}
			throw new NotImplementedException();
		}
	}
}

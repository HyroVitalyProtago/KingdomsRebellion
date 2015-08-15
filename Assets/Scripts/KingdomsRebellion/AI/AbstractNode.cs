using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using System.Linq;

namespace KingdomsRebellion.AI {
	public abstract class AbstractNode : IComparable<AbstractNode>, IPos {
		enum EState {
			OPENED,
			CLOSED,
			UNVISITED
		}

		#region Attributes

		int _totalCost, _pathCost, _estimatedCost;
		readonly IList<AbstractNode> _neighbours;
		EState _state;

		#endregion

		#region Properties

		public AbstractNode Parent { get; set; }

		public int TotalCost { get { return _totalCost; } }

		// FCost : Total cost
		public int PathCost { // GCost : Path cost from the starting point
			get {
				return _pathCost;
			}
			set {
				_pathCost = value;
				UpdateTotal();
			}
		}

		public int EstimatedCost { // HCost : Estimated path cost to the goal
			get {
				return _estimatedCost;
			}
			set {
				_estimatedCost = value;
				UpdateTotal();
			}
		}
			
		public Vec2 Pos { get; private set; }

		#endregion

		protected AbstractNode(Vec2 pos) {
			Pos = pos;
			_state = EState.UNVISITED;
			_neighbours = new List<AbstractNode>();
		}

		#region State

		public void Close() {
			_state = EState.CLOSED;
		}

		public bool IsClosed() {
			return _state == EState.CLOSED;
		}

		public void Open() {
			_state = EState.OPENED;
		}

		public bool IsOpened() {
			return _state == EState.OPENED;
		}

		public void Reset() {
			_state = EState.UNVISITED;

			_neighbours.Clear();
			foreach (var neighbour in Neighbours()) {
				neighbour._neighbours.Clear();
			}
		}

		#endregion

		#region Abstract

		public abstract bool IsFree();

		public IEnumerable<AbstractNode> Neighbours() {
			return _neighbours.Concat(RealNeighbours());
		}

		protected abstract IEnumerable<AbstractNode> RealNeighbours();

		#endregion

		void UpdateTotal() {
			_totalCost = _pathCost + _estimatedCost;
		}

		// Heuristic
		// @see http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
		public int GetDistance(AbstractNode other) {
			int dx = Mathf.Abs(Pos.X - other.Pos.X);
			int dy = Mathf.Abs(Pos.Y - other.Pos.Y);
			return (dx > dy) ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
		}

		public void Link() {
			foreach (var neighbour in Neighbours()) {
				neighbour._neighbours.Add(this);
			}
		}

		#region Equals

		public int CompareTo(AbstractNode other) {
			int result = TotalCost.CompareTo(other.TotalCost);
			return result != 0 ? result : EstimatedCost.CompareTo(other.EstimatedCost);
		}

		public override bool Equals(Object obj) {
			return this == obj;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		#endregion
	}
}
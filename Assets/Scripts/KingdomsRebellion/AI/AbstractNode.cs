using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Interfaces;
using UnityEngine;
using Object = System.Object;

namespace KingdomsRebellion.AI {
	public abstract class AbstractNode<T> : IComparable where T : IPos {
		int _totalCost, _pathCost, _estimatedCost;

		public AbstractNode<T> Parent { get; set; }
		public int TotalCost { get { return _totalCost; } } // FCost : Total cost
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

		protected AbstractNode(AbstractNode<T> parent, int path, int estimate) {
			Parent = parent;
			PathCost = path;
			EstimatedCost = estimate;
		}

		public abstract bool IsFree();
		public abstract IEnumerable<AbstractNode<T>> Neighbours();

		void UpdateTotal() { _totalCost = _pathCost + _estimatedCost; }

		public int CompareTo(object obj) {
			AbstractNode<T> temp = obj as AbstractNode<T>;
			if (temp == null) { throw new ArgumentException("Object is not an AbstractNode"); }
			
			int result = TotalCost.CompareTo(temp.TotalCost);
			if (result != 0) { return result; }
			
			return EstimatedCost.CompareTo(temp.EstimatedCost);
		}

		// Heuristic
		// @see http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
		public int GetDistance(AbstractNode<T> node) {
			int dx = Mathf.Abs(WrappedNode().Pos.X - node.WrappedNode().Pos.X);
			int dy = Mathf.Abs(WrappedNode().Pos.Y - node.WrappedNode().Pos.Y);
			
			if (dx > dy) { return 14*dy + 10*(dx-dy); }
			return 14*dx + 10*(dy-dx);
		}

		public abstract T WrappedNode();

		public static bool operator ==(AbstractNode<T> a, AbstractNode<T> b) {
			if (((object)a == null) && ((object)b == null)) { return true; }
			if (((object)a == null) || ((object)b == null)) { return false; }
			return ReferenceEquals(a.WrappedNode(), b.WrappedNode());
		}
		public static bool operator !=(AbstractNode<T> a, AbstractNode<T> b) { return !(a == b); }

		public override bool Equals(Object obj) {
			AbstractNode<T> p = obj as AbstractNode<T>;
			if ((object)p == null) { return false; }
			return this == p;
		}
		public override int GetHashCode() {
			return WrappedNode().GetHashCode();
		}
	}
}
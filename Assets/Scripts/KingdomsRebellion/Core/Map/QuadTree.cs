using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public class QuadTree<T> : IMap<QuadTreeNode<T>,T> where T : IPos, HaveRadius {

		QuadTreeNode<T> _node;
		IList<T> _objects;

		public QuadTree(int width, int height) {
			_node = new QuadTreeNode<T>(0, 0, width, height);
			_objects = new List<T>();
		}

		public bool Add(T t) {
			if (!IsInBounds(t.Pos)) {
				throw new ArgumentException("Position is not in bounds : " + t.Pos);
			}
			if (_node.Add(t)) {
				_objects.Add(t);
				return true;
			}
			return false;
		}

		public bool Remove(T t) {
			if (_node.Remove(t)) {
				_objects.Remove(t);
				return true;
			}
			return false;
		}

		public AbstractNode<QuadTreeNode<T>> FindNode(Vec2 pos) {
			if (!IsInBounds(pos)) {
				return null;
			}
			return QuadTreeNodeWrapper<T>.Wrap(_node.Find(pos));
		}

		public T Find(Vec2 pos) {
			if (!IsInBounds(pos)) {
				return default(T);
			}
			return _node.Find(pos).Objects.SingleOrDefault(obj => obj.Pos == pos);
		}

		public bool IsInBounds(Vec2 pos) { return _node.IsInBound(pos); }
		public bool IsEmpty(Vec2 target) { return _node.IsEmpty(target); }
		public void Walk(Action<QuadTreeNode<T>> f) { _node.Walk(f); }
		public IEnumerator<T> GetEnumerator() { return _objects.ToList().GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return _objects.ToList().GetEnumerator(); }
	}
}
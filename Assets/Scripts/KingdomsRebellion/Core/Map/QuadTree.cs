using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public class QuadTree<T> : IMap<QuadTreeNode<T>,T> where T : IPos, ISize {
		readonly QuadTreeNode<T> node;
		readonly IList<T> objects;

		public QuadTree(int width, int height) {
			node = new QuadTreeNode<T>(0, 0, width, height);
			objects = new List<T>();
		}

		public bool Add(T t, bool floating) {
			if (!IsInBounds(t.Pos)) {
				throw new ArgumentException("Position is not in bounds : " + t.Pos);
			}
			if (node.Add(t, floating)) {
				objects.Add(t);
				return true;
			}
			return false;
		}

		public bool Remove(T t, bool floating) {
			if (node.Remove(t, floating)) {
				objects.Remove(t);
				return true;
			}
			return false;
		}

		public AbstractNode<QuadTreeNode<T>> FindNode(Vec2 pos) {
			return !IsInBounds(pos) ? null : QuadTreeNodeWrapper<T>.Wrap(node.Find(pos));
		}

		public T Find(Vec2 pos) {
			return !IsInBounds(pos) ? default(T) : node.Find(pos).Objects.SingleOrDefault(obj => obj.Pos == pos);
		}

		public bool IsInBounds(Vec2 pos) {
			return node.IsInBound(pos);
		}

		public bool IsEmpty(Vec2 target) {
			return node.IsEmpty(target);
		}

		public void Walk(Action<QuadTreeNode<T>> f) {
			node.Walk(f);
		}

		public IEnumerator<T> GetEnumerator() {
			return objects.ToList().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return objects.ToList().GetEnumerator();
		}
	}
}
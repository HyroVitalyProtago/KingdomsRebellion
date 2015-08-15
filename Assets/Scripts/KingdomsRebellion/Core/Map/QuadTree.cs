using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.Map {
	public class QuadTree<T> : IMap<T> where T : IPos, ISize {
		readonly QuadTreeNode<T> node;
		readonly IList<T> objects;

		public QuadTree(int width, int height) {
			node = new QuadTreeNode<T>(0, 0, width, height);
			objects = new List<T>();

			EventConductor.On(this, "OnKRDrawGizmos");
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

		public IEnumerable<Vec2> FindPath(Vec2 start, Vec2 target) {
			return SimplifyPath(KRFacade.FindPath(FindNode(start), FindNode(target)));
		}

		static IEnumerable<Vec2> SimplifyPath(IEnumerable<Vec2> waypoints) {
			List<Vec2> simplifiedPath = new List<Vec2>(waypoints);

			if (simplifiedPath.Count <= 2) {
				return simplifiedPath;
			}

			Vec2 begin = simplifiedPath[0];
			bool canTraceStraightLine = true;
			do {
				Vec2 current = simplifiedPath[2];
				Bresenham.Line(begin.X, begin.Y, current.X, current.Y, delegate(int x, int y) {
					if (!KRFacade.IsEmpty(new Vec2(x, y))) {
						canTraceStraightLine = false;
						return false;
					}
					return true;
				});
				if (canTraceStraightLine) {
					simplifiedPath.RemoveAt(1);
				} else {
					return simplifiedPath;
				}
			} while(canTraceStraightLine && simplifiedPath.Count > 2);

			return simplifiedPath;
		}

		public AbstractNode FindNode(Vec2 pos) {
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

		void OnKRDrawGizmos() {
			Walk(GizmosDrawNode);
			/*
			Gizmos.color = Color.magenta;
			foreach (var x in __walkedNode) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one * .5f, Vector3.one);
			}

			Gizmos.color = Color.red;
			foreach (var x in __walkedFind) {
				Gizmos.DrawWireCube(x.ToVector3() + Vector3.one * .5f, Vector3.one);
			}
			*/
		}

		void GizmosDrawNode(QuadTreeNode<T> n) {
			Gizmos.color = n.IsFree() ? Color.blue : Color.green;
			Vector3 p0 = n.BottomLeft.ToVector3(), p1 = n.TopRight.ToVector3();
			Gizmos.DrawLine(p0, p0 + new Vector3(n.Width - .2f, 0, .1f));
			Gizmos.DrawLine(p0, p0 + new Vector3(.1f, 0, n.Height - .2f));
			Gizmos.DrawLine(p1, p1 - new Vector3(n.Width - .2f, 0, .1f));
			Gizmos.DrawLine(p1, p1 - new Vector3(.1f, 0, n.Height - .2f));
		}
	}
}
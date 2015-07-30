using System;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {

	public class QuadTreeNode<T> : KRObject, IPos where T : IPos, HaveRadius {

		enum ESide { NORTH, EAST, SOUTH, WEST }
		enum EQuadrant { NORTHWEST, NORTHEAST, SOUTHWEST, SOUTHEAST }
		enum EState { FREE, MIXED, OBSTRUCTED }

		int _level;
		int _width, _height;
		Vec2 _bottomLeft, _topRight, _center;
		QuadTreeNode<T>[] _subnodes;
		QuadTreeNode<T> _parent;
		IList<T> _objects;
		EState _state;

		public int Width { get { return _width; } }
		public int Height { get { return _height; } }
		public Vec2 Pos { get { return _center; } }
		public Vec2 BottomLeft { get { return _bottomLeft; } }
		public Vec2 TopRight { get { return _topRight; } }
		public IEnumerable<T> Objects { get { return _objects; } }

		public QuadTreeNode(int x, int y, int width = 1, int height = 1, int level = 0) :
			this(null, new Vec2(x, y), new Vec2(x+width, y+height), level) {}

		QuadTreeNode(QuadTreeNode<T> parent, Vec2 bottomLeft, Vec2 topRight, int level = 0) {
			_parent = parent;
			_bottomLeft = bottomLeft;
			_topRight = topRight;
			_level = level;
			_objects = new List<T>();

			_width = _topRight.X - _bottomLeft.X;
			_height = _topRight.Y - _bottomLeft.Y;
			_center = _bottomLeft + new Vec2(_width / 2, _height / 2);

			_state = EState.FREE;
		}

		public bool IsLeaf() { return _subnodes == null; }
		bool IsEmpty() { return _objects.Count == 0; }

		public bool Split() {
			if (IsLeaf()) {
				_state = EState.MIXED;

				int sublevel = _level + 1;
				_subnodes = new QuadTreeNode<T>[4];
				_subnodes[(int)EQuadrant.NORTHWEST] = new QuadTreeNode<T>(this, new Vec2(_bottomLeft.X, _center.Y), new Vec2(_center.X, _topRight.Y), sublevel);
				_subnodes[(int)EQuadrant.NORTHEAST] = new QuadTreeNode<T>(this, new Vec2(_center.X, _center.Y), new Vec2(_topRight.X, _topRight.Y), sublevel);
				_subnodes[(int)EQuadrant.SOUTHWEST] = new QuadTreeNode<T>(this, new Vec2(_bottomLeft.X, _bottomLeft.Y), new Vec2(_center.X, _center.Y), sublevel);
				_subnodes[(int)EQuadrant.SOUTHEAST] = new QuadTreeNode<T>(this, new Vec2(_center.X, _bottomLeft.Y), new Vec2(_topRight.X, _center.Y), sublevel);
			} else {
				for (int i = 0; i < _subnodes.Length; ++i) {
					_subnodes[i].Split();
				}
			}
			return true;
		}

		public bool Add(T obj) {
			if (!IsInBound(obj.Pos)) return false;

			if (!IsLeaf()) {
				int index = Index(obj);
				if (index != -1) {
					return _subnodes[index].Add(obj);
				} else {
					_objects.Add(obj);
					return true;
				}
			}

			_objects.Add(obj);

			if (_objects.Count <= 1) {
				if (_width == 1) {
					_state = EState.OBSTRUCTED;
					return true;
				}
			}

			Split();

			int i = 0;
			while (i < _objects.Count) {
				int index = Index(_objects[i]);
				if (index != -1) {
					_subnodes[index].Add(_objects[i]);
					_objects.RemoveAt(i);
				} else {
					++i;
				}
			}

			return true;
		}

		void Unsplit(T obj) {
			if (_subnodes.Count(n => !n.IsLeaf() || n._objects.Count > 0) <= 0) {
				_state = EState.FREE;
				_subnodes = null;
				if (_parent != null) { _parent.Unsplit(obj); }
			}
		}

		public bool Remove(T obj) {
			if (!IsLeaf()) {
				return Child(Quadrant(obj)).Remove(obj);
			}

			bool res = _objects.Remove(obj);

			if (_objects.Count() == 0) {
				_state = EState.FREE;
				_parent.Unsplit(obj);
			}

			return res;
		}

		public bool IsEmpty(Vec2 target) { return Find(target).Objects.SingleOrDefault(o => o.Pos == target) == null; }
		public IList<T> Retrieve(T obj) { return Retrieve(obj, new List<T>()); }

		IList<T> Retrieve(T obj, IList<T> list) {
			int index = Index(obj);
			if (index != -1 && !IsLeaf()) {
				_subnodes[index].Retrieve(obj, list);
			}
			
			for (int i = 0; i < _objects.Count; ++i) {
				list.Add(_objects[i]);
			}
			
			return list;
		}

		public void Clear() {
			_objects.Clear();
			if (!IsLeaf()) {
				for (int i = 0; i < _subnodes.Length; ++i) {
					_subnodes[i].Clear();
				}
			}
		}

		int Index(T obj) { return (int) Quadrant(obj); }
		EQuadrant Quadrant(T obj) { return Quadrant(obj.Pos); }
		EQuadrant Quadrant(Vec2 pos) {
			if (pos.X < _center.X) {
				return pos.Y < _center.Y ? EQuadrant.SOUTHWEST : EQuadrant.NORTHWEST;
			} else {
				return pos.Y < _center.Y ? EQuadrant.SOUTHEAST : EQuadrant.NORTHEAST;
			}
		}

		static readonly bool[,] ADJACENT = new bool[,] {
					/* NW     NE     SW     SE  */
			/* N */ { true,  true,  false, false },
			/* E */ { false, true,  false, true  },
			/* S */ { false, false, true,  true  },
			/* W */ { true,  false, true,  false }
		};
		bool Adjacent(ESide side, EQuadrant quad) {
			return ADJACENT[(int)side, (int)quad];
		}

		static readonly EQuadrant[,] REFLECT = new EQuadrant[,] {
					/*   NW         NE         SW         SE    */
			/* N */ { EQuadrant.SOUTHWEST, EQuadrant.SOUTHEAST, EQuadrant.NORTHWEST, EQuadrant.NORTHEAST },
			/* E */ { EQuadrant.NORTHEAST, EQuadrant.NORTHWEST, EQuadrant.SOUTHEAST, EQuadrant.SOUTHWEST },
			/* S */ { EQuadrant.SOUTHWEST, EQuadrant.SOUTHEAST, EQuadrant.NORTHWEST, EQuadrant.NORTHEAST },
			/* W */ { EQuadrant.NORTHEAST, EQuadrant.NORTHWEST, EQuadrant.SOUTHEAST, EQuadrant.SOUTHWEST }
		};
		EQuadrant Reflect(ESide side, EQuadrant quad) {
			return REFLECT[(int)side, (int)quad];
		}

		static readonly ESide[] OPPOSITE = new ESide[] { ESide.SOUTH, ESide.WEST, ESide.NORTH, ESide.EAST }; // N, E, S, W
		ESide Opposite(ESide side) {
			return OPPOSITE[(int)side];
		}

		EQuadrant Quadrant() {
			if (_parent == null) {
				throw new InvalidOperationException("The root node's quadrant isn't defined.");
			}

			if (_parent.Child(EQuadrant.NORTHWEST) == this) {
				return EQuadrant.NORTHWEST;
			} else if (_parent.Child(EQuadrant.SOUTHWEST) == this) {
				return EQuadrant.SOUTHWEST;
			} else if (_parent.Child(EQuadrant.NORTHEAST) == this) {
				return EQuadrant.NORTHEAST;
			} else {
				return EQuadrant.SOUTHEAST;
			}
		}

		QuadTreeNode<T> Child(EQuadrant quad) {
			return _subnodes[(int)quad];
		}

		QuadTreeNode<T> DeepChild(EQuadrant quad) {
			QuadTreeNode<T> current = this;
			while (!current.IsLeaf()) {
				current = current.Child(quad);
			}
			return current;
		}

		IList<QuadTreeNode<T>> Children(ESide direction) {
			IList<QuadTreeNode<T>> children = new List<QuadTreeNode<T>>();
			Children(direction, children);
			return children;
		}

		void Children(ESide direction, IList<QuadTreeNode<T>> children) {
			if (IsLeaf()) return;
			
			QuadTreeNode<T>[] nodes = new QuadTreeNode<T>[2];

			switch (direction) {
				case ESide.NORTH:
					nodes[0] = Child(EQuadrant.NORTHEAST);
					nodes[1] = Child(EQuadrant.NORTHWEST);
					break;
				case ESide.EAST:
					nodes[0] = Child(EQuadrant.NORTHEAST);
					nodes[1] = Child(EQuadrant.SOUTHEAST);
					break;
				case ESide.SOUTH:
					nodes[0] = Child(EQuadrant.SOUTHEAST);
					nodes[1] = Child(EQuadrant.SOUTHWEST);
					break;
				case ESide.WEST:
					nodes[0] = Child(EQuadrant.NORTHWEST);
					nodes[1] = Child(EQuadrant.SOUTHWEST);
					break;
			}

			for (int i = 0; i < nodes.Length; ++i) {
				if (nodes[i].IsLeaf()) {
					children.Add(nodes[i]);
				} else {
					nodes[i].Children(direction, children);
				}
			}
		}

		QuadTreeNode<T> Neighbour(ESide direction) {
			QuadTreeNode<T> neighbor = null;
			
			// Ascent the tree up to a common ancestor.
			if (_parent != null && Adjacent(direction, Quadrant())) {
				neighbor = _parent.Neighbour(direction);
			} else {
				neighbor = _parent;
			}
			
			// Backtrack mirroring the ascending moves.
			if (neighbor != null && !neighbor.IsLeaf()) {
				return neighbor.Child(Reflect(direction, Quadrant()));
			} else {
				return neighbor;
			}
		}

		QuadTreeNode<T> Neighbour(ESide direction, EQuadrant corner) {
			QuadTreeNode<T> quadnode = Neighbour(direction);
			if (quadnode == null) return null;
			while (!quadnode.IsLeaf()) {
				quadnode = quadnode.Child(Reflect(direction, corner));
			}
			return quadnode;
		}

		void Neighbours(ESide direction, IList<QuadTreeNode<T>> neighbours) {
			QuadTreeNode<T> quadnode = Neighbour(direction);
			if (quadnode != null) {
				if (quadnode.IsLeaf()) {
					neighbours.Add(quadnode);
				} else {
					quadnode.Children(Opposite(direction), neighbours);
				}
			}
		}
/*
		void TryAddQuadrantSide(ESide s1, ESide s2, EQuadrant q, IList<QuadTreeNode<T>> l) {
			var n1 = Neighbour(s1); if (n1 == null) return;
			var n2 = n1.Neighbour(s2); if (n2 == null) return;
			l.Add(n2.DeepChild(q));
		}
*/
		public IList<QuadTreeNode<T>> Neighbours() {
			IList<QuadTreeNode<T>> neighbours = new List<QuadTreeNode<T>>();
			Neighbours(ESide.NORTH, neighbours);
			Neighbours(ESide.SOUTH, neighbours);
			Neighbours(ESide.EAST, neighbours);
			Neighbours(ESide.WEST, neighbours);

			// TRY for Quadrant side
//			TryAddQuadrantSide(ESide.NORTH, ESide.EAST, EQuadrant.SOUTHWEST, neighbours);
//			TryAddQuadrantSide(ESide.NORTH, ESide.WEST, EQuadrant.SOUTHEAST, neighbours);
//			TryAddQuadrantSide(ESide.SOUTH, ESide.EAST, EQuadrant.NORTHWEST, neighbours);
//			TryAddQuadrantSide(ESide.SOUTH, ESide.WEST, EQuadrant.NORTHEAST, neighbours);

			return neighbours;
		}

		public bool IsFree() { return _state == EState.FREE; }

		public QuadTreeNode<T> Find(Vec2 pos) {
			if (IsLeaf()) { return this; }

			if (pos.X < _center.X) {
				return Child(pos.Y < _center.Y ? EQuadrant.SOUTHWEST : EQuadrant.NORTHWEST).Find(pos);
			} else {
				return Child(pos.Y < _center.Y ? EQuadrant.SOUTHEAST : EQuadrant.NORTHEAST).Find(pos);
			}
		}

		public bool IsInBound(Vec2 pos) { return pos.X >= _bottomLeft.X && pos.Y >= _bottomLeft.Y && pos.X < _topRight.X && pos.Y < _topRight.Y; }

		public void Walk(Action<QuadTreeNode<T>> f) {
			if (IsLeaf()) {
				f(this);
			} else {
				for (int i = 0; _subnodes != null && i < _subnodes.Length; ++i) {
					_subnodes[i].Walk(f);
				}
			}
		}
	}
}
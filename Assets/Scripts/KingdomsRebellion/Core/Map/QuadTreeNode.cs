using System;
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {

	public enum ESide {
		NORTH,
		EAST,
		SOUTH,
		WEST
	}

	public enum EQuadrant {
		NORTHWEST,
		NORTHEAST,
		SOUTHWEST,
		SOUTHEAST
	}

	public enum EState {
		FREE,
		MIXED,
		OBSTRUCTED
	}

	public class QuadTreeNode<T> : KRObject, IPos where T : IPos, ISize {
		
		int _level;
		int _width, _height;
		Vec2 _bottomLeft, _topRight, _center;
		QuadTreeNode<T>[] _subnodes;
		QuadTreeNode<T> _parent;
		List<T> _floatingObjects;
		List<T> _fixedObjects;
		EState _state;

		public int Width { get { return _width; } }

		public int Height { get { return _height; } }

		public Vec2 Pos { get { return _center; } }

		public Vec2 BottomLeft { get { return _bottomLeft; } }

		public Vec2 TopRight { get { return _topRight; } }

		public IEnumerable<T> Objects { get { return _floatingObjects.Concat(_fixedObjects); } }

		public QuadTreeNode(int x, int y, int width = 1, int height = 1, int level = 0) :
			this(null, new Vec2(x, y), new Vec2(x + width, y + height), level) {
		}

		QuadTreeNode(QuadTreeNode<T> parent, Vec2 bottomLeft, Vec2 topRight, int level = 0) {
			_parent = parent;
			_bottomLeft = bottomLeft;
			_topRight = topRight;
			_level = level;
			_fixedObjects = new List<T>();
			_floatingObjects = new List<T>();

			_width = _topRight.X - _bottomLeft.X;
			_height = _topRight.Y - _bottomLeft.Y;
			_center = _bottomLeft + new Vec2(_width / 2, _height / 2);

			_state = EState.FREE;
		}

		public bool IsLeaf() {
			return _subnodes == null;
		}

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

		// not floating
		bool Add(T obj, Vec2 pos, Vec2 size, bool floating = false) {
			if (!IsInBound(pos))
				return false;
			
			if (!IsLeaf()) {
				int index = Index(pos, size);
				if (index != -1) {
					return _subnodes[index].Add(obj, pos, size, floating);
				} else if (floating) {
					_floatingObjects.Add(obj);
					return true;
				} else {
					CutAndDispatch(obj, pos, size);
					return true;
				}
			}
			
			if (floating) {
				_floatingObjects.Add(obj);
				return true;
			}
			
			if (_width == 1) {
				_fixedObjects.Add(obj);
				_state = EState.OBSTRUCTED;
				return true;
			}
			
			Split();
			
			{
				int index = Index(pos, size);
				if (index != -1) {
					_subnodes[index].Add(obj, pos, size, floating);
				} else {
					CutAndDispatch(obj, pos, size);
				}
			}
			
			int i = 0;
			while (i < _floatingObjects.Count) {
				var fo = _floatingObjects[i];
				int index = Index(fo.Pos, fo.Size);
				if (index != -1) {
					_subnodes[index].Add(_floatingObjects[i], fo.Pos, fo.Size);
					_floatingObjects.RemoveAt(i);
				} else {
					++i;
				}
			}
			
			return true;
		}

		void CutAndDispatch(T obj, Vec2 pos, Vec2 size) {
			var p1 = pos;
			var s1 = _center - p1;
			if (s1.X > 0 && s1.Y > 0) {
				Child(EQuadrant.SOUTHWEST).Add(obj, p1, s1);
			}

			var p2 = _center;
			var s2 = size - s1;
			if (s2.X > 0 && s2.Y > 0) {
				Child(EQuadrant.NORTHEAST).Add(obj, p2, s2);
			}

			var p3 = new Vec2(p1.X, _center.Y);
			var s3 = new Vec2(s1.X, s2.Y);
			if (s3.X > 0 && s3.Y > 0) {
				Child(EQuadrant.NORTHWEST).Add(obj, p3, s3);
			}

			var p4 = new Vec2(_center.X, p1.Y);
			var s4 = new Vec2(s2.X, s1.Y);
			if (s4.X > 0 && s4.Y > 0) {
				Child(EQuadrant.SOUTHEAST).Add(obj, p4, s4);
			}
		}

		public bool Add(T obj, bool floating = true) {
			return Add(obj, obj.Pos, obj.Size, floating);
		}

		void Unsplit(T obj) {
			if (!_subnodes.Any(n => !n.IsLeaf() || n._fixedObjects.Count > 0)) {
				_state = EState.FREE;
				foreach (var subnode in _subnodes) {
					_floatingObjects.AddRange(subnode._floatingObjects);
				}
				_subnodes = null;
				if (_parent != null) {
					_parent.Unsplit(obj);
				}
			}
		}

		public bool Remove(T obj, bool floating = true) {
			if (!IsLeaf()) {
				if (floating) {
					if (_floatingObjects.Remove(obj)) {
						return true;
					} else {
						return Child(Quadrant(obj.Pos, obj.Size).Value).Remove(obj, floating);
					}
				} else {
					var q = Quadrant(obj.Pos, obj.Size);
					if (q.HasValue) {
						return Child(q.Value).Remove(obj, floating);
					} else {
						bool ret = false;
						foreach (var sn in _subnodes) {
							if (sn != null && sn._fixedObjects.Contains(obj)) {
								ret = ret || sn.Remove(obj, floating);
							}
						}
						return ret;
					}
				}
			}

			if (floating) {
				return _floatingObjects.Remove(obj);
			}

			bool res = _fixedObjects.Remove(obj);

			if (res) {
				_state = EState.FREE;
				_parent.Unsplit(obj);
			}

			return res;
		}

		public bool IsEmpty(Vec2 target) {
			var qn = Find(target);
			return !qn._fixedObjects.Any() && qn._floatingObjects.All(n => !Collide(n, target));
		}

		static bool Collide(T t, Vec2 v) {
			return v.X >= t.Pos.X && v.X < t.Pos.X + t.Size.X && v.Y >= t.Pos.Y && v.Y < t.Pos.Y + t.Size.Y;
		}

		public void Clear() {
			_fixedObjects.Clear();
			_floatingObjects.Clear();
			if (!IsLeaf()) {
				for (int i = 0; i < _subnodes.Length; ++i) {
					_subnodes[i].Clear();
				}
			}
		}

		int Index(Vec2 pos, Vec2 size) {
			var q = Quadrant(pos, size);
			return q.HasValue ? (int)q.Value : -1;
		}

		EQuadrant? Quadrant(Vec2 pos, Vec2 size) {
			if (pos.X + size.X <= _center.X) {
				if (pos.Y + size.Y <= _center.Y) {
					return EQuadrant.SOUTHWEST;
				} else if (pos.Y >= _center.Y) {
					return EQuadrant.NORTHWEST;
				}
			} else if (pos.X >= _center.X) {
				if (pos.Y + size.Y <= _center.Y) {
					return EQuadrant.SOUTHEAST;
				} else if (pos.Y >= _center.Y) {
					return EQuadrant.NORTHEAST;
				}
			}
			return null;
		}

		// [NW|NE|SW|SE][N|E|S|W]
		static readonly bool[,] ADJACENT = {
			{ true,  true,  false, false },
			{ false, true,  false, true  },
			{ false, false, true,  true  },
			{ true,  false, true,  false }
		};

		static bool Adjacent(ESide side, EQuadrant quad) {
			return ADJACENT[(int)side, (int)quad];
		}

		// [NW|NE|SW|SE][N|E|S|W]
		static readonly EQuadrant[,] REFLECT = { {
				EQuadrant.SOUTHWEST,
				EQuadrant.SOUTHEAST,
				EQuadrant.NORTHWEST,
				EQuadrant.NORTHEAST
			}, {
				EQuadrant.NORTHEAST,
				EQuadrant.NORTHWEST,
				EQuadrant.SOUTHEAST,
				EQuadrant.SOUTHWEST
			}, {
				EQuadrant.SOUTHWEST,
				EQuadrant.SOUTHEAST,
				EQuadrant.NORTHWEST,
				EQuadrant.NORTHEAST
			}, {
				EQuadrant.NORTHEAST,
				EQuadrant.NORTHWEST,
				EQuadrant.SOUTHEAST,
				EQuadrant.SOUTHWEST
			}
		};

		static EQuadrant Reflect(ESide side, EQuadrant quad) {
			return REFLECT[(int)side, (int)quad];
		}

		// N, E, S, W
		static readonly ESide[] OPPOSITE = {
			ESide.SOUTH,
			ESide.WEST,
			ESide.NORTH,
			ESide.EAST
		};

		static ESide Opposite(ESide side) {
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
			if (IsLeaf())
				return;
			
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
			if (quadnode == null)
				return null;
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

		public ESide SideOfNeighbour(QuadTreeNode<T> neighbour) {
			IList<QuadTreeNode<T>> neighbours = new List<QuadTreeNode<T>>();

			foreach (ESide side in Enum.GetValues(typeof(ESide))) {
				Neighbours(side, neighbours);
				if (neighbours.Contains(neighbour)) {
					return side;
				}
				neighbours.Clear();
			}

			throw new SystemException("Search side of an invalid neighbour");
		}

		public IList<QuadTreeNode<T>> Neighbours() {
			IList<QuadTreeNode<T>> neighbours = new List<QuadTreeNode<T>>();
			Neighbours(ESide.NORTH, neighbours);
			Neighbours(ESide.SOUTH, neighbours);
			Neighbours(ESide.EAST, neighbours);
			Neighbours(ESide.WEST, neighbours);
			return neighbours;
		}

		public bool IsFree() {
			return _state == EState.FREE;
		}

		public QuadTreeNode<T> Find(Vec2 pos) {
			if (IsLeaf()) {
				return this;
			}

			if (pos.X < _center.X) {
				return Child(pos.Y < _center.Y ? EQuadrant.SOUTHWEST : EQuadrant.NORTHWEST).Find(pos);
			} else {
				return Child(pos.Y < _center.Y ? EQuadrant.SOUTHEAST : EQuadrant.NORTHEAST).Find(pos);
			}
		}

		public bool IsInBound(Vec2 pos) {
			return pos.X >= _bottomLeft.X && pos.Y >= _bottomLeft.Y && pos.X < _topRight.X && pos.Y < _topRight.Y;
		}

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
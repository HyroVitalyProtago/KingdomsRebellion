﻿using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public class QuadTreeNodeWrapper<T> : AbstractNode<QuadTreeNode<T>>, IPos where T : IPos, ISize {
		QuadTreeNode<T> _node;

		public Vec2 Pos { get { return _node.Pos; } }

		QuadTreeNodeWrapper(QuadTreeNode<T> node) : base(null, 0, 0) {
			_node = node;
			_node.SetProperty("NodeWrapper", this);
		}

		public static QuadTreeNodeWrapper<T> Wrap(QuadTreeNode<T> node) {
			try {
				return node.GetProperty<QuadTreeNodeWrapper<T>>("NodeWrapper");
			} catch(KeyNotFoundException) {
				return new QuadTreeNodeWrapper<T>(node);
			}
		}

		public override bool IsFree() { return _node.IsFree(); }

		public override IEnumerable<AbstractNode<QuadTreeNode<T>>> Neighbours() {
			return _node.Neighbours().Select(quadTreeNode => Wrap(quadTreeNode) as AbstractNode<QuadTreeNode<T>>);
		}

		public override QuadTreeNode<T> WrappedNode() { return _node; }
		public override string ToString() { return string.Format("[QuadTreeNodeWrapper: Pos={0}]", Pos); }
	}
}
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.Core.Map {
	public class QuadTreeNodeWrapper<T> : AbstractNode where T : IPos,ISize {
		readonly QuadTreeNode<T> _node;

		QuadTreeNodeWrapper(QuadTreeNode<T> node) : base(node.Pos) {
			_node = node;
			_node.SetProperty("NodeWrapper", this);
		}

		public static QuadTreeNodeWrapper<T> Wrap(QuadTreeNode<T> node) {
			try {
				return node.GetProperty<QuadTreeNodeWrapper<T>>("NodeWrapper");
			} catch (KeyNotFoundException) {
				return new QuadTreeNodeWrapper<T>(node);
			}
		}

		public override bool IsFree() {
			return _node.IsFree();
		}

		public override IEnumerable<AbstractNode> Neighbours() {
			return _node.Neighbours().Select(quadTreeNode => Wrap(quadTreeNode) as AbstractNode);
		}

		public override string ToString() {
			return string.Format("[QuadTreeNodeWrapper: Pos={0}]", Pos);
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public class QuadTreeNodeWrapper<T> : AbstractNode where T : IPos,ISize {
		readonly QuadTreeNode<T> _node;

		QuadTreeNodeWrapper(QuadTreeNode<T> node, Vec2 pos) : base(pos) {
			_node = node;
			_node.SetProperty(GetID(pos), this);
		}

		public static QuadTreeNodeWrapper<T> Wrap(QuadTreeNode<T> node, Vec2 pos) {
			try {
				return node.GetProperty<QuadTreeNodeWrapper<T>>(GetID(pos));
			} catch (KeyNotFoundException) {
				return new QuadTreeNodeWrapper<T>(node, pos);
			}
		}

		public override bool IsFree() {
			return _node.IsFree();
		}

		protected override IEnumerable<AbstractNode> RealNeighbours() {
			return _node.Neighbours(Pos).Select(p => Wrap(p.Key, p.Value) as AbstractNode);
		}

		public override string ToString() {
			return GetID(Pos);
		}

		static string GetID(Vec2 p) {
			return string.Format("[QuadTreeNodeWrapper: Pos={0}]", p);
		}
	}
}
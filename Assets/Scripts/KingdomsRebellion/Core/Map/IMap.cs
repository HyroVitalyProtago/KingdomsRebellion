using System;
using System.Collections.Generic;
using KingdomsRebellion.AI;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public interface IMap<N,T> : IEnumerable<T> where T : IPos where N : IPos {
		bool Add(T u, bool floating);
		bool Remove(T u, bool floating);
		T Find(Vec2 pos);
		bool IsEmpty(Vec2 pos);
		bool IsInBounds(Vec2 pos);
		AbstractNode<N> FindNode(Vec2 pos);
		void Walk(Action<N> f);
	}
}
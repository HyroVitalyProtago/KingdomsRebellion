
using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.AI;
using System;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.Core.Map {
	public interface IMap<N,T> : IEnumerable<T> where T : IPos where N : IPos {
		bool Add(T u);
		bool Remove(T u);
		T Find(Vec2 pos);
		bool IsEmpty(Vec2 pos);
		AbstractNode<N> FindNode(Vec2 pos);
		void Walk(Action<N> f);
	}
}
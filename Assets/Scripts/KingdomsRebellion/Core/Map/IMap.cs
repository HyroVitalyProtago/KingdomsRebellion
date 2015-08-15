using System.Collections.Generic;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Map {
	public interface IMap<T> : IEnumerable<T> where T : IPos {
		bool Add(T u, bool floating);
		bool Remove(T u, bool floating);
		T Find(Vec2 pos);
		bool IsEmpty(Vec2 pos);
		bool IsInBounds(Vec2 pos);
		IEnumerable<Vec2> FindPath(Vec2 start, Vec2 target);
	}
}
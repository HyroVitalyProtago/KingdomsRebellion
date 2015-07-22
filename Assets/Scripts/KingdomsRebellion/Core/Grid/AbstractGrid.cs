using System;
using System.Collections.Generic;
using UnityEngine;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;

namespace KingdomsRebellion.Core.Grid {
    
    public abstract class AbstractGrid {

		static bool Instantiated = false;

		protected AbstractGrid() {
			Debug.Assert(!Instantiated);
			Instantiated = true;
		}

        public abstract void Add(GameObject go, Vec2 position);
        public abstract bool Remove(GameObject go);
        public abstract bool Move(GameObject go, Vec2 newPosition);
        public abstract GameObject GetGameObjectByPosition(Vec2 position);
        public bool IsEmpty(Vec2 position) {
            return GetGameObjectByPosition(position) == null;
        }

        public abstract bool IsInBounds(int position);

        public int ValidPosition(int position) {
            if (!IsInBounds(position)) {
                throw new IndexOutOfRangeException("Position out of grid");
            }
            return position;
        }

        public abstract List<GameObject> GetNearGameObjects(Vec2 position);

        public List<Vec2> GetVec2Between(Vec2 v1, Vec2 v2) {
            int minX = v1.X < v2.X ? v1.X : v2.X;
            int minY = v1.Y < v2.Y ? v1.Y : v2.Y;
            int maxX = v1.X > v2.X ? v1.X : v2.X;
            int maxY = v1.Y > v2.Y ? v1.Y : v2.Y;
            List<Vec2> result = new List<Vec2>();
            for (int i = minX; i <= maxX; ++i) {
                for (int j = minY; j <= maxY; ++j) {
                    result.Add(new Vec2(i,j));
                }
            }
            return result;
        }

        public abstract Dictionary<Unit, int> GetGameObjects();
        public abstract Vec2 GetPositionOf(GameObject go);

		public abstract Node NodeOf(Vec2 v);
		public abstract IEnumerable<Node> GetNeighbours(Node n);
    }
}
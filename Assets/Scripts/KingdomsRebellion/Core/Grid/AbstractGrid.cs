using System;
using UnityEngine;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Grid {
    
    public abstract class AbstractGrid {

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

        public abstract Vec2[] GetNearGameObjects(Vec2 position);
    }
}
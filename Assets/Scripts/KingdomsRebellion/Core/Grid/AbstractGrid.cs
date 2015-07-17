using UnityEngine;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Grid {
    
    public abstract class AbstractGrid {

        public abstract void Add(GameObject go);
        public abstract bool Remove(GameObject go);
        public abstract Vec2 GetPositionOfGameObject(GameObject go);
        public abstract GameObject GetGameObjectByPosition(Vec2 position);
        public bool IsEmpty(Vec2 position) {
            return GetGameObjectByPosition(position) == null;
        }
    }
}
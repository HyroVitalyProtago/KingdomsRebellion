using System;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core {
    public class KRGameObject : KRBehaviour, IPos, HaveRadius {
        public virtual Vec2 Pos { get; protected set; }
        public virtual int Radius { get; protected set; }
        public Color color;
        public int PlayerId { get; private set; }

        //Events : 
        //static event Action<GameObject> OnUnitDeath;
        
        //static KRGameObject() {
        //    EventConductor.Offer(typeof(KRGameObject), "OnUnitDeath");
        //}

        protected virtual void Start() {
            PlayerId = color == Color.blue ? 0 : 1;
        }

        //protected virtual void OnDestroy() {
        //    KRAttack _attack = GetComponent<KRAttack>();
        //    if (_attack != null) {
        //        GetComponent<KRAttack>().isDead = true;
        //    }
        //    OnUnitDeath(gameObject);
        //    KRFacade.GetMap().Remove(this);
        //}
    }
}

using System;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core {
    public class KRGameObject : KRBehaviour, IPos, HaveRadius {
        public virtual Vec2 Pos { get; protected set; }
        public virtual int Radius { get; protected set; }
        public int life;
        public int lifeMax;
        protected int defense;
        protected AttackTypeEnum weakness;
        public AttackTypeEnum AttackType { get; protected set; }
        public Color color;
        public int PlayerId { get; private set; }

        //Events : 
        static event Action<GameObject> OnUnitDeath;
        
        static KRGameObject() {
            EventConductor.Offer(typeof(KRGameObject), "OnUnitDeath");
	    }

        protected virtual void Start() {
            PlayerId = color == Color.blue ? 0 : 1;
        }

        public void OnDamageDone(AttackTypeEnum type, int damage) {
            if (weakness == type) {
                life -= Mathf.FloorToInt(1.3f*(damage - defense));
            } else {
                life -= damage - defense;
            }
            if (life <= 0) {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy() {
            Attack _attack = GetComponent<Attack>();
            if (_attack != null) {
                GetComponent<Attack>().isDead = true;
            }
            OnUnitDeath(gameObject);
            KRFacade.GetMap().Remove(this);
        }
    }
}

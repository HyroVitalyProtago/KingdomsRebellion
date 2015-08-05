using System;
using UnityEngine;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {
	
    [RequireComponent(typeof (KRMovement))]
    public class KRHealth : KRBehaviour {

		public event Action<GameObject> OnDeath;

		public int __lifeMax, __defense;
		public AttackTypeEnum __weakness;

        public int Life { get; private set; }
        public int LifeMax { get; private set; }
        public int Defense { get; private set; }
		public AttackTypeEnum Weakness { get; private set; }

//		static KRHealth() {
//			EventConductor.Offer(typeof(KRHealth), "OnUnitDeath");
//		}

        void Awake() {
			LifeMax = __lifeMax;
            Life = LifeMax;
			Defense = __defense;
			Weakness = __weakness;
        }
        
        public void OnDamageDone(AttackTypeEnum type, int damage) {
			if (Weakness == type) {
                Life -= Mathf.FloorToInt(1.3f * (damage - Defense));
            } else {
                Life -= damage - Defense;
            }

			if (IsDead()) {
                Destroy(gameObject);
            }
        }

		public bool IsDead() {
			return Life <= 0;
		}

        void OnDestroy() {
            OnDeath(gameObject);
            KRFacade.Remove(GetComponent<KRTransform>());
        }
    }
}
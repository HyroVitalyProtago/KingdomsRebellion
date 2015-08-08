using System;
using UnityEngine;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {

    public class KRHealth : KRBehaviour {

		public event Action<GameObject> OnDeath;

		public int __lifeMax, __defense;
		public AttackTypeEnum __weakness;

        public int Life { get; private set; }
        public int LifeMax { get; private set; }
        public int Defense { get; private set; }
		public AttackTypeEnum Weakness { get; private set; }

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
			if (OnDeath!= null) { OnDeath(gameObject); }
            KRFacade.Remove(GetComponent<KRTransform>());
        }
    }
}
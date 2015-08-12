using System;
using KingdomsRebellion.Core.Player;
using UnityEngine;

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
            int dam;
			if (Weakness == type) {
			    dam = Mathf.FloorToInt(1.3f*(damage - Defense));
			} else {
			        dam = damage - Defense;
			}
            Life -= Mathf.Clamp(dam, 1, Life);
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
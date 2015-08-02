using System;
using UnityEngine;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof (KRMovement))]
    public class KRHealth : KRBehaviour {

        public int Life { get; private set; }
        public int LifeMax { get; set; }
        public int Defense { get; set; }
        public AttackTypeEnum Weakness;

        void Start() {
            Life = LifeMax;
        }

        void OnEnable() {
            EventConductor.Offer(typeof(KRGameObject), "OnUnitDeath");
        }
        //Events : 
        static event Action<GameObject> OnUnitDeath;

        public void OnDamageDone(AttackTypeEnum type, int damage) {
            if (Weakness == type) {
                Life -= Mathf.FloorToInt(1.3f * (damage - Defense));
            } else {
                Life -= damage - Defense;
            }
            if (Life <= 0) {
                Destroy(gameObject);
            }
        }

        void OnDestroy() {
            KRAttack _attack = GetComponent<KRAttack>();
            if (_attack != null) {
                GetComponent<KRAttack>().isDead = true;
            }
            OnUnitDeath(gameObject);
            KRFacade.GetMap().Remove(this.GetComponent<KRGameObject>());
        }
    }
}
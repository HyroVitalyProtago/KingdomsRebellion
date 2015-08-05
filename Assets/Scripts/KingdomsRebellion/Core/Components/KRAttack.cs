using System;
using UnityEngine;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof(KRMovement))]
    public class KRAttack : KRBehaviour {

        event Action<GameObject, AttackTypeEnum, int> OnDamageDone;

		public int __strength, __attackSpeed, __range;
		public AttackTypeEnum __attackType;

		public GameObject Target { get; set; }
		public AttackTypeEnum AttackType { get; private set; }

		public int Strength { get; private set; }
		public int AttackSpeed { get; private set; }
		public int Range { get; private set; }

        KRTransform _krTransform;
        KRMovement _krMovement;
        Light _spot;
		int _currentFrame;

		void Awake() {
			_krTransform = GetComponent<KRTransform>();
			_krMovement = GetComponent<KRMovement>();
			_spot = gameObject.GetComponentInChildren<Light>();
			_spot.enabled = false;

			On("OnUnitDeath");

			Strength = __strength;
			AttackSpeed = __attackSpeed;
			Range = __range;
			AttackType = __attackType;
		}

        void Start() {
			_currentFrame = 0;
        }

        public void Attack(Vec2 modelPoint) {
			Target = KRFacade.Find(modelPoint);
        }

        public void UpdateGame() {
            Vec2 targetPos = Target.GetComponent<KRTransform>().Pos;
            if (Vec2.Dist(targetPos, gameObject.GetComponent<KRTransform>().Pos) == Range) {
				if (_currentFrame == 0) {
					_spot.enabled = true;
                    Target.GetComponent<KRHealth>().OnDamageDone(AttackType, Strength);
					_currentFrame = AttackSpeed;
                } else {
					_spot.enabled = false;
					--_currentFrame;
                }
            } else {
				_krMovement.Move(targetPos);
				_currentFrame = AttackSpeed;
				_spot.enabled = false;
            }
        }

        void OnUnitDeath(GameObject go) {
            if (go == Target) {
                Target = null;
            }
        }
    }
}
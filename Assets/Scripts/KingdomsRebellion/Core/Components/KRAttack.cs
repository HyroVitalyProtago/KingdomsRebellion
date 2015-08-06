using System;
using UnityEngine;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof(KRMovement))]
    public class KRAttack : KRBehaviour {

        event Action<GameObject, AttackTypeEnum, int> OnDamageDone;

		public int __strength, __attackSpeed, __range;

		public KRTransform Target { get; set; }
		public AttackTypeEnum AttackType { get; private set; }

		public int Strength { get; private set; }
		public int AttackSpeed { get; private set; }
		public int Range { get; private set; }

        KRTransform _krTransform;
        KRMovement _krMovement;
        GameObject _spot;
		int _currentFrame;

		void Awake() {
			_krTransform = GetComponent<KRTransform>();
			_krMovement = GetComponent<KRMovement>();
			_spot = gameObject.GetComponentInChildren<Light>().gameObject;
			_spot.SetActive(false);

			On("OnAttack");
			On("OnUnitDeath");

			Strength = __strength;
			AttackSpeed = __attackSpeed;
			Range = __range;
		}

        void Start() {
            AttackType = _krTransform.PlayerID == 0 ? AttackTypeEnum.Arrow : AttackTypeEnum.Sword;

			_currentFrame = 0;
        }

        private void OnAttack(int playerID, Vec2 modelPoint) {
			KRTransform u = KRFacade.GetMap().Find(modelPoint);
            if (u != null) { Target = u.GetComponent<KRTransform>(); }
        }

        public void UpdateGame() {
            Vec2 targetPos = Target.Pos;
            if (Vec2.Dist(targetPos, GetComponent<KRTransform>().Pos) == Range) {
				if (_currentFrame == 0) {
                    _spot.SetActive(true);
                    Target.GetComponent<KRHealth>().OnDamageDone(AttackType, Strength);
					_currentFrame = AttackSpeed;
                } else {
					--_currentFrame;
                }
            } else {
				_krMovement.Move(targetPos);
				_currentFrame = AttackSpeed;
                _spot.SetActive(false);
            }
        }

        void OnUnitDeath(GameObject go) {
            if (go == Target) {
                Target = null;
            }
        }
    }
}
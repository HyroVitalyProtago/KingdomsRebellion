using System;
using UnityEngine;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof(KRMovement))]
    public class KRAttack : KRBehaviour {

        event Action<GameObject, AttackTypeEnum, int> OnDamageDone;

        //        AbstractGrid _grid;

        KRTransform _krTransform;
        KRMovement _krMovement;
        Vec2 _oldPos;
        public GameObject Target { get; set; }
        public AttackTypeEnum AttackType { get; private set; }
        int _strength;
        GameObject _spot;
        int _attackSpeed;
        public bool isDead;
        public int range;

        void Start() {
            _krTransform = GetComponent<KRTransform>();
            _krMovement = GetComponent<KRMovement>();
            _oldPos = _krTransform.Pos;
            On("OnAttack");
            On("OnUnitDeath");
            _spot = gameObject.GetComponentInChildren<Light>().gameObject;
           // spot.GetComponent<Light>().color = _unit.color;
            _spot.SetActive(false);
            _strength = 14;
            isDead = false;
            range = 1;
            AttackType = _krTransform.PlayerID == 0 ? AttackTypeEnum.Arrow : AttackTypeEnum.Sword;
        }

        private void OnAttack(int playerID, Vec3 modelPoint) {
            KRGameObject u = KRFacade.GetMap().Find(new Vec2(modelPoint.X, modelPoint.Z));
            if (u != null) { Target = u.gameObject; }
        }

        public void UpdateGame() {
            Vec2 targetPos = Target.GetComponent<KRTransform>().Pos;
            if (Vec2.Dist(targetPos, gameObject.GetComponent<KRTransform>().Pos) == 1) {
                if (_attackSpeed == 0) {
                    _spot.SetActive(true);
                    Target.GetComponent<KRHealth>().OnDamageDone(AttackType, _strength);
                    _attackSpeed = 8;
                } else {
                    --_attackSpeed;
                }
            } else {
                _krMovement.Move(_krTransform.PlayerID, new Vec3(targetPos.X, 0, targetPos.Y));
                _attackSpeed = 8;
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
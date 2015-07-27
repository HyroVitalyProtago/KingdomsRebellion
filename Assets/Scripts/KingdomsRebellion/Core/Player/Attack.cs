using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;

namespace KingdomsRebellion.Core.Player {

    public class Attack : KRBehaviour {

        event Action<GameObject, AttackTypeEnum, int> OnDamageDone;

//        AbstractGrid _grid;
        Unit _unit;
        Vec2 _oldPos;
        public GameObject Target { get; set; }
        int strength;
        GameObject spot;
        int attackSpeed;
        public bool isDead;
        public int range;

        void Start() {
//            _grid = KRFacade.GetMap();
            _unit = GetComponent<Unit>();
            _oldPos = Vec2.FromVector3(_unit.transform.position);
            On("OnAttack");
            On("OnUnitDeath");
            spot = gameObject.GetComponentInChildren<Light>().gameObject;
            spot.GetComponent<Light>().color = _unit.color;
            spot.SetActive(false);
            strength = 14;
            isDead = false;
            range = 1;
        }

        private void OnAttack(int playerID, Vec3 modelPoint) {
            Target = KRFacade.GetMap().Find(new Vec2(modelPoint.X, modelPoint.Z)).gameObject;
        }

        // TODO remove and replace _isAttacking by _unit.attacking
        public void UpdateGame() {
			Vec2 targetPos = Target.GetComponent<Unit>().Pos;
			if (Vec2.Dist(targetPos, gameObject.GetComponent<Unit>().Pos) == 1) {
                if (attackSpeed == 0) {
                    spot.SetActive(true);
                    Target.GetComponent<Unit>().OnDamageDone(_unit.AttackType, strength);
                    attackSpeed = 8;
                } else {
                    --attackSpeed;
                }
            } else {
                _unit.GetComponent<Movement>().Move(_unit.PlayerId, new Vec3(targetPos.X, 0, targetPos.Y));
                attackSpeed = 8;
                spot.SetActive(false);
            }
        }

        void OnUnitDeath(GameObject go) {
            if (go == Target) {
                Target = null;
            }
        }
    }
}

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

        AbstractGrid _grid;
        Unit _unit;
        Vec2 _oldPos;
        GameObject _target;
        bool _isAttacking;
        int strength;
        GameObject spot;
        int attackSpeed;
        public bool isDead;

        void Start() {
            _grid = KRFacade.GetGrid();
            _unit = GetComponent<Unit>();
            _oldPos = Vec2.FromVector3(_unit.transform.position);
            _isAttacking = false;
            On("OnUnitDeath");
            spot = gameObject.GetComponentInChildren<Light>().gameObject;
            spot.GetComponent<Light>().color = _unit.color;
            spot.SetActive(false);
            strength = 14;
            isDead = false;
        }

        public void LaunchAttack(int playerID, Vec3 modelPoint) {
            _unit.GetComponent<Movement>().Move(playerID, modelPoint);
            _target = _grid.GetGameObjectByPosition(new Vec2(modelPoint.X, modelPoint.Z));
            _isAttacking = true;
        }

        // TODO remove and replace _isAttacking by _unit.attacking
        public void UpdateGame() {
            if (_target == null) {
                _isAttacking = false;
            }
            Vec2 newPos = Vec2.FromVector3(_unit.transform.position);
            if (newPos == _oldPos && !_isAttacking) {
                List<GameObject> nearObjects = _grid.GetNearGameObjects(newPos, 6);
                if (nearObjects.Count > 0) {
                    foreach (var obj in nearObjects) {
                        Debug.Log(obj + " : " + obj.GetComponent<Unit>().PlayerId);
                        if (obj.GetComponent<Unit>().PlayerId != _unit.PlayerId) {
                            LaunchAttack(_unit.PlayerId, Vec3.FromVector3(obj.transform.position));
                            break;
                        }
                    }
                }
            } else {
                _oldPos = newPos;
            }

            if (_isAttacking && attackSpeed == 0) {
                Attacking();
                attackSpeed = 8;
            } else if (_isAttacking) {
                --attackSpeed;
            }

            if (_unit.life <= 0) {
                Destroy(gameObject);
            }
        }

        void OnUnitDeath(GameObject go) {
            if (go == _target) {
                _target = null;
                _isAttacking = false;
            }
        }

        private void Attacking() {
            if ( _target != null) {
                // TODO Replace 1 by the range of the attack
                if (Vec2.Dist(_grid.GetPositionOf(_target), _grid.GetPositionOf(gameObject)) == 1) {
                    spot.SetActive(true);
                    _target.GetComponent<Unit>().OnDamageDone(_unit.AttackType, strength);
                } else {
                    Vec2 pos = _grid.GetPositionOf(_target);
                    _unit.GetComponent<Movement>().Move(_unit.PlayerId, new Vec3(pos.X, 0, pos.Y));
                    _isAttacking = false;
                }
                //spot.SetActive(false);
            }
        }
    }
}

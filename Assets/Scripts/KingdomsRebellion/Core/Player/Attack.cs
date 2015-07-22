using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;

namespace KingdomsRebellion.Core.Player {

    public class Attack : KRBehaviour {


        AbstractGrid _grid;
        Unit _unit;
        Vec2 _oldPos;
        GameObject _target;
        bool _isAttacking;

        void Start() {
            _grid = KRFacade.GetGrid();
            _unit = GetComponent<Unit>();
            _oldPos = Vec2.FromVector3(_unit.transform.position);
            _isAttacking = false;
            On("OnUnitDeath");
        }

        public void OnAttack(int playerID, Vec3 modelPoint) {
            _unit.GetComponent<Movement>().Move(playerID, modelPoint);
            _target = _grid.GetGameObjectByPosition(new Vec2(modelPoint.X, modelPoint.Z));
            _isAttacking = true;
            _unit.ennemyTargeted = _target.GetComponent<Unit>();
            _unit.attacking = true;
        }

        // TODO remove and replace _isAttacking by _unit.attacking
        public void UpdateGame() {
//            if (_target == null) {
//                _unit.attacking = false;
//                _isAttacking = false;
//            }
//            Vec2 newPos = Vec2.FromVector3(_unit.transform.position);
//            if (newPos == _oldPos && !_isAttacking) {
//                List<GameObject> nearObjects = _grid.GetNearGameObjects(newPos, 6);
//                if (nearObjects.Count > 0) {
//                    foreach (var obj in nearObjects) {
//                        if (obj.GetComponent<Unit>().playerId != _unit.playerId) {
//                            OnAttack(_unit.playerId, Vec3.FromVector3(obj.transform.position));
//                            break;
//                        }
//                    }
//                }
//            } else {
//                if (!_isAttacking) {
//                    _target = null;
//                }
//                _oldPos = newPos;
//            }
        }

        void OnUnitDeath(GameObject go) {
            if (go == _target) {
                _unit.attacking = false;
                _isAttacking = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using UnityEngine;
using System.Linq;

namespace KingdomsRebellion.Core.FSM {

    public class AttackState : FSMState {

        readonly KRAttack _attack;
		readonly KRTransform _krtransform;
        readonly KRMovement _krmovement;
        Vec2 _targetPos;

        public AttackState(FiniteStateMachine fsm) : base(fsm) {
            _attack = fsm.GetComponent<KRAttack>();
			_krtransform = fsm.GetComponent<KRTransform>();
			_krmovement = fsm.GetComponent<KRMovement>();
        }

        public override void Enter() {
            
        }


        public override Type Execute() {
            if (_attack.Target == null) {
				IEnumerable<GameObject> gameObjects = KRFacade.Around(_krtransform.Pos, 6).Where(go => go.GetComponent<KRTransform>().PlayerID != -1);
                foreach (var obj in gameObjects) {
					if (obj.GetComponent<KRTransform>().PlayerID != _krtransform.PlayerID) {
                        _attack.Attack(obj);
                        return null;
                    }
                }
                return null;
            }

			if (_attack.Target.PlayerID == _krtransform.PlayerID) {
                _krmovement.Move(_attack.Target.Pos);
                return typeof(MovementState);
            }
            int i = 0;
            while (i < _attack.Target.Size.X - 1) {
                if (_attack.Target.Pos.X + i >= _krtransform.Pos.X) break;
                ++i;
            }
            int j = 0;
            while (j < _attack.Target.Size.Y - 1) {
                if (_attack.Target.Pos.Y + j >= _krtransform.Pos.Y) break;
                ++j;
            }
            if (Vec2.Dist(_attack.Target.Pos+ new Vec2(i,j), _krtransform.Pos) ==
                _attack.Range) {
                _attack.UpdateGame();
            } else {
                if (_krmovement != null) {
                    if (_attack.Range == 1) {
                        if (_attack.Target.GetComponent<KRMovement>() != null) {
                            _krmovement.Follow(_attack.Target);
                        } else {
                            _krmovement.Move(_attack.Target.Pos+ new Vec2(i,j));
                        }
                        return typeof(MovementState);
                    } else {
                        return typeof(RangeState);
                    }
                }
                return null;
            }

            return GetType();
        }
    }
}
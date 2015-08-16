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
        
        public AttackState(FiniteStateMachine fsm) : base(fsm) {
            _attack = fsm.GetComponent<KRAttack>();
			_krtransform = fsm.GetComponent<KRTransform>();
			_krmovement = fsm.GetComponent<KRMovement>();
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

			if (Vec2.Dist(_attack.Target.Pos, _krtransform.Pos) ==
                _attack.Range) {
                _attack.UpdateGame();
            } else {
				if (_krmovement != null) {
				    if (_attack.Range == 1) {
                        _krmovement.Follow(_attack.Target);
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
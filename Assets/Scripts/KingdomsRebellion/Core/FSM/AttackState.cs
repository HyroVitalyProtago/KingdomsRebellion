using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using UnityEngine;

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

        public override void Enter() {
//            Debug.Log("A L'ATTAAAAQUE !!!");
        }

        public override Type Execute() {
            if (_attack.Target == null) {
				IEnumerable<GameObject> gameObjects = KRFacade.Around(_krtransform.Pos, 6);
                foreach (var obj in gameObjects) {
					if (obj.GetComponent<KRTransform>().PlayerID != _krtransform.PlayerID) {
                        _attack.Attack(obj);
                        return null;
                    }
                }
                return null;
            }

			if (_attack.Target.GetComponent<KRTransform>().PlayerID == _krtransform.PlayerID) {
				_fsm.GetComponent<KRMovement>().Move(_attack.Target.GetComponent<KRTransform>().Pos);
                return typeof(MovementState);
            }

			if (Vec2.Dist(_attack.Target.GetComponent<KRTransform>().Pos, _krtransform.Pos) ==
                _attack.Range) {
//                Debug.Log("FRAPPE !");
                _attack.UpdateGame();
            } else {
				if (_krmovement != null) {
					_krmovement.Follow(_attack.Target);
                    return typeof(MovementState);
                }
                return null;
            }

            return GetType();
        }

        public override void Exit() {
//            Debug.Log("Y a plus rien à attaquer :( !");
        }
    }
}
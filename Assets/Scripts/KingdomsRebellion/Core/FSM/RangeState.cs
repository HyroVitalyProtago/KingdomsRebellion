using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class RangeState : FSMState {

        readonly KRAttack _attack;
        readonly KRTransform _krtransform;
        readonly KRMovement _krmovement;

        public RangeState(FiniteStateMachine fsm) : base(fsm) {
            _attack = fsm.GetComponent<KRAttack>();
            _krtransform = fsm.GetComponent<KRTransform>();
            _krmovement = fsm.GetComponent<KRMovement>();
        }

        public override Type Execute() {
            int dist = Mathf.Abs(Vec2.Dist(_krtransform.Pos, _attack.Target.Pos));
            if (dist > _attack.Range) {
                _krmovement.Move(_attack.Target.Pos);
                _krmovement.UpdateGame();
            } else if (dist == _attack.Range) {
                return null;
            } else {
                Vec2 nearPos = KRFacade.NearSquareAt(_krtransform.Pos, _attack.Target.Pos, _attack.Range);
                if (nearPos != null) {
                    _krmovement.Move(nearPos);
                    return typeof (MovementState);
                }
            }
            return GetType();
        }



    }
}
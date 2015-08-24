using System;
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
            if (_attack.Target == null) return null;
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
            int dist = Mathf.Abs(Vec2.Dist(_krtransform.Pos, _attack.Target.Pos + new Vec2(i, j)));
            if (dist > _attack.Range) {
                _krmovement.Move(_attack.Target.Pos + new Vec2(i, j));
                _krmovement.UpdateGame();
            } else if (dist == _attack.Range) {
                return null;
            } else {
                Vec2 nearPos = KRFacade.NearSquareAt(_krtransform.Pos, _attack.Target.Pos + new Vec2(i, j), _attack.Range);
                if (nearPos != null) {
                    _krmovement.Move(nearPos);
                    return typeof (MovementState);
                }
            }
            return GetType();
        }



    }
}
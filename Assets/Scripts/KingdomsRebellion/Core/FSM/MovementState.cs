using System;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Core.FSM {

    public class MovementState : FSMState {
        readonly KRMovement _movement;

        public MovementState(FiniteStateMachine fsm) : base(fsm) {
			_movement = fsm.GetComponent<KRMovement>();
        }

        public override Type Execute() {
            if (!_movement.HaveTarget()) { return null; }
            
            _movement.UpdateGame();

            return GetType();
        }
    }
}
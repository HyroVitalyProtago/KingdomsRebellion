using System;

namespace KingdomsRebellion.Core.FSM {
    public class IDLEState :  FSMState {
        public IDLEState(FiniteStateMachine fsm) : base(fsm) { }

        public override Type Execute() {
            return GetType();
        }
    }
}


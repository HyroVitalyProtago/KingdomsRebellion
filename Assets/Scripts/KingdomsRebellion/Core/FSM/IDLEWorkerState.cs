using System;

namespace KingdomsRebellion.Core.FSM {

public class IDLEWorkerState : IDLEState {
    public IDLEWorkerState(FiniteStateMachine fsm) : base(fsm) { }

        public override Type Execute() {
            return GetType();
        }
    }
}

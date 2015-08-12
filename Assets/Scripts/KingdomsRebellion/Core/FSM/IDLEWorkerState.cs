using System;

namespace KingdomsRebellion.Core.FSM {

public class IDLEWorkerState : IDLEState {
    public IDLEWorkerState(FiniteStateMachine fsm) : base(fsm) { }

        // TODO IDLE by type of units, here is the code for soldier. Replace 6 by vision sight
        public override Type Execute() {
            return GetType();
        }
    }
}

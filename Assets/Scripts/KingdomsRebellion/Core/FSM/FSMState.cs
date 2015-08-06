using System;

namespace KingdomsRebellion.Core.FSM {
    public abstract class FSMState {

        protected FiniteStateMachine _fsm;

        protected FSMState(FiniteStateMachine fsm) { _fsm = fsm; }

        public virtual void Enter() {}
        public abstract Type Execute();
        public virtual void Exit() {}

    }
}
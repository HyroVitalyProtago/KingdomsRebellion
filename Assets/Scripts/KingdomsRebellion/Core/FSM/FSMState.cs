using KingdomsRebellion.Core.Grid;

namespace KingdomsRebellion.Core.FSM {
    public abstract class FSMState {

        protected FiniteStateMachine fsm;
        AbstractGrid _grid;

        protected FSMState(FiniteStateMachine fsm) {
            this.fsm = fsm;
        }


        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();

    }
}
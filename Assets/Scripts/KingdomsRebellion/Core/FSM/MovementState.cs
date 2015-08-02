using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class MovementState : FSMState {
        readonly Movement _movement;

        public MovementState(FiniteStateMachine fsm) : base(fsm) {
            _movement = fsm.GetComponent<Movement>();
        }

        public override void Enter() {
            Debug.Log("Encore du travail ?");
        }

        public override void Execute() {
            Debug.Log(_movement.Target);
			if (_movement._Follow) _movement.Target = _movement._Follow.GetComponent<KRGameObject>().Pos;
            if (_movement.Target == null || _movement.Pos == _movement.Target) {
                fsm.PopState();
                return;
            }
            Debug.Log("tiptap");
            _movement.UpdateGame();
        }

        public override void Exit() {
            Debug.Log("Je m'arrête ! ");
        }
    }
}
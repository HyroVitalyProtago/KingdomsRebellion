using UnityEngine;
using System.Collections;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.AI {

    public class MovementState : FSMState {

        Movement _movement;

        public MovementState(FiniteStateMachine fsm) : base(fsm) {
            _movement = fsm.GetComponent<Movement>();
        }

        public override void Enter() {
            Debug.Log("Encore du travail ?");
        }

        public override void Execute() {
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
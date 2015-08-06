using System;
using KingdomsRebellion.Core.Player;
using UnityEngine;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Core.FSM {

    public class MovementState : FSMState {
        readonly KRMovement _movement;

        public MovementState(FiniteStateMachine fsm) : base(fsm) {
			_movement = fsm.GetComponent<KRMovement>();
        }

        public override void Enter() {
//            Debug.Log("Encore du travail ?");
        }

        public override Type Execute() {
            if (!_movement.HaveTarget()) { return null; }
            
//			Debug.Log("tiptap");
            _movement.UpdateGame();

            return GetType();
        }

        public override void Exit() {
//            Debug.Log("Je m'arrête !");
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.AI {

    public class FiniteStateMachine : KRBehaviour {
        private Stack<FSMState> _stack;

        public void Start() {
            _stack = new Stack<FSMState>();
            _stack.Push(new IDLEState(this));
        }

        public void PopState() {
            GetCurrentState().Exit();
            _stack.Pop();
            if (GetCurrentState() == null) {
                PushState(new IDLEState(this), false);
            }
            GetCurrentState().Enter();
        }

        public void PushState(FSMState state, bool isHumanOrder) {
            if (isHumanOrder) {
                _stack.Clear();
                _stack.Push(new IDLEState(this));
            }
            if (state != GetCurrentState()) {
                GetCurrentState().Exit();
                _stack.Push(state);
                GetCurrentState().Enter();
            }
        }

        public FSMState GetCurrentState() {
            return _stack.Count > 0 ?_stack.Peek() : null;
        }

        public void UpdateGame() {
            FSMState state = GetCurrentState();
            if (state != null) {
                state.Execute();
            }
        }

        public void Move(int playerId, Vec3 modelPoint) {
            GetComponent<Movement>().Move(playerId, modelPoint);
            PushState(new MovementState(this), true);
        }

        public void Attack(int playerId, Vec3 modelPoint) {
            PushState(new AttackState(this), true);
        }
    }
}
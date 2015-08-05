
using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Core.FSM {

    public class FiniteStateMachine : KRBehaviour {

        Stack<FSMState> _stack;
        object[] self;

        void Awake() {
            _stack = new Stack<FSMState>();
            _stack.Push(new IDLEState(this));
            self = new object[]{ this };
        }

        void PopState() {
            GetCurrentState().Exit();
            _stack.Pop();
            if (GetCurrentState() == null) {
                PushState(new IDLEState(this));
            }
            GetCurrentState().Enter();
        }

        void PushState(FSMState state, bool isHumanOrder = false) {
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

        FSMState GetCurrentState() {
            return _stack.Count > 0 ?_stack.Peek() : null;
        }

        public void UpdateGame() {
            FSMState state = GetCurrentState();
            if (state != null) {
                Type t = state.Execute();
                if (t == null) {
                    PopState();
                } else if (state.GetType() != t) {
                    PushState(Activator.CreateInstance(t, self) as FSMState);
                }
            }
        }

        public void Move(Vec2 modelPoint) {
            GetComponent<KRMovement>().Move(modelPoint);
            PushState(new MovementState(this), true);
        }

        public void Attack(Vec2 modelPoint) {
			GetComponent<KRAttack>().Attack(modelPoint);
            PushState(new AttackState(this), true);
        }
    }
}
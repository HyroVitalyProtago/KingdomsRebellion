
using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.FSM {

    public class FiniteStateMachine : KRBehaviour {

        Stack<FSMState> _stack;
        object[] self;
        IDLEState idle;

        void Awake() {
            if (GetComponent<KRBuild>() != null) {
                idle = new IDLEWorkerState(this);
            } else {
                idle = new IDLESoldierState(this);
            }
            _stack = new Stack<FSMState>();
            _stack.Push(new IDLEState(this));
            self = new object[]{ this };
        }

        void PopState() {
            GetCurrentState().Exit();
            _stack.Pop();
            if (GetCurrentState() == null) {
                PushState(idle);
            }
            GetCurrentState().Enter();
        }

        void PushState(FSMState state, bool isHumanOrder = false) {
            if (isHumanOrder) {
                _stack.Clear();
                _stack.Push(idle);
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

        public void Build(string nameObj, Vec2 pos) {
            GetComponent<KRBuild>().Build(nameObj, pos);
            PushState(new BuildState(this), true);
        }

        public void Repare(Vec2 pos) {
            GetComponent<KRBuild>().Repare( pos);
            PushState(new BuildState(this), true);
        }
    }
}
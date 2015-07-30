using System.Collections.Generic;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class AttackState : FSMState {
        readonly Attack _attack;
        readonly Unit _unit;

        public AttackState(FiniteStateMachine fsm) : base(fsm) {
            _attack = fsm.GetComponent<Attack>();
            _unit = fsm.GetComponent<Unit>();
        }

        public override void Enter() {
            Debug.Log("A L'ATTAAAAQUE !!!");
        }

        public override void Execute() {
            if (_attack.Target == null) {
                IEnumerable<GameObject> gameObjects = KRFacade.Around(_unit.Pos, 6);
                foreach (var obj in gameObjects) {
                    if (obj.GetComponent<Unit>().PlayerId != _unit.PlayerId) {
                        _attack.Target = obj;
                        return;
                    }
                }
                fsm.PopState();
                return;
            }

            if (_attack.Target.GetComponent<Unit>().PlayerId == _unit.PlayerId) {
                fsm.GetComponent<Movement>().Target = Vec2.FromVector3(_attack.Target.transform.position);
                fsm.PopState();
                fsm.PushState(new MovementState(fsm), false);
                return;
            }
            if (Vec2.Dist(_attack.Target.GetComponent<Unit>().Pos, _unit.Pos) ==
                _attack.range) {
                Debug.Log("FRAPPE !");
                _attack.UpdateGame();
            } else {
                fsm.GetComponent<Movement>().Follow(_attack.Target);
                fsm.PushState(new MovementState(fsm), false);
            }
        }

        public override void Exit() {
            Debug.Log("Y a plus rien à attaquer :( !");
        }
    }
}
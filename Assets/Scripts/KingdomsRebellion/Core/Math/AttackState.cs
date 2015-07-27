using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Network;
using System.Linq;

namespace KingdomsRebellion.Core.AI {

    public class AttackState : FSMState {

        Attack _attack;
//        AbstractGrid _grid;

        public AttackState(FiniteStateMachine fsm) : base(fsm) {
            _attack = fsm.GetComponent<Attack>();
//            _grid = KRFacade.GetGrid();
        }

        public override void Enter() {
            Debug.Log("A L'ATTAAAAQUE !!!");
        }

        public override void Execute() {
            if (_attack.Target != null &&
			    Vec2.Dist(_attack.Target.GetComponent<Unit>().Pos, fsm.GetComponent<Unit>().Pos) ==
                _attack.range) {
                Debug.Log("FRAPPE !");
                _attack.UpdateGame();
            } else if (_attack.Target == null) {
				IEnumerable<GameObject> gameObjects = KRFacade.Around(fsm.GetComponent<Unit>().Pos, 6);
                foreach (var obj in gameObjects) {
                    if (obj.GetComponent<Unit>().PlayerId != fsm.GetComponent<Unit>().PlayerId) {
                        _attack.Target = obj;
                        return;
                    }
                }
                fsm.PopState();
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
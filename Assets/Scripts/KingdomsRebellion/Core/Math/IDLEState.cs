using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Model;

namespace KingdomsRebellion.Core.AI {

    public class IDLEState : FSMState {

//        private AbstractGrid _grid;
        
        // Use this for initialization
        private void Start() {}

        public IDLEState(FiniteStateMachine fsm) : base(fsm) {
//            _grid = KRFacade.GetGrid();
        }

        public override void Enter() {}

        public override void Execute() {
            // TODO IDLE by type of units, here is the code for soldier
			IEnumerable<GameObject> gameObjects = KRFacade.Around(fsm.GetComponent<Unit>().Pos, 6);
            foreach (var obj in gameObjects) {
                if (obj.GetComponent<Unit>().PlayerId != fsm.GetComponent<Unit>().PlayerId) {
                    fsm.PushState(new AttackState(fsm), false);
                    return;
                }
            }
        }

        public override void Exit() {}
    }
}

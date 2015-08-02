using System.Collections.Generic;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class IDLEState : FSMState {

        
        // Use this for initialization
        private void Start() {}

        public IDLEState(FiniteStateMachine fsm) : base(fsm) {}

        public override void Enter() {}

        public override void Execute() {
            // TODO IDLE by type of units, here is the code for soldier. Replace 6 by vision sight
			IEnumerable<GameObject> gameObjects = KRFacade.Around(fsm.GetComponent<Unit>().Pos, 6);
            foreach (var obj in gameObjects) {
                if (obj.GetComponent<KRGameObject>().PlayerId != fsm.GetComponent<Unit>().PlayerId) {
                    fsm.GetComponent<Attack>().Target = obj;
                    fsm.PushState(new AttackState(fsm), false);
                    return;
                }
            }
        }

        public override void Exit() {}
    }
}

using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Model;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class IDLEState : FSMState {

        public IDLEState(FiniteStateMachine fsm) : base(fsm) {}

        // TODO IDLE by type of units, here is the code for soldier. Replace 6 by vision sight
        public override Type Execute() {
			IEnumerable<GameObject> gameObjects = KRFacade.Around(_fsm.GetComponent<Unit>().Pos, 6);
            foreach (var obj in gameObjects) {
                if (obj.GetComponent<KRGameObject>().PlayerId != _fsm.GetComponent<Unit>().PlayerId) {
                    _fsm.GetComponent<KRAttack>().Target = obj;
                    return typeof(AttackState);
                }
            }
            return GetType();
        }
    }
}

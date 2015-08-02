using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class IDLEState : FSMState {

        public IDLEState(FiniteStateMachine fsm) : base(fsm) {}

        // TODO IDLE by type of units, here is the code for soldier. Replace 6 by vision sight
        public override Type Execute() {
			IEnumerable<GameObject> gameObjects = KRFacade.Around(_fsm.GetComponent<KRTransform>().Pos, 6);
            foreach (var obj in gameObjects) {
				if (obj.GetComponent<KRTransform>().PlayerID != _fsm.GetComponent<KRTransform>().PlayerID) {
                    _fsm.GetComponent<KRAttack>().Target = obj;
                    return typeof(AttackState);
                }
            }
            return GetType();
        }
    }
}

using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.Components;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {

    public class IDLESoldierState : IDLEState {

        KRTransform _krTransform;

        public IDLESoldierState(FiniteStateMachine fsm) : base(fsm) {
            _krTransform = fsm.GetComponent<KRTransform>();
        }
        
        // TODO IDLE by type of units, here is the code for soldier. Replace 6 by vision sight
        public override Type Execute() {
			IEnumerable<GameObject> gameObjects = KRFacade.Around(_fsm.GetComponent<KRTransform>().Pos, 6);
            foreach (var obj in gameObjects) {
				if (obj.GetComponent<KRTransform>().PlayerID != _fsm.GetComponent<KRTransform>().PlayerID) {
                    _fsm.GetComponent<KRAttack>().Attack(obj);
                    return typeof(AttackState);
                }
            }
            return GetType();
        }
    }
}

using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;
using UnityEngine;

namespace KingdomsRebellion.Core.Player {
	public class PlayerActions : KRBehaviour {

		IList<GameObject>[] _selectedObjects;

		void OnEnable() {
			On("OnSelection");
			On("OnMove");
		    On("OnAttack");
		}

		void OnDisable() {
			Off("OnSelection");
			Off("OnMove");
		    Off("OnAttack");
		}

		void Start() {
			_selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < _selectedObjects.Length; ++i) {
				_selectedObjects[i] = new List<GameObject>();
			}
		}

		void OnSelection(int playerID, IList<GameObject> selectedObjects) {
			_selectedObjects[playerID] = selectedObjects;
		}

		void OnMove(int playerID, Vec3 modelPoint) {
			for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
				_selectedObjects[playerID][i].GetComponent<FiniteStateMachine>().Move(playerID, modelPoint);
			}
		}

	    void OnAttack(int playerID, Vec3 modelPoint) {
            for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
                _selectedObjects[playerID][i].GetComponent<FiniteStateMachine>().Attack(playerID, modelPoint);
            }
	    }
	}
}
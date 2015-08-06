using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.Core.Components;

namespace KingdomsRebellion.Core.Player {
	public class PlayerActions : KRBehaviour {

		static IList<GameObject>[] _selectedObjects;

		void OnEnable() {
			On("OnSelection");
			On("OnMove");
		    On("OnAttack");
			On("OnSpawn");
		}

		void OnDisable() {
			Off("OnSelection");
			Off("OnMove");
		    Off("OnAttack");
			Off("OnSpawn");
		}

		void Start() {
			_selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
			for (int i = 0; i < _selectedObjects.Length; ++i) {
				_selectedObjects[i] = new List<GameObject>();
			}
		}

		// used too to drag selectables
		void OnSelection(int playerID, IList<GameObject> selectedObjects) {
			_selectedObjects[playerID] = selectedObjects;
		}

		void OnMove(int playerID, Vec2 modelPoint) {
			for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
				_selectedObjects[playerID][i].GetComponent<FiniteStateMachine>().Move(playerID, modelPoint);
			}
		}

	    void OnAttack(int playerID, Vec2 modelPoint) {
            for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
                _selectedObjects[playerID][i].GetComponent<FiniteStateMachine>().Attack(playerID, modelPoint);
            }
	    }

		// TODO
		void OnSpawn(int playerID, KeyCode keyCode) {
		    switch (keyCode) {
		        case KeyCode.C : 
                    _selectedObjects[playerID][0].GetComponent<KRSpawn>().Spawn("Infantry");
		            break;
                case KeyCode.V :
                    Debug.Log("archer");
                    _selectedObjects[playerID][0].GetComponent<KRSpawn>().Spawn("Archer");
		            break;
		    }
		   
		}

	    public static bool IsMines() {
			return _selectedObjects[NetworkAPI.PlayerId].Any(u => u.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId);
	    }

		public static bool IsBuilding() {
			return _selectedObjects[NetworkAPI.PlayerId].Any(u => u.GetComponent<KRSpawn>() != null);
		}
	}
}
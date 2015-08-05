using System.Collections.Generic;
using System.Linq;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network;
using UnityEngine;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Network.Link;
using System;

namespace KingdomsRebellion.Core.Player {
	public class PlayerActions : KRBehaviour {

		static IList<GameObject>[] _selectedObjects;

		void OnEnable() {
			On("OnGameAction");
			On("OnSelection");
		}

		void OnDisable() {
			Off("OnGameAction");
			Off("OnSelection");
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

		void OnGameAction(int playerID, Action<GameObject> f) {
			for (int i = 0; i < _selectedObjects[playerID].Count; ++i) {
				f(_selectedObjects[playerID][i]);
			}
		}

	    public static bool IsMines() {
			return _selectedObjects[NetworkAPI.PlayerId].Any(u => u.GetComponent<KRTransform>().PlayerID == NetworkAPI.PlayerId);
	    }

		public static bool IsBuilding() {
			return _selectedObjects[NetworkAPI.PlayerId].All(u => u.GetComponent<KRSpawn>() != null);
		}
	}
}
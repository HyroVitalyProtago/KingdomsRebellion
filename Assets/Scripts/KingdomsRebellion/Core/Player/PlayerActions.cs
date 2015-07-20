﻿using UnityEngine;
using System.Collections.Generic;
using KingdomsRebellion.Network;
using KingdomsRebellion.Network.Link;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Player {
	public class PlayerActions : KRBehaviour {

		IList<GameObject>[] _selectedObjects;

		void OnEnable() {
			On("OnSelection");
			On("OnMove");
		}

		void OnDisable() {
			Off("OnSelection");
			Off("OnMove");
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
				_selectedObjects[playerID][i].GetComponent<Movement>().Move(playerID, modelPoint);
			}
		}
	}
}
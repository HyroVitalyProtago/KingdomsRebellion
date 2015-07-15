﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerActions : MonoBehaviour {

	IList<GameObject>[] selectedObjects;

	void OnEnable() {
		Selection.OnSelection += OnSelection;
		MoveAction.OnMove += OnMove;
	}

	void OnDisable() {
		Selection.OnSelection -= OnSelection;
		MoveAction.OnMove -= OnMove;
	}

	void Start() {
		selectedObjects = new List<GameObject>[NetworkAPI.maxConnection];
		for (int i = 0 ; i < selectedObjects.Length ; ++i) {
			selectedObjects[i] = new List<GameObject>();
		}

	}

	void OnSelection(int playerID, IList<GameObject> selectedObjects) {
		this.selectedObjects[playerID] = selectedObjects;
	}

	void OnMove(int playerID, Camera camera, Vector3 mousePosition) {
		for (int i = 0; i < selectedObjects[playerID].Count; ++i) {
			selectedObjects[playerID][i].GetComponent<Movement>().Move(playerID, camera, mousePosition);
		}
	}
}

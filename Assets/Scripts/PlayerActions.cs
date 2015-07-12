using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerActions : MonoBehaviour {

	IList<GameObject> selectedObjects;

	void OnEnable() {
		Mouse.OnRightClick += OnMove;
		Selection.OnSelection += OnSelection;
	}

	void OnDisable() {
		Mouse.OnRightClick -= OnMove;
		Selection.OnSelection -= OnSelection;
	}

	void Start() {
		selectedObjects = new List<GameObject>();
	}

	void OnSelection(IList<GameObject> selectedObjects) {
		this.selectedObjects = selectedObjects;
	}

	void OnMove(int playerId, Camera camera, Vector3 mousePosition) {
		for (int i = 0; i < selectedObjects.Count; ++i) {
			selectedObjects[i].GetComponent<Movement>().Move(playerId, camera, mousePosition);
		}
	}
}

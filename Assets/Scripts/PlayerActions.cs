using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerActions : MonoBehaviour {

	IList<GameObject> selectedObjects;

	void OnEnable() {
		Selection.OnSelection += OnSelection;
		MoveAction.OnMove += OnMove;
	}

	void OnDisable() {
		Selection.OnSelection -= OnSelection;
		MoveAction.OnMove -= OnMove;
	}

	void Start() {
		selectedObjects = new List<GameObject>();
	}

	void OnSelection(IList<GameObject> selectedObjects) {
		this.selectedObjects = selectedObjects;
		Debug.Log("[" + GetInstanceID() + "] OnSelection : " + this.selectedObjects.Count);
	}

	void OnMove(int playerId, Camera camera, Vector3 mousePosition) {
		Debug.Log("[" + GetInstanceID() + "] OnSelection : " + this.selectedObjects.Count);
		for (int i = 0; i < selectedObjects.Count; ++i) {
			Debug.Log("[" + GetInstanceID() + "] Move object " + i);
			selectedObjects[i].GetComponent<Movement>().Move(playerId, camera, mousePosition);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO Divide preSelected in 2 lists: one for playerColor, one for others.
public class Selection : GenericSelection {

	protected Color playerColor;
	IList<GameObject> playerPreSelected;
	IList<GameObject> ennemyPreSelected;

	public delegate void ESelection(IList<GameObject> listObjects);

	public static event ESelection OnSelection;

	protected override void OnEnable() {
		base.OnEnable();
		Unit.OnDeath += OnSelectableDestroy;
	}

	void PreSelected(GameObject go) {
		var unit = go.GetComponent<Unit>();
		if (unit == null)
			return;

		if (unit.color == playerColor) {
			if (!playerPreSelected.Contains(go)) {
				playerPreSelected.Add(go);
			}
		} else {
			if (!ennemyPreSelected.Contains(go) && playerPreSelected.Count == 0) {
				ennemyPreSelected.Add(go);
			}
		}
	}

	protected override void OnUpdateDrag(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
		if (!isDragging) {
			timeLeftBeforeDragging -= Time.deltaTime;
		} else {
			for (int i = 0; i < selectableObjects.Count; ++i) {
				if (IsInRect(selectableObjects[i], originWorldPoint)) {
					PreSelected(selectableObjects[i]);
				} else if (playerPreSelected.Remove(selectableObjects[i]) || ennemyPreSelected.Remove(selectableObjects[i])) {
					selectableObjects[i].GetComponent<HealthBar>().HideHealthBar();
				}
			}
			if (playerPreSelected.Count > 0) {
				foreach (var unit in playerPreSelected) {
					unit.GetComponent<HealthBar>().ShowHealthBar();
				}
				foreach (var unit in ennemyPreSelected) {
					unit.GetComponent<HealthBar>().HideHealthBar();
				}
				ennemyPreSelected.Clear();
			} else if (ennemyPreSelected.Count > 0) {
				foreach (var unit in ennemyPreSelected) {
					unit.GetComponent<HealthBar>().ShowHealthBar();
				}
				foreach (var unit in playerPreSelected) {
					unit.GetComponent<HealthBar>().HideHealthBar();
				}
				playerPreSelected.Clear();
			}
		}
		if (timeLeftBeforeDragging <= 0f) {
			isDragging = true;
		}
	}

	protected override void Start() {
		selectedObjects = new List<GameObject>();
		selectableObjects = new List<GameObject>();
		foreach (Transform child in transform) {
			selectableObjects.Add(child.gameObject);
		}
		timeLeftBeforeDragging = .15f;
		isDragging = false;
		playerColor = Color.blue;
		playerPreSelected = new List<GameObject>();
		ennemyPreSelected = new List<GameObject>();
	}

	protected override void SelectUnits(Vector3 originWorldPoint) {
		if (playerPreSelected.Count > 0) {
			selectedObjects = playerPreSelected;
		} else if (ennemyPreSelected.Count > 0) {
			selectedObjects = ennemyPreSelected;
		}
		if (!isDragging) {
			foreach (var unit in selectedObjects) {
				unit.GetComponent<HealthBar>().ShowHealthBar();
			}
		}
		ApplySelection();
	}

	protected override void ApplySelection() {
		if (selectedObjects.Count == 1) { // show healthbar for selection of one unit
			selectedObjects[0].GetComponent<HealthBar>().ShowHealthBar();
		}
		if (OnSelection != null)
			OnSelection(selectedObjects);
	}

	protected override void ApplyDeselection() {
		foreach (var go in selectedObjects) {
			go.GetComponent<HealthBar>().HideHealthBar();
		}
	}

	void OnSelectableDestroy(GameObject go) {
		selectableObjects.Remove(go);
		selectedObjects.Remove(go);
		if (!playerPreSelected.Remove(go)) {
			ennemyPreSelected.Remove(go);
		}
	}
}

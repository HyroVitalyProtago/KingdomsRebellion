﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//TODO Divide preSelected in 2 lists: one for playerColor, one for others.
public class MouseSelection : GenericMouseSelection {

    protected Color playerColor;
    IList<GameObject> playerPreSelected;
    IList<GameObject> ennemyPreSelected;

    protected override void OnEnable() {
        base.OnEnable();
        Unit.OnDeath += OnSelectableDestroy;
    } 

    void PreSelected(GameObject go) {
        var unit = go.GetComponent<Unit>();
        if (unit == null) return;

        if (unit.color == playerColor) {
            if (!playerPreSelected.Contains(go)) {
                playerPreSelected.Add(go);
            }
        } else{
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
                } else if (playerPreSelected.Remove(selectableObjects[i])) {
                    selectableObjects[i].GetComponent<HealthBar>().HideHealthBar();
                } else if (ennemyPreSelected.Remove(selectableObjects[i])) {
                    selectableObjects[i].GetComponent<HealthBar>().HideHealthBar();
                }
            }
            if (playerPreSelected.Count > 0) {
                foreach (var unit in playerPreSelected) {
                    var healthBar = unit.GetComponent<HealthBar>();
                    healthBar.ShowHealthBar();
                }
                foreach (var unit in ennemyPreSelected) {
                    var healthBar = unit.GetComponent<HealthBar>();
                    healthBar.HideHealthBar();
                }
                ennemyPreSelected.Clear();
            } else if (ennemyPreSelected.Count > 0) {
                foreach (var unit in ennemyPreSelected) {
                    var healthBar = unit.GetComponent<HealthBar>();
                    healthBar.ShowHealthBar();
                }
                foreach (var unit in playerPreSelected) {
                    var healthBar = unit.GetComponent<HealthBar>();
                    healthBar.HideHealthBar();
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
        print (selectedObjects.Count);
        ApplySelection();
    }

    protected override void ApplySelection() {
        foreach (var go in selectedObjects) {
            var unit = go.GetComponent<Unit>();
            unit.ApplySelection();
        }
    }

    protected override void ApplyDeselection() {
        foreach (var go in selectedObjects) {
            var unit = go.GetComponent<Unit>();
            unit.ApplyDeselection();
        }
    }

    void OnSelectableDestroy(GameObject go) {
        selectableObjects.Remove(go);
        selectedObjects.Remove(go);
        if(!playerPreSelected.Remove(go)) {
            ennemyPreSelected.Remove(go);
        }
    }
}

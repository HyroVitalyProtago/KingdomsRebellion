using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections.Generic;


// TODO: How do we do when target isn't reachable ?
// And with more than one unit ?
// This script must control all the movement of units and create transform to set target if needed
public class MouseMove : MonoBehaviour {

    private Ray ray;
    private RaycastHit hit;
    private Dictionary<GameObject, Transform> targets;
    private AICharacterControl aiCharacterControl;

    void OnEnable() {
        MouseInput.OnRightClick += OnRightClick;
    }

    void OnDisable() {
        MouseInput.OnRightClick -= OnRightClick;
    }

	void Start () {
        aiCharacterControl = gameObject.GetComponent<AICharacterControl>();
	}
	
	void Update () {
        if (Input.GetMouseButtonUp(1)) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
               // target.transform.position = hit.point;
              //  MoveUnits(target.transform);
                if (hit.collider.CompareTag("Unit")) {
                    var unit = hit.collider.GetComponent<Unit>();
                   /* if (unit.color != playerColor) {
                        foreach (var ally in selectedObjects) {
                            var allyUnit = ally.GetComponent<Unit>();
                            allyUnit.ennemyTargeted = unit;
                            allyUnit.attacking = true;
                        }
                    }*/
                }
            }

            if (aiCharacterControl.target != null && (transform.position - aiCharacterControl.target.transform.position).magnitude < 0.3f) {
                aiCharacterControl.SetTarget(null);
            }
        }
	}

    void MoveUnits(Transform newtrans) {
        /*foreach (var unit in selectedObjects) {
                AICharacterControl aichar = unit.GetComponent<AICharacterControl>();
                aichar.SetTarget(newtrans);
        }*/
    }

    void OnRightClick(int playerId, Camera camera, Vector3 mousePosition) {

    }
}

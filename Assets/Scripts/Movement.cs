using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections.Generic;


// TODO: How do we do when target isn't reachable ?
// And with more than one unit ?
// This script must control all the movement of units and create transform to set target if needed
public class Movement : MonoBehaviour {

    private Ray ray;
    private RaycastHit hit;
    private AICharacterControl aiCharacterControl;
    private Object targetPrefab;
    private Unit unit;

	void Start () {
        aiCharacterControl = gameObject.GetComponent<AICharacterControl>();
        targetPrefab = Resources.Load("Prefabs/Target");
        unit = this.GetComponent<Unit>();
	}
	
	void Update () {
            if (aiCharacterControl.target != null && (transform.position - aiCharacterControl.target.transform.position).magnitude < 1f) {
                var go = aiCharacterControl.target.gameObject;
                aiCharacterControl.SetTarget(null);
                if (go.CompareTag("Target")) {
                    Destroy(go);
                }
            }
	}


    public void Move(int playerId, Camera camera, Vector3 mousePosition) {
        ray = camera.ScreenPointToRay(mousePosition);
        if (aiCharacterControl.target != null) {
            var go = aiCharacterControl.target.gameObject;
            aiCharacterControl.SetTarget(null);
            if (go.CompareTag("Target")) {
                Destroy(go);
            }
        }
        if (unit.playerId == Mouse.PlayerId && Physics.Raycast(ray.origin, ray.direction, out hit)) {
            var ennemy = hit.collider.gameObject.GetComponent<Unit>();
            if (ennemy != null && ennemy.playerId != Mouse.PlayerId) {
                unit.attacking = true;
                unit.ennemyTargeted = ennemy;
                aiCharacterControl.SetTarget(ennemy.transform);
            } else {
                GameObject target = GameObject.Instantiate(targetPrefab, hit.point, Quaternion.identity) as GameObject;
                aiCharacterControl.SetTarget(target.transform);
            }
        }
    }
}

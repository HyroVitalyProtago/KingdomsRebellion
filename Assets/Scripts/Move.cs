using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class Move : MonoBehaviour {

    private AICharacterControl aiCharacterControl;
	// Use this for initialization
	void Start () {
        aiCharacterControl = gameObject.GetComponent<AICharacterControl>();
	}
	
	// Update is called once per frame
	void Update () {
        if (aiCharacterControl.target != null && (transform.position - aiCharacterControl.target.transform.position).magnitude < 0.3f) {
            aiCharacterControl.SetTarget(null);
        }
	}
}

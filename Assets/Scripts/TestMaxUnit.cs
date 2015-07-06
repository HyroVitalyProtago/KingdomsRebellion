using UnityEngine;
using System.Collections;

public class TestMaxUnit : MonoBehaviour {

    public GameObject unit;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 200; i++) {
            Instantiate(unit, Vector3.zero, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}

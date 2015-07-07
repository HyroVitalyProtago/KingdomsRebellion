using UnityEngine;
using System.Collections;

public class UnitSpawner : MonoBehaviour {

	public float secondsWaiting;
	public GameObject prefab;

	static Transform dynamics;

	void Awake() {
		if (dynamics == null) dynamics = GameObject.Find("/Dynamics/Units").transform;
	}

	void Start() {
		StartCoroutine(SpawnEnumerator());
	}

	void Spawn() {
		float x = Random.Range(-1f,1f);
		float y = transform.localScale.y * .5f;
		float z = Random.Range(-1f,1f);
		GameObject o = Instantiate(
			prefab,
			transform.position + new Vector3(x,y,z),
			Quaternion.identity
		) as GameObject;
		o.transform.SetParent(dynamics);
	}

	IEnumerator SpawnEnumerator() {
		while(true) {
			Spawn();
			yield return new WaitForSeconds(secondsWaiting);
		}
	}

}

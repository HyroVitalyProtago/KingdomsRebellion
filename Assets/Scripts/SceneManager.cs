using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

	public static SceneManager Manager;

	public List<IHasGameFrame> GameFrameObjects;

	void Awake() {
		Manager = this;
		GameFrameObjects = new List<IHasGameFrame>();
	}
}

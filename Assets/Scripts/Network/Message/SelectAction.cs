using UnityEngine;
using System;
using System.Collections;

// Camera (Vector3 position, Vector3 rotation, int size)
// MousePosition (Vector3)
public class SelectAction : GameAction {

	public delegate void ESelectAction(int playerId,Camera camera,Vector3 mousePosition);

	public static event ESelectAction OnSelect;

	Vector3 cameraPosition;
	Vector3 cameraRotation;
	float cameraOrthographicSize;
	Vector3 mousePosition;

	public static new SelectAction FromBytes(byte[] data, int size) {
		return new SelectAction(
			BitConverter.ToInt32(data, 1),
			NetworkMessage.Vector3FromBytes(data, 5),
			NetworkMessage.Vector3FromBytes(data, 37),
			BitConverter.ToSingle(data, 41),
			NetworkMessage.Vector3FromBytes(data, 45)
		);
	}
	
	public SelectAction(int lockStepTurn, Vector3 cameraPosition, Vector3 cameraRotation, float cameraOrthographicSize, Vector3 mousePosition) : base(lockStepTurn) {
		this.cameraPosition = cameraPosition;
		this.cameraRotation = cameraRotation;
		this.cameraOrthographicSize = cameraOrthographicSize;
		this.mousePosition = mousePosition;
	}

	public override void Process(int playerID) {
		GameObject playerCamera = GameObject.Find("Cameras/Camera (" + playerID + ")") as GameObject;
		playerCamera.transform.position = cameraPosition;
		playerCamera.transform.localEulerAngles = cameraRotation;
			
		Camera camera = playerCamera.GetComponent<Camera>();
		camera.orthographicSize = cameraOrthographicSize;

		if (OnSelect != null) {
			OnSelect(playerID, camera, mousePosition);
		}
	}
	
	public override byte[] ToBytes() {
		byte[] data = new byte[NetworkAPI.bufferSize];
		int offset = 0;
		
		data[offset] = (byte)GameActionEnum.SelectAction;
		offset = AddInt(data, LockStepTurn, ++offset);
		offset = AddVector3(data, cameraPosition, offset);
		offset = AddVector3(data, cameraRotation, offset);
		offset = AddFloat(data, cameraOrthographicSize, offset);
		offset = AddVector3(data, mousePosition, offset);

		return data;
	}

}


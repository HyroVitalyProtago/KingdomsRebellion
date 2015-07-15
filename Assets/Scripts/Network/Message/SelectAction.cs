using UnityEngine;
using System;
using System.IO;

public class SelectAction : GameAction {

	public delegate void ESelectAction(int playerId, Camera camera, Vector3 mousePosition);
	public static event ESelectAction OnSelect;

	Vector3 cameraPosition;
	Vector3 cameraRotation;
	float cameraOrthographicSize;
	protected Vector3 mousePosition;
	protected GameObject playerCamera;
	protected Camera camera;

	public static SelectAction FromBytes(byte[] data) {
		return new SelectAction().GetFromBytes(data) as SelectAction;
	}
	
	public SelectAction(int lockStepTurn, Vector3 cameraPosition, Vector3 cameraRotation, float cameraOrthographicSize, Vector3 mousePosition) : base(lockStepTurn) {
		this.cameraPosition = cameraPosition;
		this.cameraRotation = cameraRotation;
		this.cameraOrthographicSize = cameraOrthographicSize;
		this.mousePosition = mousePosition;
	}

	protected SelectAction() : base() {
	}

	protected void SetPlayerData(int playerID) {
		playerCamera = GameObject.Find("Cameras/Camera (" + playerID + ")") as GameObject;
		playerCamera.transform.position = cameraPosition;
		playerCamera.transform.localEulerAngles = cameraRotation;
		
		camera = playerCamera.GetComponent<Camera>();
		camera.orthographicSize = cameraOrthographicSize;
	}

	public override void Process(int playerID) {
		SetPlayerData(playerID);

		if (OnSelect != null) {
			OnSelect(playerID, camera, mousePosition);
		}
	}

	public override byte ActionType() {
		return (byte) GameActionEnum.SelectAction;
	}

	protected override void Serialize(BinaryWriter writer) {
		base.Serialize(writer);
		SerializeVector3(cameraPosition, writer);
		SerializeVector3(cameraRotation, writer);
		writer.Write(cameraOrthographicSize);
		SerializeVector3(mousePosition, writer);
	}
	
	protected override void Deserialize(BinaryReader reader) {
		base.Deserialize(reader);
		cameraPosition = DeserializeVector3(reader);
		cameraRotation = DeserializeVector3(reader);
		cameraOrthographicSize = reader.ReadSingle();
		mousePosition = DeserializeVector3(reader);
	}

}


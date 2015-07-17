using UnityEngine;

namespace KingdomsRebellion.Network.Link {
	public class MoveAction : SelectAction {

		public delegate void EMoveAction(int playerId,Camera camera,Vector3 mousePosition);

		public static event EMoveAction OnMove;

		public static new MoveAction FromBytes(byte[] data) {
			return new MoveAction().GetFromBytes(data) as MoveAction;
		}

		public MoveAction(
		int lockStepTurn,
		Vector3 cameraPosition,
		Vector3 cameraRotation,
		float cameraOrthographicSize,
		Vector3 mousePosition) : base(lockStepTurn, cameraPosition, cameraRotation, cameraOrthographicSize, mousePosition) {
		}
	
		protected MoveAction() : base() {
		}

		public override byte ActionType() {
			return (byte)GameActionEnum.MoveAction;
		}

		public override void Process(int playerID) {
			Debug.Log("MoveAction :: Process :: playerID == " + playerID);

			SetPlayerData(playerID);
		
			if (OnMove != null) {
				OnMove(playerID, camera, mousePosition);
			}
		}
	}
}
using System;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Network.Link {

	/// <summary>
	/// Action send over the network for move units.
	/// </summary>
	public class MoveAction : SelectAction {

		public static event Action<int, Vec3> OnMove;

		public static new MoveAction FromBytes(byte[] data) {
			return new MoveAction().GetFromBytes(data) as MoveAction;
		}

		public MoveAction(uint lockStepTurn, Vec3 modelPoint) : base(lockStepTurn, modelPoint) {}
	
		protected MoveAction() {}

		public override byte ActionType() {
			return (byte) GameActionEnum.MoveAction;
		}

		public override void Process(int playerID) {
			if (OnMove != null) {
				OnMove(playerID, _modelPoint);
			}
		}
	}

}
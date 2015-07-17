﻿
namespace KingdomsRebellion.Network.Link {

	/// <summary>
	/// Default action send over the network with no behaviour.
	/// </summary>
	public sealed class NoAction : GameAction {

		public static NoAction FromBytes(byte[] data) {
			return new NoAction().GetFromBytes(data) as NoAction;
		}

		public NoAction(uint lockStepTurn) : base(lockStepTurn) {}

		private NoAction() : base() {}

		public override byte ActionType() {
			return (byte) GameActionEnum.NoAction;
		}
	}

}
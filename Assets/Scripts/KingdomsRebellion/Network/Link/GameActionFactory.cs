using System;

namespace KingdomsRebellion.Network.Link {
	public static class GameActionFactory {
		public static GameAction Get(byte[] data) {
			byte b = data[0];

			if (b == (byte) GameActionEnum.NoAction) {
				return NoAction.FromBytes(data);
			} else if (b == (byte) GameActionEnum.SelectAction) {
				return SelectAction.FromBytes(data);
			} else if (b == (byte) GameActionEnum.DragAction) {
				 return DragAction.FromBytes(data);
			} else if (b == (byte) GameActionEnum.MoveAction) {
				return MoveAction.FromBytes(data);
            } else if (b == (byte) GameActionEnum.AttackAction) {
                return AttackAction.FromBytes(data);
            }

		    throw new ArgumentException("GameActionFactory :: Get : data type don't correspond a known GameAction");
		}
	}
}
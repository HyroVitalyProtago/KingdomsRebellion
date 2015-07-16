using System;
using System.IO;

namespace KingdomsRebellion.Network.Link {
	public abstract class GameAction : NetworkMessage {

		// Template for GameAction static builder
		// public static GameAction FromBytes(byte[] data) {
		//	return new GameAction().GetFromBytes(data) as GameAction;
		// }

		public GameAction(int lockStepTurn) : base(lockStepTurn) {
		}

		protected GameAction() : base() {
		}

		public virtual void Process(int playerID) {
		}

		public abstract byte ActionType();

		protected override void Serialize(BinaryWriter writer) {
			writer.Write(ActionType());
			base.Serialize(writer);
		}

		protected override void Deserialize(BinaryReader reader) {
			if (reader.ReadByte() != ActionType()) {
				throw new ArgumentException("GameAction :: Deserialize => Bad ActionType for deserialization : " + ActionType().ToString() + " != " + ((byte)ActionType()));
			}
			base.Deserialize(reader);
		}
	}

}
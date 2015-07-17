using System.IO;

namespace KingdomsRebellion.Network.Link {
	public sealed class GameConfirmation : NetworkMessage {

		public bool Wait { get; private set; }

		public static GameConfirmation FromBytes(byte[] data) {
			return new GameConfirmation().GetFromBytes(data) as GameConfirmation;
		}

		public GameConfirmation(int lockStepTurn, bool wait) : base(lockStepTurn) {
			Wait = wait;
		}

		private GameConfirmation() {
		}

		protected override void Serialize(BinaryWriter writer) {
			base.Serialize(writer);
			writer.Write(Wait);
		}

		protected override void Deserialize(BinaryReader reader) {
			base.Deserialize(reader);
			Wait = reader.ReadBoolean();
		}

	}
}
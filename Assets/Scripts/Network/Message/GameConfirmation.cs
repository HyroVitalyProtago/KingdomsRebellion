using System.IO;

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

	protected sealed override void Serialize(BinaryWriter writer) {
		base.Serialize(writer);
		writer.Write(Wait);
	}

	protected sealed override void Deserialize(BinaryReader reader) {
		base.Deserialize(reader);
		Wait = reader.ReadBoolean();
	}

}

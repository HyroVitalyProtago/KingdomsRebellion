using UnityEngine;
using System.IO;

public abstract class NetworkMessage {

	public int LockStepTurn { get; set; }

	protected NetworkMessage(int lockStepTurn) {
		LockStepTurn = lockStepTurn;
	}

	protected NetworkMessage() : this(-1) {
	}

	protected NetworkMessage GetFromBytes(byte[] data) {
		using (MemoryStream m = new MemoryStream(data)) {
			using (BinaryReader reader = new BinaryReader(m)) {
				Deserialize(reader);
			}
		}
		return this;
	}

	public byte[] ToBytes() {
		using (MemoryStream m = new MemoryStream(new byte[NetworkAPI.bufferSize])) {
			using (BinaryWriter writer = new BinaryWriter(m)) {
				Serialize(writer);
			}
			return m.ToArray();
		}
	}

	protected virtual void Serialize(BinaryWriter writer) {
		writer.Write(LockStepTurn);
	}

	protected virtual void Deserialize(BinaryReader reader) {
		LockStepTurn = reader.ReadInt32();
	}

	// TOOLS
	protected void SerializeVector3(Vector3 v3, BinaryWriter writer) {
		writer.Write(v3.x);
		writer.Write(v3.y);
		writer.Write(v3.z);
	}

	protected Vector3 DeserializeVector3(BinaryReader reader) {
		Vector3 v3 = new Vector3();
		v3.x = reader.ReadSingle();
		v3.y = reader.ReadSingle();
		v3.z = reader.ReadSingle();
		return v3;
	}

}

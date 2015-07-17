using System;
using System.IO;
using KingdomsRebellion.Network;

namespace KingdomsRebellion.Network.Link {
	public abstract class NetworkMessage {

		uint? _lockstepTurn;
		public uint LockstepTurn {
			get {
				return _lockstepTurn.Value; // throw InvalidStateException if null
			}
			set {
				_lockstepTurn = (uint?) value;
			}
		}

		protected NetworkMessage(uint lockstepTurn) {
			_lockstepTurn = (uint?) lockstepTurn;
		}

		protected NetworkMessage() {}

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
			writer.Write(LockstepTurn);
		}

		protected virtual void Deserialize(BinaryReader reader) {
			LockstepTurn = reader.ReadUInt32();
		}

	}
}
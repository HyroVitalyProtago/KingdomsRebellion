using UnityEngine;
using System;
using System.Collections;

public abstract class NetworkMessage {

	public int LockStepTurn { get; private set; }

//	protected static Object FromBytes(byte[] bytes) { return ByteArrayToObject(bytes); }

	public NetworkMessage(int lockStepTurn) { LockStepTurn = lockStepTurn; }

	public abstract byte[] ToBytes();

	protected static Vector3 Vector3FromBytes(byte[] data, int offset) {
		return new Vector3(
			BitConverter.ToSingle(data, offset),
			BitConverter.ToSingle(data, offset+4),
			BitConverter.ToSingle(data, offset+8)
		);
	}


	// return new offset
	protected int AddInt(byte[] data, int _int, int offset) {
		byte[] bytes = BitConverter.GetBytes(_int);
		int i = offset;
		for ( ; i < bytes.Length; ++i) {
			data[i] = bytes[i];
		}
		return ++i;
	}

	protected int AddFloat(byte[] data, float _float, int offset) {
		byte[] bytes = BitConverter.GetBytes(_float);
		int i = offset;
		for ( ; i < bytes.Length; ++i) {
			data[i] = bytes[i];
		}
		return ++i;
	}

	protected int AddVector3(byte[] data, Vector3 _vector3, int offset) {
		offset += AddFloat(data, _vector3.x, offset);
		offset += AddFloat(data, _vector3.y, offset);
		offset += AddFloat(data, _vector3.z, offset);
		return offset;
	}
	
	// TODO NetworkMessage better serialization
	/*
	public byte[] Serialize() {
      using (MemoryStream m = new MemoryStream()) {
         using (BinaryWriter writer = new BinaryWriter(m)) {
            writer.Write(Id);
            writer.Write(Name);
         }
         return m.ToArray();
      }
   }

   public static MyClass Desserialize(byte[] data) {
      MyClass result = new MyClass();
      using (MemoryStream m = new MemoryStream(data)) {
         using (BinaryReader reader = new BinaryReader(m)) {
            result.Id = reader.ReadInt32();
            result.Name = reader.ReadString();
         }
      }
      return result;
   }
   */
}

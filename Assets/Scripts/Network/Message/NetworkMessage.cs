using UnityEngine;
using System.Collections;

public abstract class NetworkMessage {

	public int LockStepTurn { get; private set; }

//	protected static Object FromBytes(byte[] bytes) { return ByteArrayToObject(bytes); }

	public NetworkMessage(int lockStepTurn) { LockStepTurn = lockStepTurn; }

	public void Process() {}

	public abstract byte[] ToBytes();

}

using UnityEngine;
using System.Collections;
using System;

public class GameAction : NetworkMessage {

	public static GameAction FromBytes(byte[] data, int size) {
		return new GameAction(BitConverter.ToInt32(data, 0));
	}

	public GameAction(int lockStepTurn) : base(lockStepTurn) {
	}

	public override byte[] ToBytes() {
		byte[] data = new byte[NetworkAPI.bufferSize];
		
		byte[] lockStepTurn = BitConverter.GetBytes(LockStepTurn);
		for (int i = 0; i < lockStepTurn.Length; ++i) {
			data[i] = lockStepTurn[i];
		}

		return data;
	}
}

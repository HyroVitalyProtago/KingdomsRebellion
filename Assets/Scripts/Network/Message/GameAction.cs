using UnityEngine;
using System.Collections;
using System;

public class GameAction : NetworkMessage {

	public static GameAction FromBytes(byte[] data, int size) {
		return new GameAction(BitConverter.ToInt32(data, 1));
	}

	public GameAction(int lockStepTurn) : base(lockStepTurn) {
	}

	public virtual void Process(int playerID) {}

	public override byte[] ToBytes() {
		byte[] data = new byte[NetworkAPI.bufferSize];

		int i = 0;

		data[i] = (byte) GameActionEnum.NoAction;

		byte[] lockStepTurn = BitConverter.GetBytes(LockStepTurn);
		for (++i ; i < lockStepTurn.Length; ++i) {
			data[i] = lockStepTurn[i];
		}

		return data;
	}
}

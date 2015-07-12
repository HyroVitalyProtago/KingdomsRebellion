using UnityEngine;
using System.Collections;
using System;

public class GameConfirmation : NetworkMessage {

	public bool Wait { get; private set; }

	public static GameConfirmation FromBytes(byte[] data, int size) {
		return new GameConfirmation(BitConverter.ToInt32(data, 0), Convert.ToBoolean(data[4]));
	}

	public GameConfirmation(int lockStepTurn, bool wait) : base(lockStepTurn) {
		Wait = wait;
	}

	public override byte[] ToBytes() {
		byte[] data = new byte[NetworkAPI.bufferSize];

		int i = 0;

		byte[] lockStepTurn = BitConverter.GetBytes(LockStepTurn);
		for (; i < lockStepTurn.Length; ++i) {
			data[i] = lockStepTurn[i];
		}

		byte wait = Convert.ToByte(Wait);
		data[++i] = wait;

		return data;
	}
}

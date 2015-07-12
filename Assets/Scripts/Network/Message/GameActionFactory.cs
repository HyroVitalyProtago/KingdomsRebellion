using UnityEngine;
using System;
using System.Collections;

public static class GameActionFactory {
	public static GameAction GetGameAction(byte[] data, int size) {
		byte b = data[0];

		if (b == (byte) GameActionEnum.NoAction) {
			return GameAction.FromBytes(data, size);
		} else if (b == (byte) GameActionEnum.SelectAction) {
			return SelectAction.FromBytes(data, size);
		} else if (b == (byte) GameActionEnum.DragAction) {
//			return DragAction.FromBytes(data, size);
		} else if (b == (byte) GameActionEnum.MoveAction) {
//			return MoveAction.FromBytes(data, size);
		}

		throw new ArgumentException("data type don't correspond a known GameAction");
	}
}

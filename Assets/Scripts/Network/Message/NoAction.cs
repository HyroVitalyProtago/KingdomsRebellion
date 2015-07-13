
public sealed class NoAction : GameAction {

	public static NoAction FromBytes(byte[] data) {
		return new NoAction().GetFromBytes(data) as NoAction;
	}

	public NoAction(int lockStepTurn) : base(lockStepTurn) {
	}

	private NoAction() : base() {
	}

	public override byte ActionType() {
		return (byte) GameActionEnum.NoAction;
	}
}

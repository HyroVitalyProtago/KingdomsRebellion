using System;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Network.Link {

    /// <summary>
    /// Action send over the network for move units.
    /// </summary>
    public class AttackAction : SelectAction {

		event Action<int, Vec2> OnAttack;

		public static new AttackAction FromBytes(byte[] data) {
            return new AttackAction().GetFromBytes(data) as AttackAction;
		}

		public AttackAction(Vec2 modelPoint) : base(modelPoint) {}

        protected AttackAction() { }

		public override byte ActionType() {
            return (byte)GameActionEnum.AttackAction;
		}

		public override void Process(int playerID) {
            Offer("OnAttack");
            if (OnAttack != null) {
                OnAttack(playerID, _modelPoint);
			}
            Denial("OnAttack");
		}
	}
}

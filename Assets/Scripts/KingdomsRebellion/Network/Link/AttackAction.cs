using System;
using UnityEngine;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.FSM;
using System.IO;

namespace KingdomsRebellion.Network.Link {

    /// <summary>
    /// Action send over the network for move units.
    /// </summary>
    public class AttackAction : GameAction {

		protected Vec2 _modelPoint;

		public static new AttackAction FromBytes(byte[] data) {
            return new AttackAction().GetFromBytes(data) as AttackAction;
		}

		public AttackAction(Vec2 modelPoint) {
			_modelPoint = modelPoint;
		}

        protected AttackAction() { }

		public override byte ActionType() {
            return (byte)GameActionEnum.AttackAction;
		}

		public override Action<GameObject> GetAction() {
			return delegate(GameObject go) {
				go.GetComponent<FiniteStateMachine>().Attack(_modelPoint);
			};
		}

		protected override void Serialize(BinaryWriter writer) {
			base.Serialize(writer);
			_modelPoint.Serialize(writer);
		}
		
		protected override void Deserialize(BinaryReader reader) {
			base.Deserialize(reader);
			_modelPoint = Vec2.Deserialize(reader);
		}
	}
}

using System;
using System.IO;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {

	/// <summary>
	/// Action send over the network for move units.
	/// </summary>
	public class MoveAction : GameAction {

		protected Vec2 _modelPoint;

		public static MoveAction FromBytes(byte[] data) {
			return new MoveAction().GetFromBytes(data) as MoveAction;
		}

		public MoveAction(Vec2 modelPoint) {
			_modelPoint = modelPoint;
		}
	
		protected MoveAction() {}

		public override byte ActionType() {
			return (byte) GameActionEnum.MoveAction;
		}

		public override Action<GameObject> GetAction() {
			return delegate(GameObject go) {
                FiniteStateMachine fsm = go.GetComponent<FiniteStateMachine>();
			    if (fsm != null) {
			        fsm.Move(_modelPoint);
			    }
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
using System;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core;
using UnityEngine;
using KingdomsRebellion.Core.FSM;
using System.IO;

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
				go.GetComponent<FiniteStateMachine>().Move(_modelPoint);
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
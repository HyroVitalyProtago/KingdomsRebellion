using System;
using System.IO;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {
	
	/// <summary>
	/// Action send over the network for select units.
	/// </summary>
	public class RallyAction : GameAction {
		
		protected Vec2 _modelPoint;
		
		public static RallyAction FromBytes(byte[] data) {
			return new RallyAction().GetFromBytes(data) as RallyAction;
		}
		
		public RallyAction(Vec2 modelPoint) {
			_modelPoint = modelPoint;
		}
		
		protected RallyAction() {}
		
		public override Action<GameObject> GetAction() {
			return delegate(GameObject go) {
				go.GetComponent<KRSpawn>().RallyPoint = _modelPoint;
			};
		}
		
		public override byte ActionType() {
			return (byte) GameActionEnum.RallyAction;
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
using System;
using System.IO;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Network.Link {
	
	/// <summary>
	/// Action send over the network for select multiple units.
	/// </summary>
	public class DragAction : GameAction {
		
		static event Action<int, Vec2, Vec2, Vec2> OnDragAction;

		public static new void Awake() {
			EventConductor.Offer(typeof(DragAction), "OnDragAction");
		}
		
		protected Vec2 _beginModelPoint;
		protected Vec2 _endModelPoint;
		protected Vec2 _z;
		
		public static DragAction FromBytes(byte[] data) {
			return new DragAction().GetFromBytes(data) as DragAction;
		}
		
		public DragAction(Vec2 beginModelPoint, Vec2 endModelPoint, Vec2 z) {
			_beginModelPoint = beginModelPoint;
			_endModelPoint = endModelPoint;
			_z = z;
		}
		
		protected DragAction() {}
		
		public override void Process(int playerID) {
			if (OnDragAction != null) {
				OnDragAction(playerID, _beginModelPoint, _endModelPoint, _z);
			}
		}
		
		public override byte ActionType() {
			return (byte) GameActionEnum.DragAction;
		}
		
		protected override void Serialize(BinaryWriter writer) {
			base.Serialize(writer);
			_beginModelPoint.Serialize(writer);
			_endModelPoint.Serialize(writer);
			_z.Serialize(writer);
		}
		
		protected override void Deserialize(BinaryReader reader) {
			base.Deserialize(reader);
			_beginModelPoint = Vec2.Deserialize(reader);
			_endModelPoint = Vec2.Deserialize(reader);
			_z = Vec2.Deserialize(reader);
		}
		
	}
	
}
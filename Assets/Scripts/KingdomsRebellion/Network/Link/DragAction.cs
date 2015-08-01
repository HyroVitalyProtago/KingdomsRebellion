using UnityEngine;
using System;
using System.IO;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Network.Link {
	
	/// <summary>
	/// Action send over the network for select multiple units.
	/// </summary>
	public class DragAction : GameAction {
		
		event Action<int, Vec3, Vec3, Vec3> OnModelDrag;
		
		protected Vec3 _beginModelPoint;
		protected Vec3 _endModelPoint;
		protected Vec3 _z;
		
		public static DragAction FromBytes(byte[] data) {
			return new DragAction().GetFromBytes(data) as DragAction;
		}
		
		public DragAction(uint lockStepTurn, Vec3 beginModelPoint, Vec3 endModelPoint, Vec3 z) : base(lockStepTurn) {
			_beginModelPoint = beginModelPoint;
			_endModelPoint = endModelPoint;
			_z = z;
		}
		
		protected DragAction() {}
		
		public override void Process(int playerID) {
			Offer("OnModelDrag");
			if (OnModelDrag != null) {
				OnModelDrag(playerID, _beginModelPoint, _endModelPoint, _z);
			}
			Denial("OnModelDrag");
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
			_beginModelPoint = Vec3.Deserialize(reader);
			_endModelPoint = Vec3.Deserialize(reader);
			_z = Vec3.Deserialize(reader);
		}
		
	}
	
}
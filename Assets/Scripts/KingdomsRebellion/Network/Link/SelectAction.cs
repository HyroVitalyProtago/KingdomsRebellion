using System;
using System.IO;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core;

namespace KingdomsRebellion.Network.Link {

	/// <summary>
	/// Action send over the network for select units.
	/// </summary>
	public class SelectAction : GameAction {

		static event Action<int, Vec2> OnModelSelect;

		public static new void Awake() {
			EventConductor.Offer(typeof(SelectAction), "OnModelSelect");
		}

		protected Vec2 _modelPoint;

		public static SelectAction FromBytes(byte[] data) {
			return new SelectAction().GetFromBytes(data) as SelectAction;
		}
	
		public SelectAction(Vec2 modelPoint) {
			_modelPoint = modelPoint;
		}

		protected SelectAction() {}

		public override void Process(int playerID) {
			if (OnModelSelect != null) {
				OnModelSelect(playerID, _modelPoint);
			}
		}

		public override byte ActionType() {
			return (byte) GameActionEnum.SelectAction;
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
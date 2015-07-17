using UnityEngine;
using System;
using System.IO;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Network.Link {

	/// <summary>
	/// Action send over the network for select units.
	/// </summary>
	public class SelectAction : GameAction {

		public static event Action<int, Vec3> OnSelect;

		protected Vec3 _modelPoint;

		public static SelectAction FromBytes(byte[] data) {
			return new SelectAction().GetFromBytes(data) as SelectAction;
		}
	
		public SelectAction(uint lockStepTurn, Vec3 modelPoint) : base(lockStepTurn) {
			_modelPoint = modelPoint;
		}

		protected SelectAction() {}

		public override void Process(int playerID) {
			Debug.Log("SelectAction :: Process :: playerID == " + playerID);

			if (OnSelect != null) {
				OnSelect(playerID, _modelPoint);
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
			_modelPoint = Vec3.Deserialize(reader);
		}

	}

}
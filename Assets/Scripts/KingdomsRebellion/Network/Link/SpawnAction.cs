using System;
using System.IO;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {
	
	/// <summary>
	/// Action send over the network for select units.
	/// </summary>
	public class SpawnAction : GameAction {
		
		event Action<int, KeyCode> OnSpawn;
		
		protected KeyCode _keyCode;
		
		public static SpawnAction FromBytes(byte[] data) {
			return new SpawnAction().GetFromBytes(data) as SpawnAction;
		}
		
		public SpawnAction(KeyCode keyCode) {
			_keyCode = keyCode;
		}
		
		protected SpawnAction() {}
		
		public override void Process(int playerID) {
			Offer("OnSpawn");
			if (OnSpawn != null) {
				OnSpawn(playerID, _keyCode);
			}
			Denial("OnSpawn");
		}
		
		public override byte ActionType() {
			return (byte) GameActionEnum.SpawnAction;
		}
		
		protected override void Serialize(BinaryWriter writer) {
			base.Serialize(writer);
			writer.Write((Int32) _keyCode);
		}
		
		protected override void Deserialize(BinaryReader reader) {
			base.Deserialize(reader);
			_keyCode = (KeyCode) reader.ReadInt32();
		}
		
	}
	
}
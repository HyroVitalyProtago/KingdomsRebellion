using System;
using System.IO;
using KingdomsRebellion.Core.Components;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {
	
	/// <summary>
	/// Action send over the network for select units.
	/// </summary>
	public class SpawnAction : GameAction {
		
		protected KeyCode _keyCode;
		
		public static SpawnAction FromBytes(byte[] data) {
			return new SpawnAction().GetFromBytes(data) as SpawnAction;
		}
		
		public SpawnAction(KeyCode keyCode) {
			_keyCode = keyCode;
		}
		
		protected SpawnAction() {}
		
		public override Action<GameObject> GetAction() {
			return delegate(GameObject go) {
				switch (_keyCode) {
					case KeyCode.C : 
						go.GetComponent<KRSpawn>().Spawn("Infantry");
						break;
					case KeyCode.V :
						go.GetComponent<KRSpawn>().Spawn("Archer");
						break;
                    case KeyCode.T:
                        go.GetComponent<KRSpawn>().Spawn("Worker");
                        break;
				}
			};
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
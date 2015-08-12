using System;
using System.IO;
using KingdomsRebellion.Core.Components;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {

    /// <summary>
    /// Action send over the network for select units.
    /// </summary>
    public class BuildAction : GameAction {

        protected KeyCode _keyCode;

        public static BuildAction FromBytes(byte[] data) {
            return new BuildAction().GetFromBytes(data) as BuildAction;
        }

        public BuildAction(KeyCode keyCode) {
            _keyCode = keyCode;
        }

        protected BuildAction() { }

        public override Action<GameObject> GetAction() {
            return delegate(GameObject go) {
                       switch (_keyCode) {
                           case KeyCode.C:
                               go.GetComponent<KRBuild>().Build("Base");
                               break;
                           case KeyCode.V:
                               go.GetComponent<KRBuild>().Build("Barrack");
                               break;
                       }
                   };
        }

        public override byte ActionType() {
            return (byte)GameActionEnum.BuildAction;
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
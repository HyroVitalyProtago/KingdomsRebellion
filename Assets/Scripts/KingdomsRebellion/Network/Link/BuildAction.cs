using System;
using System.IO;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Network.Link {

    /// <summary>
    /// Action send over the network for select units.
    /// </summary>
    public class BuildAction : GameAction {

        protected KeyCode _keyCode;
        protected Vec2 _pos;

        public static BuildAction FromBytes(byte[] data) {
            return new BuildAction().GetFromBytes(data) as BuildAction;
        }

        public BuildAction(KeyCode keyCode, Vec2 pos) {
            _keyCode = keyCode;
            _pos = pos;
        }

        protected BuildAction() { }

        public override Action<GameObject> GetAction() {
            return delegate(GameObject go) {
                       switch (_keyCode) {
                           case KeyCode.C:
                               go.GetComponent<KRBuild>().Build("Base",_pos);
                               break;
                           case KeyCode.V:
                               go.GetComponent<KRBuild>().Build("Barrack",_pos);
                               break;
                       }
                   };
        }

        public override byte ActionType() {
            return (byte)GameActionEnum.BuildAction;
        }

        protected override void Serialize(BinaryWriter writer) {
            base.Serialize(writer);
            writer.Write((Int32)_keyCode);
            _pos.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader) {
            base.Deserialize(reader);
            _keyCode = (KeyCode)reader.ReadInt32();
            _pos = Vec2.Deserialize(reader);
        }
    }
}
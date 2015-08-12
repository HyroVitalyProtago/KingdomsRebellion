using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof (KRTransform))]
    public class KRBuild : KRBehaviour {

        private Transform _dynamics;
        private KRTransform _krtransform;

        private Vec2 _spawnPoint;
        public Vec2 RallyPoint { get; set; }
        private Dictionary<String, Object> _spawnableObjects;

        private Material _sweetBlue, _sweetRed;

        private void Awake() {
            _krtransform = GetComponent<KRTransform>();
            _spawnableObjects = new Dictionary<String, Object>();
            AddSpawnable("Base");
            AddSpawnable("Barrack");
            _sweetBlue = (Material) Resources.Load("Materials/SweetBlue", typeof (Material));
            _sweetRed = (Material) Resources.Load("Materials/SweetRed", typeof (Material));
        }

        private void Start() {
            GameObject go = GameObject.Find("KRGameObjects");
            if (go == null) {
                throw new SystemException("KRGameObjects not found in scene");
            }
            _dynamics = go.transform;

            Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

            RallyPoint = _krtransform.Pos + 2*toCenter;
            _spawnPoint = _krtransform.Pos + 2*toCenter;
        }

        public void AddSpawnable(String name) {
            _spawnableObjects.Add(name, Resources.Load("Prefabs/Buildings/" + name));
        }

        public void Build(String nameObj) {
            if (!KRFacade.IsEmpty(_spawnPoint)) return;

            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo =
                    Instantiate(_spawnableObjects[nameObj], _spawnPoint.ToVector3().Adjusted(), Quaternion.identity) as
                        GameObject;

                kgo.transform.SetParent(_dynamics);

                kgo.GetComponent<KRTransform>().PlayerID = _krtransform.PlayerID;
                if (_krtransform.PlayerID == 0) {
                    kgo.GetComponent<Renderer>().sharedMaterial = _sweetBlue;
                } else {
                    kgo.GetComponent<Renderer>().sharedMaterial = _sweetRed;
                }
            }
        }
    }
}

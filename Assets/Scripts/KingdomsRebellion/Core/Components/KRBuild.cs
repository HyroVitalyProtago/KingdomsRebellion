using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Inputs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KingdomsRebellion.Core.Components {

    [RequireComponent(typeof (KRTransform))]
    public class KRBuild : KRBehaviour {

        Transform _dynamics;
        KRTransform _krtransform;
        GameObject _building;
        public Vec2 RallyPoint { get; set; }
        Dictionary<String, Object> _spawnableObjects;
        Dictionary<String, Material> _materials;

        void Awake() {
            _krtransform = GetComponent<KRTransform>();
            _spawnableObjects = new Dictionary<String, Object>();
            _materials = new Dictionary<String, Material>();
            AddSpawnable("Base");
            AddSpawnable("Barrack");
            _materials.Add("SweetBlue", (Material)Resources.Load("Materials/SweetBlue", typeof(Material)));
            _materials.Add("SweetRed", (Material)Resources.Load("Materials/SweetRed", typeof(Material)));
            _materials.Add("TransparentBlue", (Material)Resources.Load("Materials/TransparentBlue", typeof(Material)));
            _materials.Add("TransparentRed", (Material)Resources.Load("Materials/TransparentRed", typeof(Material)));
        }

        void Start() {
            GameObject go = GameObject.Find("KRGameObjects");
            _building = null;
            if (go == null) {
                throw new SystemException("KRGameObjects not found in scene");
            }
            _dynamics = go.transform;

            Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

            RallyPoint = _krtransform.Pos + 2*toCenter;
        }

        public void AddSpawnable(String name) {
            _spawnableObjects.Add(name, Resources.Load("Prefabs/Buildings/" + name));
        }

        public void Build(String nameObj, Vec2 pos) {
            if (!KRFacade.IsEmpty(pos)) return;
            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo =
                    Instantiate(_spawnableObjects[nameObj], pos.ToVector3().Adjusted(), Quaternion.identity) as
                        GameObject;

                kgo.transform.SetParent(_dynamics);

                kgo.GetComponent<KRTransform>().PlayerID = _krtransform.PlayerID;
                if (_krtransform.PlayerID == 0) {
                    kgo.GetComponentInChildren<Renderer>().sharedMaterial = _materials["SweetBlue"];
                } else {
                    kgo.GetComponentInChildren<Renderer>().sharedMaterial = _materials["SweetRed"];
                }
            }
        }

        public void OnBuild(KeyCode k) {
            if (_building != null) Destroy(_building);
            switch (k) {
                case KeyCode.C:
                    _building = Instantiate(Resources.Load("Prefabs/Buildings/View/Base"),
                        InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
                case KeyCode.V:
                    _building = Instantiate(Resources.Load("Prefabs/Buildings/View/Barrack"),
                        InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
            }
            if (_building != null) {
                if (_krtransform.PlayerID == 0) {
                    _building.GetComponentInChildren<Renderer>().sharedMaterial = _materials["TransparentBlue"];
                } else {
                    _building.GetComponentInChildren<Renderer>().sharedMaterial = _materials["TransparentRed"];
                }
            }
        }

        void Update() {
            if (_building != null) {
                Vector3 mousePos = InputModelAdapter.WorldPosition(Input.mousePosition);
                if (_building.transform.position != mousePos) {
                    _building.transform.position = mousePos.Adjusted();
                }
            }
        }

        public bool CanBuild(Vec2 pos) {
            Vec2 size = Vec2.FromVector3(_building.transform.GetChild(0).localScale);
            for (int i = 0; i < size.X; ++i) {
                for (int j = 0; j < size.Y; ++j) {
                    if (!KRFacade.IsEmpty(pos + new Vec2(i, j))) {
                        return false;
                    }
                }
            }
            return true;
        }

        public void DisableBuildMode() {
            Destroy(_building);
        }
    }
}
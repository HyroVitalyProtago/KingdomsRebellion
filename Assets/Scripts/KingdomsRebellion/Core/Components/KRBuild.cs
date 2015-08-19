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
        GameObject _view;
        public KRTransform Building { get; private set; }
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
            _view = null;
            if (go == null) {
                throw new SystemException("KRGameObjects not found in scene");
            }
            _dynamics = go.transform;

            Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

            RallyPoint = _krtransform.Pos + 2*toCenter;
        }

        public void AddSpawnable(String nameObj) {
            _spawnableObjects.Add(nameObj, Resources.Load("Prefabs/Buildings/" + nameObj));
        }

        public void Build(String nameObj, Vec2 pos) {
            if (!KRFacade.IsEmpty(pos)) return;
            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo =
                    Instantiate(_spawnableObjects[nameObj], pos.ToVector3().Adjusted(), Quaternion.identity) as
                        GameObject;

                kgo.transform.SetParent(_dynamics);
                Building = kgo.GetComponent<KRTransform>();
                Building.PlayerID = _krtransform.PlayerID;
                kgo.GetComponent<KRHealth>().OnSpawn();

                if (_krtransform.PlayerID == 0) {
                    Building.GetComponentInChildren<Renderer>().sharedMaterial = _materials["SweetBlue"];
                } else {
                    Building.GetComponentInChildren<Renderer>().sharedMaterial = _materials["SweetRed"];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k">KeyCode press</param>
        /// <returns>True if player want to Build new Object. If player want to repare, return false.</returns>
        public bool OnBuild(KeyCode k) {
            if (_view != null) Destroy(_view);
            switch (k) {
                case KeyCode.C:
                    _view = Instantiate(Resources.Load("Prefabs/Buildings/View/Base"),
                        InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
                case KeyCode.V:
                    _view = Instantiate(Resources.Load("Prefabs/Buildings/View/Barrack"),
                        InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
                case KeyCode.R:
                    return false;
            }
            if (_view != null) {
                if (_krtransform.PlayerID == 0) {
                    _view.GetComponentInChildren<Renderer>().sharedMaterial = _materials["TransparentBlue"];
                } else {
                    _view.GetComponentInChildren<Renderer>().sharedMaterial = _materials["TransparentRed"];
                }
            }
            return true;
        }

        void Update() {
            if (_view != null) {
                Vector3 mousePos = InputModelAdapter.WorldPosition(Input.mousePosition);
                if (_view.transform.position != mousePos) {
                    _view.transform.position = mousePos.Adjusted();
                }
            }
        }

        public bool CanBuild(Vec2 pos) {
            Vec2 size = Vec2.FromVector3(_view.transform.GetChild(0).localScale);
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
            Destroy(_view);
        }

        public void Repare(Vec2 pos) {
            Building = KRFacade.Find(pos).GetComponent<KRTransform>();
        }
    }
}
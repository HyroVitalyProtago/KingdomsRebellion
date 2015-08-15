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
        Vec2 _buildPoint;
        public Vec2 RallyPoint { get; set; }
        Dictionary<String, Object> _spawnableObjects;

        Material _sweetBlue, _sweetRed;

        void Awake() {
            _krtransform = GetComponent<KRTransform>();
            _spawnableObjects = new Dictionary<String, Object>();
            AddSpawnable("Base");
            AddSpawnable("Barrack");
            _sweetBlue = (Material) Resources.Load("Materials/SweetBlue", typeof (Material));
            _sweetRed = (Material) Resources.Load("Materials/SweetRed", typeof (Material));
            On("OnBuild");
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
            _buildPoint = _krtransform.Pos + 2 * toCenter;
        }

        public void AddSpawnable(String name) {
            _spawnableObjects.Add(name, Resources.Load("Prefabs/Buildings/" + name));
        }

        public void Build(String nameObj) {
            if (!KRFacade.IsEmpty(_buildPoint)) return;

            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo =
                    Instantiate(_spawnableObjects[nameObj], _buildPoint.ToVector3().Adjusted(), Quaternion.identity) as
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

        void OnBuild(KeyCode k) {
            if (_building != null) Destroy(_building);
            switch (k) {
                case KeyCode.C:
                    _building = Instantiate(_spawnableObjects["Base"],
                    InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
                case KeyCode.V: 
                    if (_building != null) Destroy(_building);
                    _building = Instantiate(_spawnableObjects["Barrack"],
                    InputModelAdapter.WorldPosition(Input.mousePosition), Quaternion.identity) as GameObject;
                    break;
            }
            if (_building != null) {
                _building.layer = LayerMask.NameToLayer("Ignore Raycast");
                _building.transform.SetParent(_dynamics);
                _building.GetComponent<KRTransform>().PlayerID = _krtransform.PlayerID;
                if (_krtransform.PlayerID == 0) {
                    _building.GetComponentInChildren<Renderer>().sharedMaterial = _sweetBlue;
                } else {
                    _building.GetComponentInChildren<Renderer>().sharedMaterial = _sweetRed;
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

        void OnMouseUp() {
            Debug.Log("coucou");
        }
    }
}
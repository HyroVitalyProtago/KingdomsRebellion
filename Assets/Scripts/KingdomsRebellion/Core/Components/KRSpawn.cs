using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using Object = UnityEngine.Object;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
    public class KRSpawn : KRBehaviour {

		Transform _dynamics;
		KRTransform _krtransform;

        Vec2 _spawnPoint;
        Vec2 _rallyPoint;
        Dictionary<String, Object> _spawnableObjects;

		Material _sweetBlue, _sweetRed;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
            _spawnableObjects = new Dictionary<String, Object>();
			AddSpawnable("Infantry");
            AddSpawnable("Archer");
			_sweetBlue = (Material) Resources.Load("Materials/SweetBlue", typeof(Material));
			_sweetRed = (Material) Resources.Load("Materials/SweetRed", typeof(Material));
		}

        void Start() {
			GameObject go = GameObject.Find("KRGameObjects");
			if (go == null) { throw new SystemException("KRGameObjects not found in scene"); }
			_dynamics = go.transform;

			Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

			_rallyPoint = _krtransform.Pos + 5 * toCenter;
			_spawnPoint = _krtransform.Pos + 2 * toCenter;
        }

        public void AddSpawnable(String name) {
            _spawnableObjects.Add(name, Resources.Load("Prefabs/" + name));
        }

		public void Spawn(String nameObj) {
			if (!KRFacade.IsEmpty(_spawnPoint)) return;

            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo = Instantiate(_spawnableObjects[nameObj], _spawnPoint.ToVector3().Adjusted(), Quaternion.identity) as GameObject;

				kgo.transform.SetParent(_dynamics);

				kgo.GetComponent<KRTransform>().PlayerID = _krtransform.PlayerID;
				if (_krtransform.PlayerID == 0) {
					kgo.transform.Find("EthanBody").GetComponent<Renderer>().sharedMaterial = _sweetBlue;
					kgo.transform.Find("Spotlight").GetComponent<Light>().color = new Color(0f,.4f,1f);
				} else {
					kgo.transform.Find("EthanBody").GetComponent<Renderer>().sharedMaterial = _sweetRed;
					kgo.transform.Find("Spotlight").GetComponent<Light>().color = new Color(.85f,.85f,.3f);
				}

				kgo.GetComponent<FiniteStateMachine>().Move(_rallyPoint);
            }
        }
    }
}
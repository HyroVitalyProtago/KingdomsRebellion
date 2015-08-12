﻿using System;
using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Math;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(KRTransform))]
    public class KRSpawn : KRBehaviour {

		Transform _dynamics;
		KRTransform _krtransform;

        Vec2 _spawnPoint;
		public Vec2 RallyPoint { get; set; }
        Dictionary<String, Object> _spawnableObjects;

		Material _sweetBlue, _sweetRed;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
            _spawnableObjects = new Dictionary<String, Object>();
			AddSpawnable("Infantry");
            AddSpawnable("Archer");
            AddSpawnable("Worker");
			_sweetBlue = (Material) Resources.Load("Materials/SweetBlue", typeof(Material));
			_sweetRed = (Material) Resources.Load("Materials/SweetRed", typeof(Material));
		}

        void Start() {
			GameObject go = GameObject.Find("KRGameObjects");
			if (go == null) { throw new SystemException("KRGameObjects not found in scene"); }
			_dynamics = go.transform;

			Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

			RallyPoint = _krtransform.Pos + 5 * toCenter;
			_spawnPoint = _krtransform.Pos + 2 * toCenter;
        }

        public void AddSpawnable(String name) {
            _spawnableObjects.Add(name, Resources.Load("Prefabs/Units/" + name));
        }

		public void Spawn(String nameObj) {
			if (!KRFacade.IsEmpty(_spawnPoint)) return;

            if (_spawnableObjects.ContainsKey(nameObj)) {
                GameObject kgo = Instantiate(_spawnableObjects[nameObj], _spawnPoint.ToVector3().Adjusted(), Quaternion.identity) as GameObject;

				kgo.transform.SetParent(_dynamics);

				kgo.GetComponent<KRTransform>().PlayerID = _krtransform.PlayerID;
				if (_krtransform.PlayerID == 0) {
					kgo.transform.Find("Body").GetComponent<Renderer>().sharedMaterial = _sweetBlue;
					kgo.transform.Find("Spotlight").GetComponent<Light>().color = new Color(0f,.4f,1f);
				} else {
					kgo.transform.Find("Body").GetComponent<Renderer>().sharedMaterial = _sweetRed;
					kgo.transform.Find("Spotlight").GetComponent<Light>().color = new Color(.85f,.85f,.3f);
				}

				kgo.GetComponent<FiniteStateMachine>().Move(RallyPoint);
            }
        }
    }
}
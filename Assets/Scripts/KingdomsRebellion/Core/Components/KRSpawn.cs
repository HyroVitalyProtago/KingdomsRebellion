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

		KRTransform _krtransform;

        Vec2 _spawnPoint;
        Vec2 _rallyPoint;
        List<Object> _spawnableObjects;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
			_spawnableObjects = new List<Object>();
			AddSpawnable("Prefabs/Unit Red");
		}

        void Start() {
			_rallyPoint = _krtransform.Pos + 5 * Vec2.One;
			_spawnPoint = _krtransform.Pos + 2 * Vec2.One;
        }

        public void AddSpawnable(String path) {
            _spawnableObjects.Add(Resources.Load(path));
        }

        public void CreateGameObject(int numObj) {
            if (numObj < _spawnableObjects.Count) {
                GameObject kgo = Instantiate(_spawnableObjects[numObj], _spawnPoint.ToVector3(), Quaternion.identity) as GameObject;
				kgo.transform.SetParent(KRFacade.Dynamics);   
				kgo.GetComponent<FiniteStateMachine>().Move(_krtransform.PlayerID, new Vec3(_rallyPoint.X, 0, _rallyPoint.Y));
            }
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.C)) {
                CreateGameObject(0);
            }
        }
    }
}
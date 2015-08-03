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
        List<Object> _spawnableObjects;

		void Awake() {
			_krtransform = GetComponent<KRTransform>();
			_spawnableObjects = new List<Object>();
			AddSpawnable("Prefabs/Unit Red");
		}

        void Start() {
			GameObject go = GameObject.Find("KRGameObjects");
			if (go == null) { throw new SystemException("KRGameObjects not found in scene"); }
			_dynamics = go.transform;

			Vec2 toCenter = -(new Vec2(System.Math.Sign(_krtransform.Pos.X), System.Math.Sign(_krtransform.Pos.X)));

			_rallyPoint = _krtransform.Pos + 5 * toCenter;
			_spawnPoint = _krtransform.Pos + 2 * toCenter;
        }

        public void AddSpawnable(String path) {
            _spawnableObjects.Add(Resources.Load(path));
        }

        public void Spawn(int numObj) {
			if (!KRFacade.GetMap().IsEmpty(_spawnPoint)) return;

			Debug.Log("Spawn !");

            if (numObj < _spawnableObjects.Count) {
                GameObject kgo = Instantiate(_spawnableObjects[numObj], _spawnPoint.ToVector3().Adjusted(), Quaternion.identity) as GameObject;
				kgo.transform.SetParent(_dynamics);   
				kgo.GetComponent<FiniteStateMachine>().Move(_krtransform.PlayerID, _rallyPoint);
            }
        }
    }
}
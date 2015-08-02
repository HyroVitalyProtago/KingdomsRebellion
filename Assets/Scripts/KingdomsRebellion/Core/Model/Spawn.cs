using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.FSM;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using Object = UnityEngine.Object;

namespace KingdomsRebellion.Core.Model {
    public class Spawn : KRGameObject {

        int _radius;
        Vec2 _spawnPoint;
        Vec2 _rallyPoint;
        GameObject _mouse;
        public override Vec2 Pos { get; protected set; }

        public override int Radius {
            get { return _radius; }
        }

        List<Object> _spawnableObjects;

        protected override void Start() {
            base.Start();
            _mouse = GameObject.Find("Mouse");
            _spawnableObjects = new List<Object>();
            Pos = Vec2.FromVector3(transform.position);
            _radius = 3;
            _rallyPoint = Pos + 5* Vec2.One;
            _spawnPoint = Pos + 2* Vec2.One;
            KRFacade.GetMap().Add(this);
            AddSpawnable("Prefabs/UnitBlue");
            lifeMax = 100;
            life = 100;
            defense = 10;
            AttackType = AttackTypeEnum.Arrow;
            weakness = AttackTypeEnum.Sword;
        }

        public void AddSpawnable(String path) {
            _spawnableObjects.Add(Resources.Load(path));
        }

        public void CreateGameObject(int numObj) {
            if (numObj < _spawnableObjects.Count) {
                GameObject go = Instantiate(_spawnableObjects[numObj], _spawnPoint.ToVector3(), Quaternion.identity) as GameObject;
                go.transform.SetParent(_mouse.transform);   
                go.GetComponent<FiniteStateMachine>().Move(PlayerId, new Vec3(_rallyPoint.X, 0, _rallyPoint.Y));
            }
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.C)) {
                CreateGameObject(0);
            }
        }
    }
}
using System;
using KingdomsRebellion.Core.Components;
using KingdomsRebellion.Core.Math;
using UnityEngine;

namespace KingdomsRebellion.Core.FSM {
    public class BuildState : FSMState {

        private readonly KRTransform _krtransform;
        private readonly KRMovement _krMovement;
        private readonly KRBuild _krBuild;
        private KRHealth _krHealth;
        private KRTransform _buildingTransform;

        public BuildState(FiniteStateMachine fsm) : base(fsm) {
            _krtransform = fsm.GetComponent<KRTransform>();
            _krMovement = fsm.GetComponent<KRMovement>();
            _krBuild = fsm.GetComponent<KRBuild>();
        }

        public override void Enter() {
            _krHealth = _krBuild.Building.GetComponent<KRHealth>();
            _buildingTransform = _krBuild.Building.GetComponent<KRTransform>();
        }

        public override Type Execute() {
            if (Vec2.Dist(_buildingTransform.Pos, _krtransform.Pos) > 1) {
                _krMovement.Move(_buildingTransform.Pos);
                return typeof (MovementState);
            }
            if (!_krHealth.Ready) {
                // TODO replace by a component that all buildings have.
                _krHealth.Heal(1, _krBuild.Building.GetComponent<KRSpawn>() != null);
                return GetType();
            }
            return null;
        }
    }
}
﻿using UnityEngine;
using KingdomsRebellion.Core.Model;
using UnityStandardAssets.Characters.ThirdPerson;
using KingdomsRebellion.Core.Math;

namespace KingdomsRebellion.Core.Player {

	// TODO: How do we do when target isn't reachable ?
	// And with more than one unit ?
	// This script must control all the movement of units and create transform to set target if needed
	public class Movement : KRBehaviour {

		private Ray ray;
		private RaycastHit hit;
		private AICharacterControl aiCharacterControl;
		private Object targetPrefab;
		private Unit unit;

		void Start() {
			aiCharacterControl = gameObject.GetComponent<AICharacterControl>();
			targetPrefab = Resources.Load("Prefabs/Target");
			unit = GetComponent<Unit>();
		}
	
		void Update() {
			if (aiCharacterControl.target != null && (transform.position - aiCharacterControl.target.transform.position).magnitude < 1f) {
				var go = aiCharacterControl.target.gameObject;
				aiCharacterControl.SetTarget(null);
				if (go.CompareTag("Target")) {
					Destroy(go);
				}
			}
		}

		public void Move(int player, Vec3 targetModelPosition) {
			if (aiCharacterControl.target != null) {
				var go = aiCharacterControl.target.gameObject;
				aiCharacterControl.SetTarget(null);
				if (go.CompareTag("Target")) {
					Destroy(go);
				}
			}
			if (unit.playerId == player) {
				var go = KRFacade.GetGrid().GetGameObjectByPosition(new Vec2(targetModelPosition.X, targetModelPosition.Z));
				if (go == null) {
					Debug.Log("No game object found on (" + targetModelPosition.X + ", " + targetModelPosition.Z + ")");
				}

				var ennemy = (go == null) ? null : go.GetComponent<Unit>();
				if (go != null && ennemy != null && ennemy.playerId != player) {
					unit.attacking = true;
					unit.ennemyTargeted = ennemy;
					aiCharacterControl.SetTarget(ennemy.transform);
				} else {
					GameObject target = Instantiate(targetPrefab, targetModelPosition.ToVector3(), Quaternion.identity) as GameObject;
					aiCharacterControl.SetTarget(target.transform);
				}
			}
		}
	}
}
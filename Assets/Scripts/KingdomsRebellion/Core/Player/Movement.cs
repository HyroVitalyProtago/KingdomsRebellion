using UnityEngine;
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

		NavMeshPath path;

		void Start() {
			aiCharacterControl = gameObject.GetComponent<AICharacterControl>();
			targetPrefab = Resources.Load("Prefabs/Target");
			unit = GetComponent<Unit>();

			path = new NavMeshPath();
		}
	
		void Update() {
			if (aiCharacterControl.target != null && (transform.position - aiCharacterControl.target.transform.position).magnitude < 1f) {
				var go = aiCharacterControl.target.gameObject;
				aiCharacterControl.SetTarget(null);
				if (go.CompareTag("Target")) {
					Destroy(go);
				}
			}

			for (int i = 0; i < path.corners.Length-1; i++)
				Debug.DrawLine(path.corners[i], path.corners[i+1], Color.red);

			if (path.corners.Length > 0 && aiCharacterControl.target != null && (transform.position - path.corners[path.corners.Length-1]).magnitude < 1) {
				var go = aiCharacterControl.target.gameObject;
				aiCharacterControl.SetTarget(null);
				if (go.CompareTag("Target")) {
					Destroy(go);
				}
			}
		}

	    Vec3 _targetModelPosition;
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
					if (NavMesh.CalculatePath(gameObject.transform.position, targetModelPosition.ToVector3(), NavMesh.AllAreas, path) && (targetModelPosition.ToVector3() - path.corners[path.corners.Length-1]).magnitude < 1) {
						GameObject target = Instantiate(targetPrefab, targetModelPosition.ToVector3(), Quaternion.identity) as GameObject;
						aiCharacterControl.SetTarget(target.transform);
					    _targetModelPosition = targetModelPosition;
					} else {
						Debug.Log("I can't go here !!");
					}
				}
			}
		}

        public void UpdateGame() {
            Vec2 pos = KRFacade.GetGrid().GetPositionOf(gameObject);
            Debug.Log(pos);
            Debug.Log(_targetModelPosition);
            if (_targetModelPosition != null && (_targetModelPosition.X != pos.X || _targetModelPosition.Z != pos.Y) && path.corners.Length > 0) {
                int dx = _targetModelPosition.X - pos.X;
                int dy = _targetModelPosition.Z - pos.Y;
                dx = dx == 0 ? 0 : dx > 0 ? 1 : -1;
                dy = dy == 0 ? 0 : dy > 0 ? 1 : -1;
	            KRFacade.GetGrid().Move(gameObject, pos + new Vec2(dx, dy));
            }
            Debug.DrawLine(pos.ToVector3(), new Vector3(0, 0, 0));
	    }
	}
}
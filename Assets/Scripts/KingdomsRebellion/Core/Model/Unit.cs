using UnityEngine;
using System;
using System.Collections;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using KingdomsRebellion.Core.Interfaces;

namespace KingdomsRebellion.Core.Model {

//TODO: All attributes HAVE TO be private.
//Create events to modify attributes when needed.
	public class Unit : KRBehaviour, IPos, HaveRadius {

		public Color color;
		public int playerId;

		public string type { get; private set; }
		// public bool selected;
		public int life;
		public int lifeMax;
		public bool attacking;
		private bool attacklaunched;
		public int strength;
		public int defense;
		public Unit ennemyTargeted;
//	    public Vec2 Position;
		private GameObject spot;

		public Vec2 Pos { get { return GetComponent<Movement>().Pos; } }
		public int Radius { get { return 1; } }

		//Events : 
	    static event Action<GameObject> OnUnitDeath;

	    static Unit() {
            EventConductor.Offer(typeof(Unit), "OnUnitDeath");
	    }

        void Start() {
			//   selected = false;
			lifeMax = 30;
			life = 30;
			attacking = false;
			attacklaunched = false;
			strength = 14;
			defense = 10;
			spot = gameObject.GetComponentInChildren<Light>().gameObject;
			spot.GetComponent<Light>().color = color;
			spot.SetActive(false);
			if (color == Color.blue) {
				playerId = 0;
			} else {
				playerId = 1;
			}

		    KRFacade.GetMap().Add(this);
		}
	
		// Update is called once per frame
		void Update() {
			if (attacking && !attacklaunched) {
				StartCoroutine(Attack(ennemyTargeted));
				attacklaunched = true;
			} else if (!attacking && attacklaunched) {
				StopCoroutine(Attack(ennemyTargeted));
				attacklaunched = false;
			}

			if (life <= 0) {
				Destroy(gameObject);
			}
		}

		public IEnumerator Attack(Unit ennemy) {
			while (attacking && ennemy != null) {
				if ((ennemy.transform.position - transform.position).magnitude <= 1.1f) {
					spot.SetActive(true);
					ennemy.life -= strength - ennemy.defense;
				}
                //if (ennemy.life <= 0) {
                //    attacking = false;
                //}
				yield return new WaitForSeconds(0.2f);
				spot.SetActive(false);
				yield return new WaitForSeconds(1.8f);
			}
			yield return null;
		}

		void OnDestroy() {
            OnUnitDeath(gameObject);
		    KRFacade.GetMap().Remove(this);
		}
	}
}
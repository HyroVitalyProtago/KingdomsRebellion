using UnityEngine;
using System;
using System.Collections;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core.Model {

//TODO: All attributes HAVE TO be private.
//Create events to modify attributes when needed.
	public class Unit : KRBehaviour {

	    public Color color;
		public int PlayerId { get; private set; }

		public string type { get; private set; }
		// public bool selected;
		public int life;
		public int lifeMax;
		int defense;
	    AttackTypeEnum weakness;
	    public AttackTypeEnum AttackType { get; private set; }
//	    public Vec2 Position;

		//Events : 
	    static event Action<GameObject> OnUnitDeath;

	    static Unit() {
            EventConductor.Offer(typeof(Unit), "OnUnitDeath");
	    }

        void Start() {
			//   selected = false;
			lifeMax = 30;
			life = 30;
			defense = 10;
            if (color == Color.blue) {
				PlayerId = 0;
                weakness = AttackTypeEnum.Sword;
                AttackType = AttackTypeEnum.Arrow;
			} else {
				PlayerId = 1;
                weakness = AttackTypeEnum.Sword;
                AttackType = AttackTypeEnum.Sword;
			}
		    KRFacade.GetGrid().Add(gameObject, Vec2.FromVector3(transform.position));
		}
	
		void OnDestroy() {
		    GetComponent<Attack>().isDead = true;
            OnUnitDeath(gameObject);
		    KRFacade.GetGrid().Remove(gameObject);
            
		}

	    public void OnDamageDone(AttackTypeEnum type, int damage) {
	            if (weakness == type) {
	                life -= Mathf.FloorToInt(1.3f*(damage - defense));
	            } else {
	                life -= damage - defense;
	            }
	        if (life <= 0) {
	            Destroy(gameObject);
	        }
	    }
	}
}
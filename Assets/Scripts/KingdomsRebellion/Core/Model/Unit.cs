using System;
using KingdomsRebellion.Core.Interfaces;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core.Model {

//TODO: All attributes HAVE TO be private.
//Create events to modify attributes when needed.
	public class Unit : KRBehaviour, IPos, HaveRadius {

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

		    KRFacade.GetMap().Add(this);
		}
	
		void OnDestroy() {
		    GetComponent<Attack>().isDead = true;
            OnUnitDeath(gameObject);
		    KRFacade.GetMap().Remove(this);
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
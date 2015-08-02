using System;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Core.Player;
using UnityEngine;

namespace KingdomsRebellion.Core.Model {

//TODO: All attributes HAVE TO be private.
//Create events to modify attributes when needed.
	public class Unit : KRGameObject {


		public string type { get; private set; }
		// public bool selected;
//	    public Vec2 Position;

		public override Vec2 Pos { get { return GetComponent<Movement>().Pos; } }
		public override int Radius { get { return 1; } }

		//Events : 
	    static event Action<GameObject> OnUnitDeath;

	    static Unit() {
            EventConductor.Offer(typeof(Unit), "OnUnitDeath");
	    }

        protected override void Start() {
            base.Start();

		    KRFacade.GetMap().Add(this);
		}
	
		
	}
}
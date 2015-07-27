using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Core.AI;
using KingdomsRebellion.Inputs;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core {
	public class KRFacade : KRObject {

		static InputNetworkAdapter InputNetworkAdapter;
		static AbstractGrid Grid;

		static KRFacade() {
			InputNetworkAdapter = new InputNetworkAdapter();
			Grid = new FlatGrid(256, 256);
		}

		public static AbstractGrid GetGrid() {
			return Grid;
		}

		public static void UpdateGame() {
            var units = new List<Unit>(Grid.GetGameObjects().Keys);
		    foreach (var unit in units) {
                //unit.GetComponent<Movement>().UpdateGame();
                //unit.GetComponent<Attack>().UpdateGame();
                unit.GetComponent<FiniteStateMachine>().UpdateGame();
		    }
		}
	}
}
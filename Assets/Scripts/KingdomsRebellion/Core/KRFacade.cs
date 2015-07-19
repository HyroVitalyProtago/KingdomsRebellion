using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KingdomsRebellion.Inputs;
using KingdomsRebellion.Core.Grid;
using KingdomsRebellion.Core.Model;
using KingdomsRebellion.Core.Player;

namespace KingdomsRebellion.Core {
	public class KRFacade : KRBehaviour {

		static bool Instantiated = false;

		static InputNetworkAdapter InputNetworkAdapter;
		static AbstractGrid Grid;

		void Start() {
			Debug.Assert(!Instantiated);
			InputNetworkAdapter = new InputNetworkAdapter();
			Grid = new FlatGrid(256, 256);
			Instantiated = true;
		}

		public static AbstractGrid GetGrid() {
			return Grid;
		}

		public static void UpdateGame() {
            var units = new List<Unit>(Grid.GetGameObjects().Keys);
		    foreach (var unit in units) {
		        unit.GetComponent<Movement>().UpdateGame();
		    }
		}

#if UNITY_EDITOR
		void Update() {
			for (int i = -25 ; i < 25 ; ++i) {
				Debug.DrawLine(
					transform.position + Vector3.left * 25 + i * Vector3.forward,
					transform.position - Vector3.left * 25 + i * Vector3.forward
				);

				Debug.DrawLine(
					transform.position + Vector3.forward * 25 + i * Vector3.right,
					transform.position - Vector3.forward * 25 + i * Vector3.right
				);
			}
		}
#endif

	}
}
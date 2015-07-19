using UnityEngine;
using System.Collections;
using KingdomsRebellion.Inputs;
using KingdomsRebellion.Core.Grid;

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

		}
	}
}
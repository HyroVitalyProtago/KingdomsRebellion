using UnityEngine;
using KingdomsRebellion.UI;

namespace KingdomsRebellion.Core.Components {

	[RequireComponent(typeof(HealthBar))]
	public class KRSelection : KRBehaviour {
		
		public bool IsSelected { get; private set; }
		
		HealthBar _healthBar;

		void Awake() {
			_healthBar = GetComponent<HealthBar>();
		}

		void Start() {
			IsSelected = false;
		}

		void OnMouseEnter() {
			Show();
		}

		void OnMouseExit() {
			Hide();
		}

		public void Select() {
			Show();
			IsSelected = true;
		}

		public void Deselect() {
			IsSelected = false;
			Hide();
		}

		void Show() {
			if (IsSelected) return;
			_healthBar.Show();
		}

		void Hide() {
			if (IsSelected) return;
			_healthBar.Hide();
		}
	}
}

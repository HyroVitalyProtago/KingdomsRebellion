using UnityEngine;
using System.Collections;
using KingdomsRebellion.Core.Player;
using UnityEngine.UI;

namespace KingdomsRebellion.Core.Player {
    public class UIObj : MonoBehaviour {

        private HealthBar _healthBar;
        private GameObject _healthContainer;

        private void Start() {
            _healthBar = GetComponent<HealthBar>();
            _healthContainer = _healthBar.GetComponentInChildren<Image>().gameObject;
            _healthContainer.SetActive(false);
            _healthBar.enabled = false;
        }

        private void OnMouseEnter() {
            if (!_healthBar.IsSelected) {
                _healthBar.enabled = true;
                _healthContainer.SetActive(true);
            }
        }

        private void OnMouseExit() {
            Debug.Log(!_healthBar.IsSelected);
            if (!_healthBar.IsSelected) {
                _healthContainer.SetActive(false);
                _healthBar.enabled = false;
            }
        }
    }
}

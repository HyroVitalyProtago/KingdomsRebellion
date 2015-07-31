using UnityEngine;
using System.Collections;
using KingdomsRebellion.Core.Player;
using UnityEngine.UI;

public class UIObj : MonoBehaviour {

    HealthBar _healthBar;
    GameObject _healthContainer; 

    void Start() {
        _healthBar = GetComponent<HealthBar>();
        _healthContainer = _healthBar.GetComponentInChildren<Image>().gameObject;
        _healthContainer.SetActive(false);
        _healthBar.enabled = false;
    }

    void OnMouseEnter() {
        if (!_healthBar.IsSelected) {
            _healthBar.enabled = true;
            _healthContainer.SetActive(true);
        }
    }

    void OnMouseExit() {
        Debug.Log(!_healthBar.IsSelected);
        if (!_healthBar.IsSelected) {
            _healthContainer.SetActive(false);
            _healthBar.enabled = false;
        }
    }
}

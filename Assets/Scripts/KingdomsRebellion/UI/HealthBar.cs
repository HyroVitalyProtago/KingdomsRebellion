using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Components;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomsRebellion.UI {

	/// <summary>
	/// Display a HealthBar on GameObject which have a Canvas containing an Image which contains another name HealthBar. 
	/// </summary>
	public class HealthBar : KRBehaviour {

		GameObject _healthContainer;
		RectTransform _healthBar;
        KRHealth _krHealth;
		float _initWidth;
		float _initCameraSize;
		RectTransform _rectTransform;
		Image _image;

        void Awake() {
        	On("OnMainCameraChange");

        	_healthContainer = GetComponentInChildren<Image>().gameObject;
			_healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
			_rectTransform = _healthContainer.GetComponent<RectTransform>();
			_healthBar = _healthContainer.transform.FindChild("HealthBar").GetComponent<RectTransform>();
			_image = _healthBar.GetComponent<Image>();
            _krHealth = transform.GetComponentInParent<KRHealth>();
        }

		void Start() {
		    _initCameraSize = 4f;
			_initWidth = _healthBar.rect.width;
			_image.color = Color.green;

			Hide();
		}

		void Update() {
			_healthContainer.transform.position = transform.position + 2 * Vector3.up;
			if (Camera.main.orthographicSize / _initCameraSize > 1.5f) {
				_rectTransform.localScale = Vector3.one;
			} else {
				_rectTransform.localScale = 2 * (Vector3.one + (1 - Camera.main.orthographicSize / _initCameraSize) * Vector3.one);
			}

			// TODO update color or size only when something change with listener/event system (OnHealthChange)
			OnHealthChange();
		}

		void OnHealthChange() {
            float lifePercent = (float)_krHealth.Life / (float)_krHealth.LifeMax;
			_healthBar.sizeDelta = new Vector2(_initWidth * lifePercent, _healthBar.rect.height);
			if (lifePercent <= 0.66f && lifePercent > 0.33f) {
				_image.color = Color.yellow;
			} else if (lifePercent <= 0.33f) {
				_image.color = Color.red;
			} else {
				_image.color = Color.green;
			}
		}

		void OnMainCameraChange() {
			_healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
		}

		public void Show() {
			enabled = true;
			Update(); // for better display
			_healthContainer.SetActive(true);
		}

		public void Hide() {
			enabled = false;
			_healthContainer.SetActive(false);
		}
	}
}
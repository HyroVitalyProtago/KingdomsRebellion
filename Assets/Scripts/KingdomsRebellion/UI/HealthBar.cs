using KingdomsRebellion.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace KingdomsRebellion.Core.Player {

	//TODO Call SetActive only one time by creating an event.
	//
	// Display a HealthBar on GameObject which have a Canvas containing an Image which contains another name HealthBar. 
	//
	public class HealthBar : KRBehaviour {

		private GameObject healthContainer;
		private RectTransform healthBar;
		private Unit unitData;
		private float initWidth;
		private float initCameraSize;
		private RectTransform rectTransform;
		private Image image;
        public bool IsSelected { get; private set; }
		void OnMainCameraChange() {
			healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
		}

		void Start() {
			On("OnMainCameraChange");

			healthContainer = GetComponentInChildren<Image>().gameObject;
			healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
			rectTransform = healthContainer.GetComponent<RectTransform>();
			healthBar = healthContainer.transform.FindChild("HealthBar").GetComponent<RectTransform>();
			image = healthBar.GetComponent<Image>();
			unitData = transform.GetComponentInParent<Unit>();
		    initCameraSize = 4f;
			initWidth = healthBar.rect.width;
			image.color = Color.green;
		}

		void Update() {
			healthContainer.transform.position = transform.position + 2 * Vector3.up;
			float lifePercent = (float)unitData.life / (float)unitData.lifeMax;
            var scale = rectTransform.localScale = Camera.main.orthographicSize / initCameraSize > 1.5f ? Vector3.one : 2 * (Vector3.one + (1 - Camera.main.orthographicSize / initCameraSize) * Vector3.one);
			healthBar.sizeDelta = new Vector2(initWidth * lifePercent, healthBar.rect.height);
			if (lifePercent <= 0.66f && lifePercent > 0.33f) {
				image.color = Color.yellow;
			} else if (lifePercent <= 0.33f) {
				image.color = Color.red;
			} else {
				image.color = Color.green;
			}
		}

		public void HideHealthBar() {
			enabled = false;
		    IsSelected = false;
			healthContainer.SetActive(false);
		}

		public void ShowHealthBar() {
			Update();
			enabled = true;
		    IsSelected = true;
			healthContainer.SetActive(true);
		}
	}
}
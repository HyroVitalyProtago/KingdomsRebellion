using UnityEngine;
using UnityEngine.UI;
using KingdomsRebellion.Core;

namespace KingdomsRebellion.UI {

	/// <summary>
	/// Display a HealthBar on GameObject which have a Canvas containing an Image which contains another name HealthBar. 
	/// </summary>
	public class HealthBar : KRBehaviour {

		GameObject healthContainer;
		RectTransform healthBar;
        KRGameObject unitData;
		float initWidth;
		float initCameraSize;
		RectTransform rectTransform;
		Image image;

        void Awake() {
        	On("OnMainCameraChange");

        	healthContainer = GetComponentInChildren<Image>().gameObject;
			healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
			rectTransform = healthContainer.GetComponent<RectTransform>();
			healthBar = healthContainer.transform.FindChild("HealthBar").GetComponent<RectTransform>();
			image = healthBar.GetComponent<Image>();
            unitData = transform.GetComponentInParent<KRGameObject>();
        }

		void Start() {
		    initCameraSize = 4f;
			initWidth = healthBar.rect.width;
			image.color = Color.green;

			Hide();
		}

		void Update() {
			healthContainer.transform.position = transform.position + 2 * Vector3.up;
			if (Camera.main.orthographicSize / initCameraSize > 1.5f) {
				rectTransform.localScale = Vector3.one;
			} else {
				rectTransform.localScale = 2 * (Vector3.one + (1 - Camera.main.orthographicSize / initCameraSize) * Vector3.one);
			}

			// TODO update color or size only when something change with listener/event system (OnHealthChange)
			OnHealthChange();
		}

		void OnHealthChange() {
			float lifePercent = (float)unitData.life / (float)unitData.lifeMax;
			healthBar.sizeDelta = new Vector2(initWidth * lifePercent, healthBar.rect.height);
			if (lifePercent <= 0.66f && lifePercent > 0.33f) {
				image.color = Color.yellow;
			} else if (lifePercent <= 0.33f) {
				image.color = Color.red;
			} else {
				image.color = Color.green;
			}
		}

		void OnMainCameraChange() {
			healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
		}

		public void Show() {
			enabled = true;
			Update(); // for better display
			healthContainer.SetActive(true);
		}

		public void Hide() {
			enabled = false;
			healthContainer.SetActive(false);
		}
	}
}
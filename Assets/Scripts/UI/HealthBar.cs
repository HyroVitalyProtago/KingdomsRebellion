using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//TODO Call SetActive only one time by creating an event.
/*
 *Display a HealthBar on GameObject which have a Canvas containing an Image which contains another name HealthBar. 
 */
public class HealthBar : MonoBehaviour {

    private GameObject healthContainer;
    private RectTransform healthBar;
    private Unit unitData;
    private float initWidth;
    private float initCameraSize;
    private RectTransform rectTransform;
    private Image image;

	void Start () {
        healthContainer = this.GetComponentInChildren<Image>().gameObject;
        healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        rectTransform = healthContainer.GetComponent<RectTransform>();
        healthBar = healthContainer.transform.FindChild("HealthBar").GetComponent<RectTransform>();
        image = healthBar.GetComponent<Image>();
        unitData = transform.GetComponentInParent<Unit>();
        initCameraSize = Camera.main.orthographicSize; // Set a value by default to have the same for all units without depending on initial zoom.
        initWidth = healthBar.rect.width;
        image.color = Color.green;
        healthContainer.SetActive(false);
        enabled = false;
	}
	

	void Update () {
        healthContainer.transform.position = transform.position + 2 * Vector3.up;
        float lifePercent = (float)unitData.life / (float)unitData.lifeMax;
        var scale = rectTransform.localScale = Vector3.one + (1 - Camera.main.orthographicSize / initCameraSize) * Vector3.one;
        if (scale.x < 0.3f) {
            scale = 0.3f * Vector3.one;
        }
        healthBar.sizeDelta = new Vector2(initWidth * lifePercent, healthBar.rect.height);
        if(lifePercent <= 0.66f && lifePercent > 0.33f) {
            image.color = Color.yellow;
        } else if(lifePercent <=  0.33f) {
            image.color = Color.red;
        } else {
            image.color = Color.green;
        }
     }

    public void HideHealthBar() {
        healthContainer.SetActive(false);
        enabled = false;
    }

    public void ShowHealthBar() {
        Update();
        healthContainer.SetActive(true);
        enabled = true;
    }
}

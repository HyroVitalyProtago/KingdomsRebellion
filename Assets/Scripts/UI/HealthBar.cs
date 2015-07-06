using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private GameObject healthContainer;
    private RectTransform healthBar;
    private Unit unitData;
    private float initWidth;
    private float initCameraSize;
	// Use this for initialization
	void Start () {
        healthContainer = this.GetComponentInChildren<Image>().gameObject;
        healthContainer.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        healthBar = healthContainer.transform.FindChild("HealthBar").GetComponent<RectTransform>();
        unitData = transform.GetComponentInParent<Unit>();
        initCameraSize = Camera.main.orthographicSize;
        initWidth = healthBar.rect.width;
        healthBar.GetComponent<Image>().color = Color.green;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.GetComponentInParent<Unit>().selected) {
            DisplayData();
        } else {
            healthContainer.SetActive(false);
        }
	}

    void DisplayData() {
        healthContainer.transform.position = transform.position + 2 * Vector3.up;
        float lifePercent = (float)unitData.life / (float)unitData.lifeMax;
        var scale = healthContainer.GetComponent<RectTransform>().localScale = Vector3.one + (1 - Camera.main.orthographicSize / initCameraSize) * Vector3.one;
        if (scale.x < 0.3f) {
            scale = 0.3f * Vector3.one;
        }
        healthBar.sizeDelta = new Vector2(initWidth * lifePercent, healthBar.rect.height);
        if(lifePercent <= 0.66f && lifePercent > 0.33f) {
            healthBar.GetComponent<Image>().color = Color.yellow;
        } else if(lifePercent <=  0.33f) {
            healthBar.GetComponent<Image>().color = Color.red;
        }
        healthContainer.SetActive(true);
     }
}

using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public Color color;
    public string type { get; private set; }
    public bool selected;
    public int life;
    public int lifeMax;
    public bool attacking;
    private bool attacklaunched;
    public int strength;
    public int defense;
    public Unit ennemyTargeted;
    private GameObject light;
	// Use this for initialization
	void Start () {
        selected = false;
        lifeMax = 30;
        life = 30;
        attacking = false;
        attacklaunched = false;
        strength = 14;
        defense = 10;
        light = gameObject.GetComponentInChildren<Light>().gameObject;
        light.GetComponent<Light>().color = color;
        light.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (attacking && !attacklaunched) {
            StartCoroutine(Attack(this, ennemyTargeted));
            attacklaunched = true;
        } else if(!attacking && attacklaunched){
            StopCoroutine(Attack(this, ennemyTargeted));
            attacklaunched = false;
        }

        if (life <= 0) {
            GameObject.Destroy(gameObject);
        }
	}

    public IEnumerator Attack(Unit selectedUnit, Unit ennemy) {
        if (selectedUnit.color == ennemy.color) {
            attacking = false;
            yield return null;
        }
        while (attacking && ennemy != null) {
            if ((ennemy.transform.position - selectedUnit.transform.position).magnitude <= 1.1f) {
                light.SetActive(true);
                ennemy.life -= selectedUnit.strength - ennemy.defense;
            }
            if (ennemy.life <= 0) {
                attacking = false;
            }
            yield return new WaitForSeconds(0.2f);
            light.SetActive(false);
            yield return new WaitForSeconds(1.8f);
        }
        yield return null;
    }
    void OnDestroy() {
        if (selected) {
            MouseSelection.selectedObjects.Remove(this.gameObject);
        }
    }
}

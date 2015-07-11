using UnityEngine;
using System.Collections;

//TODO: All attributes HAVE TO be private.
//Create events to modify attributes when needed.
public class Unit : MonoBehaviour {

    public Color color;
    public int playerId;
    public string type { get; private set; }
    public bool selected;
    public int life;
    public int lifeMax;
    public bool attacking;
    private bool attacklaunched;
    public int strength;
    public int defense;
    public Unit ennemyTargeted;
    private GameObject spot;

    //Events : 
    public delegate void EOnDeath(GameObject go);

    public static event EOnDeath OnDeath;

	// Use this for initialization
	void Start () {
        selected = false;
        lifeMax = 30;
        life = 30;
        attacking = false;
        attacklaunched = false;
        strength = 14;
        defense = 10;
        spot = gameObject.GetComponentInChildren<Light>().gameObject;
        spot.GetComponent<Light>().color = color;
        spot.SetActive(false);
        if (color == Color.blue) {
            playerId = 0;
        } else {
            playerId = 1;
        }
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
                spot.SetActive(true);
                ennemy.life -= selectedUnit.strength - ennemy.defense;
            }
            if (ennemy.life <= 0) {
                attacking = false;
            }
            yield return new WaitForSeconds(0.2f);
            spot.SetActive(false);
            yield return new WaitForSeconds(1.8f);
        }
        yield return null;
    }
    void OnDestroy() {
        if (selected) {
            //selectedObjects.Remove(this.gameObject);
            OnDeath(gameObject);
        }
    }

    public void ApplySelection() {
        selected = true;
    }

    public void ApplyDeselection() {
        GetComponent<HealthBar>().HideHealthBar();
        selected = false;
    }
}

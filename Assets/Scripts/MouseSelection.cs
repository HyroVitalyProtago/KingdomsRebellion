using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class MouseSelection : MonoBehaviour {

    private Ray ray;
    private RaycastHit hit;
    public static IList<GameObject> selectedObjects;
    private GameObject target;
    private Vector3 hitpoint;
    private Vector3 mousePosition;
    private bool isDragging;
    private float timeLeftBeforeDragging;
    public Color playerColor;

    void Start() {
        selectedObjects = new List<GameObject>();
        target = GameObject.Find("Transform");
        timeLeftBeforeDragging = .15f;
        isDragging = false;
    }

    // Update is called once per frame
    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
            if (Input.GetMouseButtonDown(0)) {
                hitpoint = hit.point;
                mousePosition = Input.mousePosition; 
                if (!Input.GetKey(KeyCode.LeftControl)) {
                    DeselectUnits();
                }
                if (hit.collider.CompareTag("Unit")) {
                    
                    selectedObjects.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<Unit>().selected = true;
                }

            } else if (Input.GetMouseButton(0)) {
                if (!isDragging) {
                    timeLeftBeforeDragging -= Time.deltaTime;
                }
                if (timeLeftBeforeDragging <= 0f) {
                    isDragging = true;
                }
            } else if (Input.GetMouseButtonUp(0)) {
                if (isDragging) {
                    SelectUnits();
                }
                isDragging = false;
                timeLeftBeforeDragging = .15f;
            }
            
            
            if (Input.GetMouseButtonUp(1)) {
                target.transform.position = hit.point; 
                MoveUnits(target.transform);
                if (hit.collider.CompareTag("Unit")) {
                    var unit = hit.collider.GetComponent<Unit>();
                    if ( unit.color != playerColor) {
                        foreach (var ally in selectedObjects) {
                            var allyUnit = ally.GetComponent<Unit>();
                            allyUnit.ennemyTargeted = unit;
                            allyUnit.attacking = true;
                        }
                    }
                }
            }
        }
    }


    void MoveUnits(Transform newtrans) {
        foreach (var unit in selectedObjects) {
            if (unit.GetComponent<Unit>().color == playerColor) {
                AICharacterControl aichar = unit.GetComponent<AICharacterControl>();
                Debug.Log("move");
                aichar.SetTarget(newtrans);
            }
        }
    }

    void OnGUI() {
        if(isDragging) {
            Vector2 pos = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            Vector2 size = new Vector2(Input.mousePosition.x - mousePosition.x, mousePosition.y - Input.mousePosition.y);
            Rect rect = new Rect(pos, size);
            BoxTools.DrawRect(rect, new Color(0.8f,0.8f,0.8f,0.25f));
            BoxTools.DrawRectBorder(rect, 1f, Color.blue);
        }
    }

    void SelectUnits() {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (var unit in units) {
            if (unit.GetComponent<Unit>().color == playerColor && IsInRect(unit)) {
                selectedObjects.Add(unit);
                unit.GetComponent<Unit>().selected = true;
            }
        }
    }

    void DeselectUnits() {
        foreach (var unit in selectedObjects) {
                unit.GetComponent<Unit>().selected = false;
        }
        selectedObjects.Clear();
        Debug.Log("clear");
    }

    bool IsInRect(GameObject gameObject) {
        if (!isDragging)
            return false;

        var camera = Camera.main;
        var viewportBounds = BoxTools.GetViewportBounds(camera, mousePosition, Input.mousePosition);

        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    
}

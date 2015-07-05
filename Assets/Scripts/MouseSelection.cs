using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class MouseSelection : MonoBehaviour {

    private Ray ray;
    private RaycastHit hit;
    public IList<GameObject> selectedObjects;
    private GameObject target;
    private Vector3 hitpoint;
    private Vector3 mousePosition;
    private bool isDragging;
    private float timeLeftBeforeDragging;
    public Color playerColor;

    void Start() {
        selectedObjects = new List<GameObject>();
        target = GameObject.Find("Transform");
        timeLeftBeforeDragging = .2f;
        isDragging = false;
    }

    // Update is called once per frame
    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
            if (hit.collider.CompareTag("Unit") && hit.collider.gameObject.GetComponent<Unit>().color == playerColor) {
                if (Input.GetMouseButtonDown(0)) {
                    if (!Input.GetKey(KeyCode.LeftControl)) {
                        selectedObjects.Clear();
                    }
                    selectedObjects.Add(hit.collider.gameObject);
                }
            }
            if (Input.GetMouseButtonDown(0)) {
                Physics.Raycast(ray.origin, ray.direction, out hit);
                hitpoint = hit.point;
                mousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(0)) {
                if (!isDragging) {
                    timeLeftBeforeDragging -= Time.deltaTime;
                }
                if (timeLeftBeforeDragging <= 0f) {
                    isDragging = true;
                    Debug.Log("Dragging");
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
                }
                    if (isDragging) {
                        SelectUnits();
                    }
                isDragging = false;
                timeLeftBeforeDragging = .2f;
            }
            if (Input.GetMouseButtonUp(1)) {
                target.transform.position = hit.point; 
                MoveUnits(target.transform);
            }
        }
    }


    void MoveUnits(Transform newtrans) {
        foreach (var unit in selectedObjects) {
            AICharacterControl aichar = unit.GetComponent<AICharacterControl>();
            aichar.SetTarget(newtrans);
        }
    }

    void OnGUI() {
        if(isDragging) {
            Vector2 pos = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            Vector2 size = new Vector2(Input.mousePosition.x - mousePosition.x, mousePosition.y - Input.mousePosition.y);
            GUI.Box(new Rect(pos, size), GUIContent.none);
        }
    }

    void SelectUnits() {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (var unit in units) {

            if (unit.GetComponent<Unit>().color == playerColor && IsInRect(unit.transform.position)) {
                selectedObjects.Add(unit);
            }
        }
    }

    bool IsInRect(Vector3 pos) {
        float xMin = (hitpoint.x < hit.point.x) ? hitpoint.x : hit.point.x;
        float zMin = (hitpoint.z < hit.point.z) ? hitpoint.z : hit.point.z;
        float xMax = (hitpoint.x < hit.point.x) ? hit.point.x : hitpoint.x;
        float zMax = (hitpoint.z < hit.point.z) ? hit.point.z : hitpoint.z;
        if (pos.x < xMin || pos.x > xMax || pos.z < zMin || pos.z > zMax) {
            return false;
        }
        return true;
    }
}

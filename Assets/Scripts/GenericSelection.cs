using UnityEngine;
using System.Collections.Generic;

//TODO: Make this class more generic.
//This class Select all selectable unit
public class GenericSelection : MonoBehaviour {

    Ray ray;
    private RaycastHit hit;
    protected IList<GameObject> selectedObjects;
    //Don't forget to notify (SendMessage) when new Selectable object is created.
    protected IList<GameObject> selectableObjects;
    protected Vector3 originWorldMousePoint;
    Camera originCamera;
    protected bool isDragging;
    protected float timeLeftBeforeDragging;

    protected virtual void OnEnable() {
        Mouse.OnUpdateDrag += OnUpdateDrag;
        Mouse.OnLeftClick += OnLeftClick;
        Mouse.OnDrag += OnDrag;
    }

    protected virtual void OnDisable() {
        Mouse.OnUpdateDrag -= OnUpdateDrag;
        Mouse.OnLeftClick -= OnLeftClick;
        Mouse.OnDrag -= OnDrag;
    }

    protected virtual void Start() {
        selectedObjects = new List<GameObject>();
        selectableObjects = new List<GameObject>();
        foreach (Transform child in transform) {
            selectableObjects.Add(child.gameObject);
        }
        timeLeftBeforeDragging = .15f;
        isDragging = false;
    }

    public void OnGUI() {
        if(isDragging) {
            Vector2 pos = new Vector2(Camera.main.WorldToScreenPoint(originWorldMousePoint).x, Screen.height - Camera.main.WorldToScreenPoint(originWorldMousePoint).y);
            Vector2 size = new Vector2(Input.mousePosition.x - Camera.main.WorldToScreenPoint(originWorldMousePoint).x, Camera.main.WorldToScreenPoint(originWorldMousePoint).y - Input.mousePosition.y);
            Rect rect = new Rect(pos, size);
            BoxTools.DrawRect(rect, new Color(0.8f,0.8f,0.8f,0.25f));
            BoxTools.DrawRectBorder(rect, 1f, Color.blue);
        }
    }

    protected virtual void SelectUnits(Vector3 originWorldPoint) {
        foreach (var unit in selectableObjects) {
            if (IsInRect(unit, originWorldPoint)) {
                selectedObjects.Add(unit);
            }
        }
        ApplySelection();
    }

    void DeselectUnits() {
        ApplyDeselection();
        selectedObjects.Clear();
    }

    protected bool IsInRect(GameObject gameObject, Vector3 originWorldPoint) {
        if (!isDragging) {
            return false;
        }
        var originViewportPoint = originCamera.WorldToViewportPoint(originWorldPoint);
        var camera = Camera.main;
        var viewportBounds = BoxTools.GetViewportBounds(originViewportPoint, camera, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    protected virtual void ApplySelection() {}

    protected virtual void ApplyDeselection() {}

    void OnLeftClick(int playerId, Camera camera, Vector3 mousePosition) {
        ray = camera.ScreenPointToRay(mousePosition);
        if (playerId == Mouse.PlayerId) {
            this.originWorldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
            originCamera = camera;
            if (!Input.GetKey(KeyCode.LeftControl)) {
                DeselectUnits();
            }
        }
        if (hit.collider != null) {
            var colliderGameObject = hit.collider.gameObject;
            //TODO Resolve problem when holding LeftControl to forbid to select unit of different color.
            if (selectableObjects.Contains(colliderGameObject)) {
                if (!selectedObjects.Contains(colliderGameObject)) {
                    selectedObjects.Add(colliderGameObject);
                } else {
                    ApplyDeselection();
                    selectedObjects.Remove(colliderGameObject);
                }
            }
        }
    }

    protected virtual void OnUpdateDrag(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
        if (!isDragging) {
            timeLeftBeforeDragging -= Time.deltaTime;
        }
        if (timeLeftBeforeDragging <= 0f) {
            isDragging = true;
        }
    }

    void OnDrag(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition) {
        SelectUnits(originWorldPoint);
        isDragging = false;
        timeLeftBeforeDragging = .15f;
    }
}

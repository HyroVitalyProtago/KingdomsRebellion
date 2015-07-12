﻿using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

    public delegate void EClickAction(int playerId, Camera camera, Vector3 mousePosition);
    public delegate void EDragAction(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition);

    public static event EClickAction OnLeftClick;
    public static event EClickAction OnRightClick;
    public static event EDragAction OnUpdateDrag;
    public static event EDragAction OnDrag;

    public static int PlayerId = 0;
    Vector3 originWorldPoint;

	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            originWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            OnLeftClick(Mouse.PlayerId, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButton(0)) {
            OnUpdateDrag(Mouse.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
            OnDrag(Mouse.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButtonDown(1)) {
            OnRightClick(Mouse.PlayerId, Camera.main, Input.mousePosition);
        }
	}

}

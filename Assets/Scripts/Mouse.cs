using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

    public delegate void EClickAction(int playerId, Camera camera, Vector3 mousePosition);
    public delegate void EDragAction(int playerId, Vector3 originWorldPoint, Camera currentCamera, Vector3 currentMousePousition);

    public static event EClickAction OnLeftClick;
    public static event EClickAction OnRightClick;
    public static event EDragAction OnUpdateDrag;
    public static event EDragAction OnDrag;

    Vector3 originWorldPoint;

	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            originWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (OnLeftClick != null) OnLeftClick(NetworkAPI.PlayerId, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButton(0)) {
//			if (OnUpdateDrag != null) OnUpdateDrag(NetworkAPI.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
//			if (OnDrag != null) OnDrag(NetworkAPI.PlayerId, originWorldPoint, Camera.main, Input.mousePosition);
        } else if (Input.GetMouseButtonDown(1)) {
			if (OnRightClick != null) OnRightClick(NetworkAPI.PlayerId, Camera.main, Input.mousePosition);
        }
	}

}

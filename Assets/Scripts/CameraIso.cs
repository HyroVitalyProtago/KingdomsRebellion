using UnityEngine;
using System.Collections;

/*
 * Navigation system for RTS
 * @advice Attach on camera
 */
public class CameraIso : MonoBehaviour {

	public static float ScrollWidth { get { return Screen.width * .1f; } }
	public static float ScrollHeight { get { return Screen.height * .2f; } }
	public static float ScrollFactor { get { return .4f; } }
	public static float ScrollSpeed { get { return 2f; } }
	public static float ScrollMaxSpeed { get { return 10f; } }
	public static float RotateAmount { get { return 10f; } }
	public static float RotateSpeed { get { return 100f; } }
	public static float MinCameraZoom { get { return 3f; } }
	public static float MaxCameraZoom { get { return 20f; } }

	void Update() {
		Move();
		Scroll();
		Rotate();
	}

	float Speed(float pos, float scrollSize, float screenSize, KeyCode key1, KeyCode key2) {
		float speed = 0;

		if (pos < scrollSize && pos >= 0) {
			speed = (pos / scrollSize) - 1;
		} else if (pos > screenSize - scrollSize && pos <= screenSize) {
			speed = 1 + ((pos - screenSize) / scrollSize);
		} else if (Input.GetKey(key1)) {
			speed = -ScrollSpeed;
		} else if (Input.GetKey(key2)) {
			speed = ScrollSpeed;
		}

		if (speed != 0) {
			speed = ScrollSpeed * Mathf.Sign(speed) * ((speed * speed) * ScrollFactor);
		}

		return speed;
	}

	void Move() {
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3(0,0,0);

		// horizontal camera movement
		movement.x += Speed(xpos, ScrollWidth, Screen.width, KeyCode.A, KeyCode.D);

		// vertical camera movement
		movement.z += Speed(ypos, ScrollHeight, Screen.height, KeyCode.S, KeyCode.W);

		// make sure movement is in the direction the camera is pointing
		// but ignore the vertical tilt of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection(movement);
		movement.y = 0;

		// calculate desired camera position based on received input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin + movement;

		if (destination != origin) {
		    Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ScrollMaxSpeed);
		}
	}

	void Scroll() {
		float scroll = ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0) {
			Camera.main.orthographicSize = Mathf.Clamp(
				Camera.main.orthographicSize + scroll,
				MinCameraZoom,
				MaxCameraZoom
			);
		}
	}

	// @todo move camera around, not just change eulerAngles...
	void Rotate() {
		Vector3 origin = Camera.main.transform.eulerAngles;
		Vector3 destination = origin;

		// detect rotation amount if Command is being held and the Right mouse button is down
		if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetMouseButton(1)) {
			destination.x -= Input.GetAxis("Mouse Y") * RotateAmount;
			destination.y += Input.GetAxis("Mouse X") * RotateAmount;
		}

		// if a change in position is detected perform the necessary update
		if (destination != origin) {
			Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * RotateSpeed);
		}
	}
}

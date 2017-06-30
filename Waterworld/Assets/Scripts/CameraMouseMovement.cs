using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMouseMovement : MonoBehaviour {

	public int   MovementZone = 150;
	public float MoveOffset = 0.15f;

	private Vector3 mousePos;
	
	void Update () {
		mousePos = Input.mousePosition;
		// left
		if (mousePos.x > 0 && mousePos.x < MovementZone) {
			transform.position = new Vector3(
				transform.position.x - MoveOffset, 
				transform.position.y, 
				transform.position.z);
		}

		// down
		if (mousePos.y > 0 && mousePos.y < MovementZone) {
			transform.position = new Vector3(
				transform.position.x, 
				transform.position.y, 
				transform.position.z - MoveOffset);
		}

		// rigth
		if (mousePos.x > Screen.width - MovementZone && mousePos.x < Screen.width) {
			transform.position = new Vector3(
				transform.position.x + MoveOffset, 
				transform.position.y, 
				transform.position.z);
		}

		// up
		if (mousePos.y > Screen.height - MovementZone && mousePos.y < Screen.height) {
			transform.position = new Vector3(
				transform.position.x, 
				transform.position.y, 
				transform.position.z + MoveOffset);
		}
	}
}

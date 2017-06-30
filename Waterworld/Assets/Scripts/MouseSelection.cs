using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelection : MonoBehaviour {

	Ray mouseRay;
	
	void Update () {
		// only interested if left mouse button
		if (!Input.GetMouseButtonDown(0)) { return; }
		RaycastHit hit;

		mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out hit);

		// nothing hit
		if (hit.distance == 0) { return; }

		Debug.Log(hit.transform.gameObject.name);
	}
}

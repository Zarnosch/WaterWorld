using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelection : MonoBehaviour {

	private Ray _mouseRay;
	
	void Update () {
		// only interested if left mouse button
		if (!Input.GetMouseButtonDown(0)) { return; }

		RaycastHit hit;
        _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(_mouseRay, out hit);

		// nothing hit
		if (hit.distance == 0) { return; }

		BuildingType type = null;
		try {
			type = hit.transform.gameObject.GetComponent<BuildingType>();
		} catch (System.NullReferenceException _e) {
			Debug.Log("Selecting building without 'BuildingType' something went wrong.");
			Debug.Log(_e.StackTrace);
			return;
		}

        if (type == null || type.Type == Building.Raft) { return; }

        GameManager.Instance.UI.ToggleUpgradeUI(type);
	}
}

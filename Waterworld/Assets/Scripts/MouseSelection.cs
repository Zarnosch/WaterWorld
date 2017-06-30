using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelection : MonoBehaviour {

	Ray mouseRay;
	
	void Update () {
		// only interested if left mouse button
		if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
		RaycastHit hit;
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out hit);

		// nothing hit
		if (hit.distance == 0) { return; }

        if(hit.transform.gameObject.name == "Raft")
        {
            Vec2i hitCell = hit.transform.gameObject.GetComponent<RaftBehaviour>().LightMapPosToHeightmap(hit.lightmapCoord);
            Debug.Log("Hit Raft on Cell: " + hitCell.X + " , " + hitCell.Y);
        }
        else
            Debug.Log(hit.transform.gameObject.name);
	}
}

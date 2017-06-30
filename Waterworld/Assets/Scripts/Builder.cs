using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Building {
	Raft,
	Fisherman,
	RainCapture,
	SeawaterPurification,
	MusselFarm,
	SeaweedFarm,
	LookOut,
	Sail,
	Rudder,
	Hood,
	Anchor,
	CoralDivers,

	NONE
}

public class Builder : MonoBehaviour {

	public RaftBehaviour Raft;

	private Building _selectedBuilding;
	private bool     _isBuilding;

	Ray mouseRay;

	public void Build(Building _building) {
		_selectedBuilding = _building;
		_isBuilding       = true;
	}

	void Update() {
		// only interested if left mouse button
		if (!Input.GetMouseButtonDown(0)) { return; }
		RaycastHit hit;
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(mouseRay, out hit);

		// nothing hit
		if (hit.distance == 0) { return; }

        if (hit.transform.gameObject.GetComponent<BuildingType>().Type == Building.Raft) {
            Vec2i raftPos = Raft.LightMapPosToHeightmap(hit.lightmapCoord);
			if (Raft.Build(raftPos, _selectedBuilding)) {
				_isBuilding       = false;
				_selectedBuilding = Building.NONE;
			}
        }
	}
}

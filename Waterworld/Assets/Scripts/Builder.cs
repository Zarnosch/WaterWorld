using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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

	public RaftBehaviour      Raft;

	public List<BuildingType> Buildings;

	private Building _selectedBuilding;
	private bool     _isBuilding;
	private Ray      _mouseRay;

	public void Build(Building _building) {
		_selectedBuilding = _building;
		_isBuilding       = true;
	}

	void Update() {
		// only interested if left mouse button
		if (!Input.GetMouseButtonDown(0)) { return; }

		RaycastHit hit;
        _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(_mouseRay, out hit);

		// nothing hit
		if (hit.distance == 0) { return; }

		if (!_isBuilding) { return; }
		Building type = Building.NONE;
		try {
			type = hit.transform.gameObject.GetComponent<BuildingType>().Type;
		} catch (System.NullReferenceException _e) {
			Debug.Log("Selecting building without 'BuildingType' something went wrong.");
			Debug.Log(_e.StackTrace);
			return;
		}

        if (type == Building.Raft) {
            Vec2i raftPos = Raft.LightMapPosToHeightmap(hit.lightmapCoord);
			if (Raft.Build(raftPos, _selectedBuilding)) {
				placeBuilding(_selectedBuilding, new Vector2(raftPos.X + 0.5f, raftPos.Y + 0.5f));
				_isBuilding       = false;
				_selectedBuilding = Building.NONE;
			}
        }
	}

	void placeBuilding(Building _building, Vector2 _pos) {
		Vector3 placePos = new Vector3(_pos.x, Raft.transform.position.y, _pos.y);

		BuildingType buildingPrefab = null;
		// var buildingPrefab = (from b in Buildings where b.Type == _building select b).ElementAt(0);
		foreach (var b in Buildings) {
			if (b.Type == _building) {
				buildingPrefab = b;
			}
		}
		Debug.Log(buildingPrefab);

		if (buildingPrefab == null) { return; }

		switch (_building) {
			case Building.Fisherman:
				BuildingType building = Instantiate(buildingPrefab);
				// building.transform.SetParent(transform);
				building.transform.position = placePos;
				break;
		}
	}

	public void Upgrade(BuildingType _building) {
		Debug.Log("TODO " + _building.Type.ToString());
	}
}

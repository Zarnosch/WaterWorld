using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public GameObject UpgradePanel;

	private Text _panelTitel;

	[System.NonSerialized]
	public BuildingType SelectedBuilding;

	void Awake() {
		_panelTitel = UpgradePanel.GetComponentInChildren<Text>();
	}

    public void ToggleUpgradeUI(BuildingType _building) {
		UpgradePanel.gameObject.SetActive(!UpgradePanel.gameObject.activeInHierarchy);
		
		if (UpgradePanel.gameObject.activeInHierarchy) {
			SelectedBuilding = _building;
			_panelTitel.text = SelectedBuilding.Type.ToString();
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public GameObject UpgradePanel;

	private Text _panelTitel;

	void Awake() {
		_panelTitel = UpgradePanel.GetComponentInChildren<Text>();
	}

    public void ToggleUpgradeUI(string _buildingName) {
		UpgradePanel.gameObject.SetActive(!UpgradePanel.gameObject.activeInHierarchy);
		
		if (UpgradePanel.gameObject.activeInHierarchy) {
			_panelTitel.text = _buildingName;
		}
    }
}

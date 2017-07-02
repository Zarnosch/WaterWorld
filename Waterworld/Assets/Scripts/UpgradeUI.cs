using UnityEngine;

public class UpgradeUI : MonoBehaviour {

	public void UpgradeBuilding() {
		GameManager.Instance.Builder.Upgrade(GameManager.Instance.UI.SelectedBuilding);
	}

}

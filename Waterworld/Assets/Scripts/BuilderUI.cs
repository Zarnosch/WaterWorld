using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderUI : MonoBehaviour {

	public void BuildFisherman() {
		GameManager.Instance.Builder.Build(Building.Fisherman);
	}
}

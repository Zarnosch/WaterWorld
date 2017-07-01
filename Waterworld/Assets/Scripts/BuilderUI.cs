using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderUI : MonoBehaviour {

	public void BuildRaft() {
		GameManager.Instance.Builder.Build(Building.Raft);
	}

	public void BuildFisherman() {
		GameManager.Instance.Builder.Build(Building.Fisherman);
	}

	public void BuildRainCapture() {
		GameManager.Instance.Builder.Build(Building.RainCapture);
	}

	public void BuildSeawaterPurification() {
		GameManager.Instance.Builder.Build(Building.SeawaterPurification);
	}

	public void BuildMusselFarm() {
		GameManager.Instance.Builder.Build(Building.MusselFarm);
	}

	public void BuildSeaweedFarm() {
		GameManager.Instance.Builder.Build(Building.SeaweedFarm);
	}

	public void BuildLookOut() {
		GameManager.Instance.Builder.Build(Building.LookOut);
	}

	public void BuildSail() {
		GameManager.Instance.Builder.Build(Building.Sail);
	}

	public void BuildRudder() {
		GameManager.Instance.Builder.Build(Building.Rudder);
	}

	public void BuildHood() {
		GameManager.Instance.Builder.Build(Building.Hood);
	}

	public void BuildAnchor() {
		GameManager.Instance.Builder.Build(Building.Anchor);
	}

	public void BuildCoralDivers() {
		GameManager.Instance.Builder.Build(Building.CoralDivers);
	}
}

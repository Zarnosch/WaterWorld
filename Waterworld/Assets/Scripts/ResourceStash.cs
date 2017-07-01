using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Resource {
	Water,
	Nutriment,
	Material,
	Coral
}

public class ResourceStash : MonoBehaviour {

	[System.NonSerialized]
	public Dictionary<Resource, int> Resources;

	public GameObject ResourcesUI;

	private Dictionary<Building, Dictionary<Resource, int>> ResourcesPerBuilding;
	private Dictionary<Building, float> UpgradeAmountPerBuilding;

	public float ResourceRecieveSpeed = 5.0f;
	private float _counter;

	void Awake() {
		Resources = new Dictionary<Resource, int> {
			{ Resource.Nutriment, 500 },
			{ Resource.Water,     500 },
			{ Resource.Material,  100 },
			{ Resource.Coral,     0   },
		};

		ResourcesPerBuilding = new Dictionary<Building, Dictionary<Resource, int>> {
			{ Building.Fisherman,            new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.Anchor,               new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.CoralDivers,          new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.Hood,                 new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.LookOut,              new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.MusselFarm,           new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.RainCapture,          new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.Rudder,               new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.Sail,                 new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.SeaweedFarm,          new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } },
			{ Building.SeawaterPurification, new Dictionary<Resource, int> { { Resource.Nutriment, 10 } } }
		};

		UpgradeAmountPerBuilding = new Dictionary<Building, float> {
			{ Building.Fisherman,            1.0f },
			{ Building.Anchor,               1.0f },
			{ Building.CoralDivers,          1.0f },
			{ Building.Hood,                 1.0f },
			{ Building.LookOut,              1.0f },
			{ Building.MusselFarm,           1.0f },
			{ Building.RainCapture,          1.0f },
			{ Building.Rudder,               1.0f },
			{ Building.Sail,                 1.0f },
			{ Building.SeaweedFarm,          1.0f },
			{ Building.SeawaterPurification, 1.0f }
		};

		updateUI();
	}

	public bool ConsumeResources(Dictionary<Resource, int> _res) {		
		foreach (var item in _res) {
			var resource     = item.Key;
			var amountNeeded = item.Value;

			int amountInStash = Resources[resource];
			
			if (amountInStash < amountNeeded) { return false; }

			Resources[resource] = amountInStash - amountNeeded;

			updateUI();
			return true;
		}

		return false;
	}

	private void updateUI() {
		var resourcesLabels = ResourcesUI.GetComponentsInChildren<Text>();
		foreach (var label in resourcesLabels) {
			switch (label.gameObject.name) {
				case "Water":
					label.text = "Water: " + Resources[Resource.Water];
					break;
				case "Nutriment":					
					label.text = "Nutriment: " + Resources[Resource.Nutriment];
					break;
				case "Material":					
					label.text = "Material: " + Resources[Resource.Material];
					break;
				case "Coral":
					label.text = "Coral: " + Resources[Resource.Coral];
					break;
			}
		}
	}

	public Dictionary<Resource, int> GetResourcesNeededToBuild(Building _building) {
		switch (_building) {
			case Building.Fisherman:
				return new Dictionary<Resource, int> {
					{ Resource.Material, 10 }
				};
			
			default:
				return new Dictionary<Resource, int>();
		}
	}

	public Dictionary<Resource, int> GetResourcesNeededToUpgrade(Building _building) {
		switch (_building) {
			case Building.Fisherman:
				return new Dictionary<Resource, int> {
					{ Resource.Coral,     5 },
					{ Resource.Material, 10 }
				};
			
			default:
				return new Dictionary<Resource, int>();
		}
	}

	private void applyResources() {
		foreach (var item in GameManager.Instance.Builder.BuildingAmount) {
			var building = item.Key;
			var amount   = item.Value;

			if (amount <= 0) { continue; }

			var adds = ResourcesPerBuilding[building];

			for (var i = 0; i < amount; i++) {
				foreach (var addition in adds) {
					var resource = addition.Key;
					var add      = addition.Value;

					Resources[resource] += add;
				}	
			}			
		}

		updateUI();
	}

	void Update() {
		_counter += 0.5f * Time.deltaTime;
		if (_counter > ResourceRecieveSpeed) {
			_counter = 0;

			applyResources();
		}
	}

}

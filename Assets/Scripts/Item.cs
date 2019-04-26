using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor
//Represents an item which can be collected
[System.Serializable]
public class Item{

	private string name;

	public Item (string name) {
		this.name = name;
	}

	public string getName() {
		return name;
	}
}

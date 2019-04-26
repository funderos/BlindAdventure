using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor, Benjamin Funder
//Represents an obstacle from a steeplechase
[System.Serializable]
public class Obstacle{

	private int time; //Time until obstacle appears
	private int direction; //Direction where the obstacle appears

	public void setTime (int time) {
		this.time = time;
	}

	public int getTime() {
		return time;
	}

	public void setDirection (int direction) {
		this.direction = direction;
	}

	public int getDirection() {
		return direction;
	}
}

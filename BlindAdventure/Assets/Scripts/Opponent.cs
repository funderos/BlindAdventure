using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor
//Represents an opponent from a fight
[System.Serializable]
public class Opponent{

	private int time; //Time until opponent appears
	private string opponentTyp; //Typ of the opponent (dungeon, lion, witch)
	private int direction; //Direction where the opponent appears

	public void setTime (int time) {
		this.time = time;
	}

	public int getTime() {
		return time;
	}

	public void setTyp (string typ) {
		this.opponentTyp = typ;
	}

	public string getTyp() {
		return opponentTyp;
	}

	public void setDirection (int direction) {
		this.direction = direction;
	}

	public int getDirection() {
		return direction;
	}
}

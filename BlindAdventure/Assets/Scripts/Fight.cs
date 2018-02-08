using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor
//Represents the minigame "fight"
[System.Serializable]
public class Fight{

	private List <Opponent> opponentList = new List <Opponent> (); //List to save opponents

	public void setList (List <Opponent> opponentList) {
		this.opponentList = opponentList;
	}

	public List<Opponent> getList() {
		return opponentList;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor
//Represents the minigame "steeplechase"
[System.Serializable]
public class Steeplechase{

	private List <Obstacle> obstacleList = new List <Obstacle> (); //List to save obstacles

	public void setList (List <Obstacle> obstacleList) {
		this.obstacleList = obstacleList;
	}

	public List<Obstacle> getList() {
		return obstacleList;
	}
}

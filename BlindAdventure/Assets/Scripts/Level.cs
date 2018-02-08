using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: Stadler Viktor
//Represents a level
[System.Serializable]
public class Level{

	private List <Node> nodeList = new List <Node> (); //List to save nodes
	private int levelNumber;
	private int backgroundMusic;
	private bool levelCompleted = false; //if all items from the level are collected -> true

	public Level (List <Node> nodeList, int levelNumber) {
		this.nodeList = nodeList;
		this.levelNumber = levelNumber;
	}
	public Level(){}

	public void setLevelNumber (int levelNumber){
		this.levelNumber = levelNumber;
	}

	public int getLevelnumber () {
		return levelNumber;
	}

	public void setList (List <Node> nodeList) {
		this.nodeList = nodeList;
	}

	public List<Node> getList() {
		return nodeList;
	}

	public void setBackgroundMusic(int backgroundMusic) {
		this.backgroundMusic = backgroundMusic;
	}

	public int getBackgroundMusic(){
		return backgroundMusic;
	}
		
	public void setLevelCompleted(bool levelCompleted) {
		this.levelCompleted = levelCompleted;
	}

	public bool getLevelCompleted(){
		return levelCompleted;
	}
}

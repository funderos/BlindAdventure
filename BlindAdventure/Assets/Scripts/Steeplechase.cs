using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Stadler Viktor
//Represents the minigame "steeplechase"
[System.Serializable]
public class Steeplechase{

    private int obstacleNumber = 0;
    private List<KeyValuePair<int, Obstacle>> obstacleList = new List<KeyValuePair<int, Obstacle>>(); //List to save obstacle number with appropriate object

    public int getObstacleNumber()
    {
        return obstacleNumber;
    }

    public void setObstacleNumber(int obstacleNumber)
    {
        this.obstacleNumber = obstacleNumber;
    }

    public void setList (List<KeyValuePair<int, Obstacle>> list) {
		this.obstacleList = list;
	}

    public List<KeyValuePair<int, Obstacle>> getList()
    {
        return obstacleList;
    }

    public void newObstacle()
    {
        obstacleNumber++;
    }
}

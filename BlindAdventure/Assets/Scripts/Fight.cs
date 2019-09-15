using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authors: Stadler Viktor, Funder Benjamin
//Represents the minigame "fight"
[System.Serializable]
public class Fight{

    private int opponentCount = 1;
    private int opponentNumber = 0;
    List<KeyValuePair<int, int>> opponentList = new List<KeyValuePair<int, int>>(); //List to save the opponent number with the appropriate direction

    public int getOpponentNumber()
    {
        return opponentNumber;
    }

    public void setOpponentNumber(int opponentNumber)
    {
        this.opponentNumber = opponentNumber;
    }

    public int getOpponentCount()
    {
        return opponentCount;
    }

    public void setOpponentCount(int opponentNumber)
    {
        this.opponentCount = opponentNumber;
    }

    public List<KeyValuePair<int, int>> getList()
    {
        return opponentList;
    }

    public void setList(List<KeyValuePair<int, int>> list)
    {
        this.opponentList = list;
    }

    public void newOpponent()
    {
        opponentNumber++;
    }
}

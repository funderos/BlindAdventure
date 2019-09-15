using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authors: Stadler Viktor
//Represents the minigame "quiz"
[System.Serializable]
public class Quiz{

	private int questionNumber = 0;
	private List<KeyValuePair<int,int>> questionList = new List <KeyValuePair<int,int>> (); //List to save the question number with the appropriate answer

	public int getQuestionNumber() {
		return questionNumber;
	}

	public void setQuestionNumber(int questionNumber) {
		this.questionNumber = questionNumber;
	}

	public List<KeyValuePair<int,int>> getList() {
		return questionList;
	}

	public void setList(List<KeyValuePair<int,int>> list) {
		this.questionList = list;
	}

	public void newQuestion() {
		questionNumber++;
	}
}

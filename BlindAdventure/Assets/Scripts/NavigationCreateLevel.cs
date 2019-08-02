using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

//Author: Stadler Viktor
//This class manages the methods to navigate in the CreateLevelScene
public class NavigationCreateLevel : MonoBehaviour {

	private Vector3 menuPosition; //returns the new vector3 position
	public XMLReader xmlReader;

	//Returns the new Vector3 position and gives an audio output which new functions are possible. Depends on which Button was pressed.
	public Vector3 navigateTo (string position) {
		Handheld.Vibrate();
        TTSManager.Stop();
		if(position == "BackgroundMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateBackgroundMenu"), false);
			menuPosition = Vector3.left * 800; //BackgroundMenu
			if (PlayerPrefs.HasKey ("backgroundName")) {
				string backgroundName = PlayerPrefs.GetString ("backgroundName"); //Gets the current position name in the BackgroundMenu
				TTSManager.Speak (backgroundName, true);
			} else {
				TTSManager.Speak ("Musik 1", true);
			}
		}
		else if(position == "CreateNodeMenu") {
			menuPosition = Vector3.left * 1600; //CreateNodeMenu
		}
		else if (position == "NodeNameMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateNodeNameMenu"), false);
			menuPosition = Vector3.left * 2400; //NodeNameMenu
		}
		else if(position == "MinigameMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateMinigameMenu"), false);
			menuPosition = Vector3.left * 3200; //MinigameMenu
			if (PlayerPrefs.HasKey ("minigameName")) {
				TTSManager.Speak (PlayerPrefs.GetString ("minigameName"), true); //Gets the current position name in the MinigameMenu
			} else {
				if(PlayerPrefs.GetInt ("Language") == 1)
					TTSManager.Speak ("Hindernislauf", true);
				else 
					TTSManager.Speak ("Steeplechase", true);
			}
		}
		else if(position == "ItemMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateItemMenu"), false);
			menuPosition = Vector3.left * 4000; //ItemMenu
		}
		else if(position == "QuizMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateQuizMenu"), false);
			menuPosition = Vector3.left * 4800; //QuizMenu
		} 
		else if(position == "ExitNodeMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateExitNodeMenu"), false);
			menuPosition = Vector3.left * 10400; //ExitNodeMenu
		}
		else if(position == "FightMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateFightMenu"), false);
			menuPosition = Vector3.left * 5600; //FightMenu
		}
		else if(position == "OpponentMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateOpponentMenu"), false);
			menuPosition = Vector3.left * 6400; //OpponentMenu
		}
		else if(position == "ExitFightMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateExitFightMenu"), false);
			menuPosition = Vector3.left * 7200; //ExitFightMenu
		}
		else if(position == "SteeplechaseMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateSteeplechaseMenu"), false);
			menuPosition = Vector3.left * 8000; //SteeplechaseMenu
		}
		else if(position == "ObstacleMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateObstacleMenu"), false);
			menuPosition = Vector3.left * 8800; //ObstacleMenu
		}
		else if(position == "ExitSteeplechaseMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateExitSteeplechaseMenu"), false);
			menuPosition = Vector3.left * 9600; //ExitSteeplechaseMenu
		}
		else if(position == "EditLevelMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateEditLevelMenu"), false);
			menuPosition = Vector3.left * -800; //EditLevelMenu
		}
		else if(position == "SelectNodeMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateSelectNodeMenu"), false);
			menuPosition = Vector3.left * -1600; //SelectNodeMenu
		}
		else if(position == "ChangeNodeMenu") {
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateChangeNodeMenu"), false);
			menuPosition = Vector3.left * -2400; //ChangeNodeMenu
		}
		else {
			menuPosition = Vector3.zero; //LevelOptionMenu
		}
		return menuPosition;
	}

	//Returns the new Vector3 position and gives an audio output which new functions are possible. Depends on which vector3 position the swipe-up was.
	public Vector3 swipeUp (Vector3 menuPosition) {
		Handheld.Vibrate ();
        TTSManager.Stop();
		if (menuPosition == Vector3.left * 800) { //BackgroundMenu
			SceneManager.LoadScene("CreateLevelScene"); //Loads scene "CreateLevelScene"  
		} else if (menuPosition == Vector3.left * 1600) { //CreateNodeMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				SceneManager.LoadScene("CreateLevelScene"); //Loads scene "CreateLevelScene"  
			}
		} else if (menuPosition == Vector3.left * 2400) { //NodeNameMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
				menuPosition = Vector3.left * 1600; //CreateNodeMenu
			}
		} else if (menuPosition == Vector3.left * 3200) { //MinigameMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateNodeNameMenu"), false);
				menuPosition = Vector3.left * 2400; //NodeNameMenu
			}
		} else if (menuPosition == Vector3.left * 4000) { //ItemMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp2") + xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
				deleteQuestions ();
				menuPosition = Vector3.left * 1600; //CreateNodeMenu
			}
		} else if (menuPosition == Vector3.left * 5600) { //FightMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp2") + xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
				menuPosition = Vector3.left * 1600; //CreateNodeMenu
			}
		} else if (menuPosition == Vector3.left * 6400) { //OpponentMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateFightMenu"), false);
				menuPosition = Vector3.left * 5600; //FightMenu
			}
		} else if (menuPosition == Vector3.left * 8000) { //SteeplechaseMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp2") + xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
				menuPosition = Vector3.left * 1600; //CreateNodeMenu
			}
		} else if (menuPosition == Vector3.left * 8800) { //ObstacleMenu
			if (CreateLevelMenu.changeNode == true) {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp"), false);
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateSteeplechaseMenu"), false);
				menuPosition = Vector3.left * 8000; //SteeplechaseMenu
			}
		} else if (menuPosition == Vector3.left * 4800) { //QuizMenu
			CreateLevelMenu.direction = 2; //Sets the direction for án answer
		} else if (menuPosition == Vector3.left * 7200) { //ExitFightMenu
			CreateLevelMenu.direction = 2; //Sets the direction for án opponent
		} else if (menuPosition == Vector3.left * 9600) { //ExitSteeplechaseMenu
			CreateLevelMenu.direction = 2; //Sets the direction for án obstacle
		} else if (menuPosition == Vector3.left * 10400) { //ExitNodeMenu
			if (CreateLevelMenu.saveNodeButton == false) { //If minigame already saved
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeUp3"), false); 
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateItemMenu"), false);
				TTSManager.Speak (PlayerPrefs.GetString ("itemName"), true); //Gets the current position name in the ItemMenu
				menuPosition = Vector3.left * 4000; //ItemMenu
			}
		} else if (menuPosition == Vector3.left * -800) { //EditLevelMenu
			SceneManager.LoadScene("CreateLevelScene"); //Loads scene "CreateLevelScene"  
		} else if (menuPosition == Vector3.left * -1600) { //SelectNodeMenu
			SceneManager.LoadScene("CreateLevelScene"); //Loads scene "CreateLevelScene"  
		} else if (menuPosition == Vector3.left * -2400) { //ChangeNodeMenu
			SceneManager.LoadScene("CreateLevelScene"); //Loads scene "CreateLevelScene"  
		} else if (menuPosition == Vector3.zero) { //LevelOptionMenu
			SceneManager.LoadScene("MainScene"); //Loads scene "MainScene" 
		} 
		return menuPosition;
	}

	//Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
	public void swipeDown (Vector3 menuPosition) {
		Handheld.Vibrate ();
        TTSManager.Stop();
		if (menuPosition == Vector3.zero) { //LevelOptionMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelLevelOptionMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * -800){ //EditLevelMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelEditLevelMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * -1600){ //SelectNodeMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSelectNodeMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * -2400){ //ChangeNodeMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelChangeNodeMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 800){ //BackgroundMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelBackgroundMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 1600){ //CreateNodeMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelCreateNodeMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 4000){ //ItemMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelItemMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 10400){ //ExitNodeMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelExitNodeExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 2400){ //NodeNameMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNodeNameMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 3200){ //MinigameMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelMinigameMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 4800){ //QuizMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelQuizMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 5600){ //FightMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelFightMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 6400){ //OpponentMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelOpponentMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 7200){ //ExitFightMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelExitFightMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 8000){ //SteeplechaseMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSteeplechaseMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 8800){ //ObstacleMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelObstacleMenuExplanation"), false);
		}
		else if(menuPosition == Vector3.left * 9600){ //ExitSteeplechaseMenu
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelExitSteeplechaseMenuExplanation"), false);
		}
		else { //No explanation available
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelErrorExplanation"), false);
		}
	}

	//Sets the direction for an answer, opponent or obstacle. Depends on which vector3 position the swipe-right was. 
	public void swipeRight (Vector3 menuPosition) {
		if (menuPosition == Vector3.left * 4800) { //QuizMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 3; //Sets the direction for án answer
		}
		if (menuPosition == Vector3.left * 7200) { //ExitFightMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 3; //Sets the direction for án opponent
		}
		if (menuPosition == Vector3.left * 9600) { //ExitSteeplechaseMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 3; //Sets the direction for án obstacle
		}
		if (menuPosition == Vector3.left * 10400) { //Loads scene "CreateLevelScene" (Swipe-right in the ExitNodeMenu)
			Handheld.Vibrate ();
			if (CreateLevelMenu.changeNode == false) {
				if (CreateLevelMenu.saveNodeButton == true) { //Deletes recorded questions when the minigame discarded
					deleteQuestions();
				}
				SceneManager.LoadScene ("CreateLevelScene");
			} else {
				TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelSwipeRight"), false); //Changed node not saved
			}
		}
	}

	//Sets the direction for an answer, opponent or obstacle. Depends on which vector3 position the swipe-left was. 
	public void swipeLeft (Vector3 menuPosition) {
		if (menuPosition == Vector3.left * 4800) { //QuizMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 1; //Sets the direction for án answer
		}
		if (menuPosition == Vector3.left * 7200) { //ExitFightMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 1; //Sets the direction for án opponent
		}
		if (menuPosition == Vector3.left * 9600) { //ExitSteeplechaseMenu
			Handheld.Vibrate ();
			CreateLevelMenu.direction = 1; //Sets the direction for án obstacle
		}
	}

	//Deletes recorded questions when the minigame discarded
	public void deleteQuestions() {
		int levelNumber = PlayerPrefs.GetInt ("LevelNumber");
		int k = 1;
		while(File.Exists (Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + CreateLevelMenu.nodeIndex + "Question" + k + ".wav")) {
			File.Delete (Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + CreateLevelMenu.nodeIndex + "Question" + k + ".wav");
			k++;
		}
	}
}

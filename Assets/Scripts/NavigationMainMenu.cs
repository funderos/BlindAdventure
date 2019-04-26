using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author: Stadler Viktor
//This class manages the methods to navigate in the Mainscene
public class NavigationMainMenu : MonoBehaviour {

	private Vector3 menuPosition; //returns the new vector3 position
	public XMLReader xmlReader;

	//Returns the new Vector3 position. Depends on which Button was pressed.
	public Vector3 navigateTo (string position) {
		Handheld.Vibrate ();
		if (position == "CreateLevelMenu") {
			menuPosition = Vector3.left * 800; //CreateLevelMenu
		}
		else if(position == "StartGameMenu") {
			menuPosition = Vector3.right * 800; //StartGameMenu
		}
		else {
			menuPosition = Vector3.zero; //MainMenu
		}
		return menuPosition;
	}
		
	//Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
	public void swipeDown (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.zero) { //MainMenu
			TTSManager.Speak (xmlReader.translate ("MainMenuExplanation"), false);
		} else if (menuPosition == Vector3.left * 800) { //CreateLevelMenu
			TTSManager.Speak (xmlReader.translate ("CreateLevelExplanation"), false);
		} else { //StartGameMenu
			TTSManager.Speak (xmlReader.translate ("StartGameMenuExplanation"), false);
		}
	}
}

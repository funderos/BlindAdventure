using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author: Stadler Viktor
//This class manages the methods to navigate in the PlayLevelNetworkScene
public class NavigationPlayLevelNetwork : MonoBehaviour {

	private Vector3 menuPosition; //returns the new vector3 position
	public XMLReader xmlReader;

	//Returns the new Vector3 position. Depends on which Button was pressed.
	public Vector3 navigateTo (string position) {
		Handheld.Vibrate ();
		if (position == "PlayNodeMenu") {
			menuPosition = Vector3.left * 800; //PlayNodeMenu
		} else {
			menuPosition = Vector3.zero; //SelectNodeMenu
		}
		return menuPosition;
	}

	//Loads scene "Mainscene" or sets the direction for án answer, opponent or obstacle. Depends on which vector3 position the swipe-up was.
	public void swipeUp (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.left * 800) { //PlayNodeMenu
			PlayLevelMenuNetwork.direction = 2; //Sets the direction for án answer, opponent or obstacle
		} else{ 
			SceneManager.LoadScene("NetworkScene"); //Loads scene "Networkscene"
		} 
	}

	//Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
	public void swipeDown (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.left * 800) { //PlayNodeMenu
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuExplanation2"), false);
		} else { //SelectNodeMenu
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuExplanation"), false);
		}
	}

	//Sets the direction for an answer, opponent, obstacle or the direction for the next node. Depends on which vector3 position the swipe-right was. 
	public int swipeRight (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.left * 800) { //PlayNodeMenu
			return 3; //Sets the direction for an answer, opponent or obstacle
		} else { //SelectNodeMenu
			return 33; //next node
		} 
	}

	//Sets the direction for an answer, opponent, obstacle or the direction for the next node. Depends on which vector3 position the swipe-left was.
	public int swipeLeft (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.left * 800) { //PlayNodeMenu
			return 1; //Sets the direction for an answer, opponent or obstacle
		} else { //SelectNodeMenu
			return 11; //previous node
		} 
	}
}

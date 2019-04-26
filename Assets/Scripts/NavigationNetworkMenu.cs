using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author: Stadler Viktor
//This class manages the methods to navigate in the NetworkScene
public class NavigationNetworkMenu : MonoBehaviour {

	private Vector3 menuPosition; //returns the new vector3 position
	public XMLReader xmlReader;

	//Returns the new Vector3 position. Depends on which Button was pressed.
	public Vector3 navigateTo (string position) {
		if (position == "RegisterNameMenu") {
			menuPosition = Vector3.left * 800; 
			int i = 0;
			PlayerPrefs.SetInt ("FromPlayLevelMenuNetwork", i);
		}
		else if(position == "RegisterPasswordMenu") {
			menuPosition = Vector3.left * 1600; 
		}
		else if(position == "RegisterEmailMenu") {
			menuPosition = Vector3.left * 2400;  
		}
		else if(position == "LoginMenu") {
			menuPosition = Vector3.right * 800; 
			int i = 0;
			PlayerPrefs.SetInt ("FromPlayLevelMenuNetwork", i);
		}
		else if(position == "LoginPasswordMenu") {
			menuPosition = Vector3.right * 1600; 
		}
		else if(position == "UploadMenu") {
			menuPosition = Vector3.right * 2400; 
		}
		else if(position == "SearchMenu") {
			menuPosition = Vector3.right * 3200; 
		}
		else if(position == "DownloadMenu") {
			menuPosition = Vector3.right * 4000; 
		}
		else if(position == "LevelMenu") {
			menuPosition = Vector3.right * 4800; 
		}
		else {
			menuPosition = Vector3.zero; //MainMenu
		}
		return menuPosition;
	}

	//Returns the new Vector3 position and gives an audio output which new functions are possible. Depends on which vector3 position the swipe-up was.
	public Vector3 swipeUp (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.zero) { //LoginRegisterMenu
			SceneManager.LoadScene ("MainScene"); //Loads scene "MainScene" 
		} else if (menuPosition == Vector3.left * 800) { //RegisterNameMenu
			SceneManager.LoadScene ("NetworkScene"); //Loads scene "NetworkScene"  
		} else if (menuPosition == Vector3.left * 1600) { //RegisterPasswordMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuRegisterButton"), false);
			menuPosition = Vector3.left * 800; //RegisterNameMenu
		} else if (menuPosition == Vector3.left * 2400) { //RegisterEmailMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuConfirmNameButton"), false);
			menuPosition = Vector3.left * 1600; //RegisterPasswordMenu
		} else if (menuPosition == Vector3.left * -800) { //LoginNameMenu
			SceneManager.LoadScene ("NetworkScene"); //Loads scene "NetworkScene" 
		} else if (menuPosition == Vector3.left * -1600) { //LoginPasswordMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginButton"), false);
			menuPosition = Vector3.left * -800; //LoginNameMenu
		} else if (menuPosition == Vector3.left * -2400) { //UploadMenu
			SceneManager.LoadScene ("MainScene"); //Loads scene "MainScene" 
		} else if (menuPosition == Vector3.left * -3200) { //SearchMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginToDB"), false);
			menuPosition = Vector3.left * -2400; //UploadMenu
		} else if (menuPosition == Vector3.left * -4000) { //DownloadMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSearchButton"), false);
			menuPosition = Vector3.left * -3200; //SearchMenu
		} else if (menuPosition == Vector3.left * -4800 && PlayerPrefs.GetInt ("FromPlayLevelMenuNetwork") != 1) { //LevelMenu
			SceneManager.LoadScene ("MainScene"); //Loads scene "MainScene"
		}
		return menuPosition;
	}

	//Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
	public void swipeDown (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.zero) {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginRegisterMenu"), false); 
		} else if (menuPosition == Vector3.left * 800) { //RegisterNameMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuRegisterNameMenu"), false); 
		} else if (menuPosition == Vector3.left * 1600) { //RegisterPasswordMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuRegisterPasswordMenu"), false); 
		} else if (menuPosition == Vector3.left * 2400) { //RegisterEmailMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuRegisterEmailMenu"), false); 
		} else if (menuPosition == Vector3.left * -800) { //LoginNameMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginNameMenu"), false); 
		} else if (menuPosition == Vector3.left * -1600) { //LoginPasswordMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginPasswordMenu"), false); 
		} else if (menuPosition == Vector3.left * -2400) { //UploadMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuUploadMenu"), false); 
		} else if (menuPosition == Vector3.left * -3200) { //SearchMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSearchMenu"), false); 
		} else if (menuPosition == Vector3.left * -4000) { //DownloadMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuDownloadMenu"), false); 
		} else if (menuPosition == Vector3.left * -4800) { //LevelMenu
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLevelMenu"), false); 
		}
	}
}

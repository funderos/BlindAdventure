using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Authors: Stadler Viktor
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
        else if (position == "CreateEditLevelMenu")
        {
            menuPosition = Vector3.left * 1600; //CreateLevelMenu
        }
        else if (position == "DeleteSureMenu")
        {
            menuPosition = Vector3.right * 1600; //MainMenuReallyDelete
        }
        else {
			menuPosition = Vector3.zero; //MainMenu
		}
		return menuPosition;
	}

    public void swipeUp(Vector3 menuPosition)
    {
        Handheld.Vibrate();
        if (menuPosition == Vector3.left * 800)
        { //PlayNodeMenu
            PlayLevelMenu.direction = 2; //Sets the direction for án answer, opponent or obstacle
        }
        else
        {
            SceneManager.LoadScene("MainScene"); //Loads scene "Mainscene"
        }
    }

    //Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
    public void swipeDown (Vector3 menuPosition) {
		Handheld.Vibrate ();
		if (menuPosition == Vector3.zero) { //MainMenu
			TTSManager.Speak (xmlReader.translate ("MainMenuExplanation"), false);
		} else if (menuPosition == Vector3.left * 800) { //CreateLevelMenu
			TTSManager.Speak (xmlReader.translate ("CreateLevelExplanation"), false);
		} else if (menuPosition == Vector3.left * 1600) // CreateEditLevelMenu
        {
            TTSManager.Speak(xmlReader.translate("CreateEditLevelMenuExplanation"), false);
        }
        else if (menuPosition == Vector3.right * 1600) // DeleteSureMenu
        {
            TTSManager.Speak(xmlReader.translate("DeleteSureMenuExplanation"), false);
        }
        else
        { //StartGameMenu
			TTSManager.Speak (xmlReader.translate ("StartGameMenuExplanation"), false);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//Authors: Stadler Viktor, Funder Benjamin
//This class manages the methods in the start menu
public class StartMenu : MonoBehaviour {

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true

	void Awake(){
		StartCoroutine (outputAwake ());
	}

    IEnumerator outputAwake()
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/CurrentGame/");
        Directory.CreateDirectory(Application.persistentDataPath + "/SavedGames/");
        Directory.CreateDirectory(Application.persistentDataPath + "/DownloadTemp/");
        TTSManager.Initialize(transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
        while (!TTSManager.IsInitialized())
        {
            yield return null;
        }
        TTSManager.SetLanguage(TTSManager.ENGLISH);
        if (!PlayerPrefs.HasKey("Language"))
        { //If there is no player preferences key "Language" (first start of the app), standard language is english
            xmlReader.setLanguage(0);
            PlayerPrefs.SetInt("Language", 0);
            language = 0;
        }
            language = PlayerPrefs.GetInt("Language"); //Gets the current language from the player preferences; german = 1 or english = 0
        if (language == 0)
        {
            xmlReader.setLanguage(0);
            TTSManager.SetLanguage(TTSManager.ENGLISH);
        }
        else
        {
            xmlReader.setLanguage(1);
            TTSManager.SetLanguage(TTSManager.GERMAN);
            language = 1;
        }
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }
        TTSManager.Speak(xmlReader.translate("StartMenuExplanation"), false);
        if (!PlayerPrefs.HasKey("NetworkUser") || !PlayerPrefs.HasKey("NetworkPW"))
        { //If no Email settings are set, the default value will be used
            LoadSaveGame.setStandardUser();
        }
    }

	void Update () {
		//Detects swipe direction 
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				startPosition = touch;
			} 
			else if (swiped == false && touch.phase == TouchPhase.Moved) {
				float differenceX = startPosition.position.x - touch.position.x;
				float differenceY = startPosition.position.y - touch.position.y;
				if (Mathf.Abs (differenceX) > 5 || Mathf.Abs (differenceY) > 5) {
					if (Mathf.Abs (differenceX) < Mathf.Abs (differenceY)) {
						//Swipe Down
						if (differenceY > 0) {
							Handheld.Vibrate ();
							TTSManager.Speak (xmlReader.translate ("StartMenuGameExplanation"), false);
						}
						//Swipe Up
						if (differenceY < 0) {
							Handheld.Vibrate ();
							if (language == 0) { //Change language to german
								xmlReader.setLanguage (1);
								PlayerPrefs.SetInt ("Language", 1); 
								language = 1;
								TTSManager.SetLanguage (TTSManager.GERMAN);
								TTSManager.Speak ("Neue Sprache, Deutsch!" + xmlReader.translate ("StartMenuExplanation"), false);
							} else { //Change language to english
								xmlReader.setLanguage (0);
								PlayerPrefs.SetInt ("Language", 0); 
								language = 0;
								TTSManager.SetLanguage (TTSManager.ENGLISH);
								TTSManager.Speak ("New Language, English!" + xmlReader.translate ("StartMenuExplanation"), false);
							}
						}
					}
					if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
						//Swipe Right
						if (differenceX < 0) {
							Handheld.Vibrate ();
							Application.Quit (); //Close App
						}
					}
					swiped = true;
				}
			} 				
			else if (touch.phase == TouchPhase.Ended) {
				startPosition = new Touch ();
				swiped = false;
			}
		}
	}

	//Navigates to the Scene "MainScene"
	public void onMainMenuButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			SceneManager.LoadScene("MainScene");
		}
	}
}

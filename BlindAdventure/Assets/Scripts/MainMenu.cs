using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

//Author: Stadler Viktor
//This class manages the methods in the main menu
public class MainMenu : MonoBehaviour {

	//Variables to navigate 
	private Vector3 menuPosition;	
	public RectTransform menuContainer;
	public NavigationMainMenu navigationMainMenu;

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language

	public AudioSource audioSource;

	void Awake(){
		StartCoroutine (outputAwake ());
		int i = 0;
		PlayerPrefs.SetInt ("FromPlayLevelMenuNetwork", i);
	}

	IEnumerator outputAwake() {
		TTSManager.Initialize (transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
		while(!TTSManager.IsInitialized()){
			yield return null;
		}
		language = PlayerPrefs.GetInt ("Language"); //Gets the current language from the player preferences; german = 1 or english = 0
		if (language == 1) {
			xmlReader.setLanguage (1);
			TTSManager.SetLanguage (TTSManager.GERMAN);
		} else {
			xmlReader.setLanguage (0);
			TTSManager.SetLanguage (TTSManager.ENGLISH);
		}
		TTSManager.Speak (xmlReader.translate ("MainMenuAwake"), false);
	}
		
	void Update () {
		menuContainer.anchoredPosition3D = Vector3.Lerp (menuContainer.anchoredPosition3D, menuPosition, 1f); //To navigate to a position

		//Detects swipe direction 
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				startPosition = touch;
			} 
			else if (swiped == false && touch.phase == TouchPhase.Moved) {
				float differenceX = startPosition.position.x - touch.position.x;
				float differenceY = startPosition.position.y - touch.position.y;
				if (Mathf.Abs (differenceX) > 30 || Mathf.Abs (differenceY) > 30) {
					if (Mathf.Abs (differenceX) < Mathf.Abs (differenceY)) {
						//Swipe Down
						if (differenceY > 0) {
							navigationMainMenu.swipeDown (menuPosition);
						}
						//Swipe Up
						if (differenceY < 0) {
							Handheld.Vibrate ();
							if (menuPosition == Vector3.zero) { //MainMenu, change language
								if (language == 0) { //Change language to german
									xmlReader.setLanguage (1);
									PlayerPrefs.SetInt ("Language", 1); 
									language = 1;
									TTSManager.SetLanguage (TTSManager.GERMAN);
									TTSManager.Speak ("Neue Sprache, Deutsch!", false);
								} else { //Change language to english
									xmlReader.setLanguage (0);
									PlayerPrefs.SetInt ("Language", 0); 
									language = 0;
									TTSManager.SetLanguage (TTSManager.ENGLISH);
									TTSManager.Speak ("New Language, English!", false);
								}
							}
							else {
								SceneManager.LoadScene ("MainScene"); //Loads scene "Mainscene"
							}
						}
					}
					if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
						Handheld.Vibrate ();
						//Swipe Right
						if (differenceX < 0 && menuPosition == Vector3.zero) {
							Application.Quit (); //Close App
						}
						//Swipe Left
						if (differenceX > 0 && menuPosition == Vector3.zero) {
							Handheld.Vibrate ();
							SceneManager.LoadScene ("NetworkScene"); //Loads scene "NetworkScene"
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
		
	//Navigates to the CreateLevelMenu
	public void onCreateGameButtonClick() {
		if (swiped == false) {
			menuPosition = navigationMainMenu.navigateTo ("CreateLevelMenu");
			TTSManager.Speak (xmlReader.translate ("MainMenuCreateButton"), false);
		}
	}

	//Navigates to the StartGameMenu
	public void onPlayGameButtonClick() {
		if (swiped == false) {
			menuPosition = navigationMainMenu.navigateTo ("StartGameMenu");
			TTSManager.Speak (xmlReader.translate ("MainMenuPlayButton"), false);
		}
	}

	//Navigates to the Scene "CreateLevelScene" if previous level exists or level 1
	public void onCreateLevelButtonClick(int levelIndex) {
		if (swiped == false) {
            Handheld.Vibrate ();
			PlayerPrefs.SetInt ("LevelNumber", levelIndex); //Sets the levelNumber to the player preferences
			if (levelIndex != 1) {  
				int levelIndexPrevious = levelIndex - 1;
				if (File.Exists (Application.persistentDataPath + "/Level_" + levelIndexPrevious + ".txt")) { //Navigates to the Scene "CreateLevelScene", because previous level exists
					PlayerPrefs.SetInt ("LevelNumber", levelIndex);
					SceneManager.LoadScene("CreateLevelScene");
					TTSManager.Speak (xmlReader.translate ("MainMenuWait"), false);
				} else {
					TTSManager.Speak (xmlReader.translate ("MainMenuCreateLevelButton"), false); //Previous level doesn't exist
				}
			} else {
				SceneManager.LoadScene("CreateLevelScene"); //Navigates to the Scene "CreateLevelScene", because level 1
				TTSManager.Speak (xmlReader.translate ("MainMenuWait"), false);
			}
		}
	}

	//Navigates to the Scene "PlayLevelScene" if level exists and level 1 or previous level successfully completed
	public void onPlayLevelButtonClick(int levelIndex) {
		if (swiped == false) {
			Handheld.Vibrate ();
			if (File.Exists (Application.persistentDataPath + "/Level_" + levelIndex + ".txt")) { //Level exist
				if (levelIndex != 1) {  
					PlayerPrefs.SetInt ("LevelNumber", levelIndex - 1);
					int previousLevelNumber = levelIndex - 1;
					if (File.Exists (Application.persistentDataPath + "/Level_" + previousLevelNumber + ".txt")) {
						Level level = LoadSaveGame.loadLevel ();
						if (level.getLevelCompleted () == true) { //if previous level successfully completed; Navigates to the Scene "PlayLevelScene", because level exists and previous level successfully completed
							PlayerPrefs.SetInt ("LevelNumber", levelIndex);
							StartCoroutine (SceneLoad ());
						} else {
							TTSManager.Speak (xmlReader.translate ("MainMenuPlayLevelButton"), false);
						}
					} else {
						TTSManager.Speak (xmlReader.translate ("MainMenuPlayLevelButton3"), false); //Previous level doesn't exist
					}
				} else { //Navigates to the Scene "PlayLevelScene", because level exists and level 1
					PlayerPrefs.SetInt ("LevelNumber", levelIndex);
					StartCoroutine (SceneLoad ());
				}
			} else {
				TTSManager.Speak (xmlReader.translate ("MainMenuPlayLevelButton2"), false); //Level doesn't exist
			}
		}
	}

	IEnumerator SceneLoad() {
		TTSManager.Speak (xmlReader.translate ("MainMenuWait2"), false);
		yield return new WaitForSeconds (5);
		SceneManager.LoadScene ("PlayLevelScene"); 
	}
}

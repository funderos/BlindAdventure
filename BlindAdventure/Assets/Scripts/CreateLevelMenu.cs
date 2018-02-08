using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

//Author: Stadler Viktor
//This class manages the methods to create a level
public class CreateLevelMenu : MonoBehaviour {

	private Level level;
	private Node node;
	private int levelNumber;
	private Quiz quiz;
	private Fight fight;
	private Opponent opponent;
	private Steeplechase steepleChase;
	private Obstacle obstacle;
	private List <Node> nodeList = new List <Node> ();
	public static int nodeIndex; 

	//Variables to navigate 
	private Vector3 menuPositionLevel;	
	public RectTransform levelContainer;
	public NavigationCreateLevel navigationLevel;

	//Variables for the background musics
	public AudioSource backgroundMusic1;
	public AudioSource backgroundMusic2;
	public AudioSource backgroundMusic3;
	public AudioSource backgroundMusic4;
	public AudioSource backgroundMusic5;
	public AudioSource backgroundMusic6;
	public AudioSource backgroundMusic7;

	//Variables to record the node names
	public Button startNameButton; //Button to start recording the node name
	public Button stopNameButton; //Button to stop recording the node name
	public Voice voice;

	//Variables to record a question
	public Button startQuestionButton; //Button to start recording the question
	public Button stopQuestionButton; //Button to stop recording the question
	public Button exitQuizButton; //Button to exit the quiz-minigame
	private List<KeyValuePair<int,int>> quizList; //List with the question numbers and the appropriate answers

	//Variables to create a steeplechase
	public Button newObstacleButton; //Button for a new obstacle in the steeplechase
	public Button startTimeObstacleButton; //Button to start the time recording 
	public Button stopTimeObstacleButton; //Button to stop the time recording
	private DateTime dateTimeStart1; //Timestamp 1
	private DateTime dateTimeStart2; //Timestamp 2
	private List <Obstacle> obstacleList; //List with the obstacles of a steeplechase
	public Button exitSteeplechaseButton; //Button to exit the steeplechase-minigame

	//Variables to create a fight
	public Button newOpponentButton; //Button for a new opponent in the steeplechase
	public Button startTimeOpponentButton; //Button to start the time recording 
	public Button stopTimeOpponentButton; //Button to stop the time recording
	private List <Opponent> opponentList; //List with the opponents of a fight
	public AudioSource dungeon; //Audio dungeon-scream
	public AudioSource lion; //Audio lion-scream
	public AudioSource witch; //Audio witch-scream
	public Button exitFightButton; //Button to exit the fight-minigame

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true
	public static int direction = 0; //Direction from the swipe: left = 1(a), up = 2(b), right = 3(c), down = 4
	  
	//Variables to init the Scroll Rect in the SelectNodeMenu to change a node
	public GameObject changeNodePrefab;
	public GameObject changeNodeContainer;

	public bool deleteLevelButton = true; //If a level exits, delete level button is operable
	public bool editNodeButton = true; //If a node exits, edit node button is operable
	public static bool saveNodeButton; //If old node was saved 
	public static bool changeNode = false; //New node: false; change node: true (for saving, see line 389)
	public Button yesButton; //Yes Button to delete the level. If user press on the deleteLevelButton the button will be visible
	public Button noButton; //No Button to delete the level. If user press on the deleteLevelButton the button will be visible

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language

	void Awake() {
		StartCoroutine (outputAwake ());
		levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Gets the current levelNumber from the player preferences
		if(File.Exists(Application.persistentDataPath + "/Level_" + levelNumber + ".txt")) {  //If the level exists, it will be loaded. Otherwise a new one will be created.
			level = LoadSaveGame.loadLevel ();
			nodeList = level.getList ();
			deleteLevelButton = true;
			editNodeButton = true;
		}
		else {
			level = new Level(nodeList, levelNumber);
			deleteLevelButton = false;
			editNodeButton = false;
		}

		//Deletes the current position names in the scroll rects
		PlayerPrefs.DeleteKey ("itemName");
		PlayerPrefs.DeleteKey ("backgroundName");
		PlayerPrefs.DeleteKey ("minigameName");
		PlayerPrefs.DeleteKey ("opponentName");
		PlayerPrefs.DeleteKey ("obstacleName");
		yesButton.gameObject.SetActive (false);
		noButton.gameObject.SetActive (false);
		saveNodeButton = true;
	}

	IEnumerator outputAwake() {
		TTSManager.Initialize (transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
		while (!TTSManager.IsInitialized ()) {
			yield return null;
		}
		language = PlayerPrefs.GetInt ("Language");	//Gets the current language from the player preferences; german = 1 or english = 0
		if (language == 1) {
			xmlReader.setLanguage (1);
			TTSManager.SetLanguage (TTSManager.GERMAN);
		}	 else {
			xmlReader.setLanguage (0);
			TTSManager.SetLanguage (TTSManager.ENGLISH);
		}
		TTSManager.Speak ("Level" + levelNumber + xmlReader.translate ("CreateLevelMenuAwake"), false);
	}
		
	void Start(){
		if(File.Exists(Application.persistentDataPath + "/Level_" + levelNumber + ".txt")) {  //Inits the Scroll Rect in the SelectNodeMenu to change a node (if level exists)
			nodeList = level.getList (); 
			for(int i = 0; i < nodeList.Count; i++){
				GameObject container = Instantiate (changeNodePrefab) as GameObject;
				container.transform.SetParent(changeNodeContainer.transform, false);
				int index = i;
				container.GetComponent<Button> ().onClick.AddListener (() => onNodeSelectButtonClick (index));
				container.GetComponent<Button> ().GetComponentInChildren<Text>().text = "Node " + i;
			}
		}
	}

	void Update () {
		levelContainer.anchoredPosition3D = Vector3.Lerp (levelContainer.anchoredPosition3D, menuPositionLevel, 1f); //To navigate to a position

		//Detects swipe direction 
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				startPosition = touch;
			} 
			else if (swiped == false && touch.phase == TouchPhase.Moved) {
				float differenceX = startPosition.position.x - touch.position.x;
				float differenceY = startPosition.position.y - touch.position.y;
				if (Mathf.Abs (differenceX) > 30 || Mathf.Abs (differenceY) > 30) {
					if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
						//Swipe Left
						if (differenceX > 0) {
							navigationLevel.swipeLeft(menuPositionLevel);
						}
						//Swipe Right
						if (differenceX < 0) {
							navigationLevel.swipeRight(menuPositionLevel);
						}
					}
					if (Mathf.Abs (differenceX) < Mathf.Abs (differenceY)) {
						//Swipe Down
						if (differenceY > 0) {
							navigationLevel.swipeDown(menuPositionLevel);
						}
						//Swipe Up
						if (differenceY < 0) {
							menuPositionLevel = navigationLevel.swipeUp (menuPositionLevel);
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

	//Navigates to the EditLevelMenu
	public void onEditLevelButtonClick() {
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("EditLevelMenu");
		}
	}

	//Sets the Yes -and No Button visible
	public void onDeleteLevelButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			if (deleteLevelButton == true) { //If level exists
				yesButton.gameObject.SetActive (true);
				noButton.gameObject.SetActive (true);
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuDeleteLevelButton"), false); //Are you sure you want to delete the level?
			} else {
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuDeleteLevelButton2"), false); //No level available!
			}
		}	
	}
		
	//Navigates to the SelectNodeMenu if node exists
	public void onEditNodeButtonClick (){
		if (swiped == false) {
			if (editNodeButton == true) {
				menuPositionLevel = navigationLevel.navigateTo ("SelectNodeMenu");
			} else {
				Handheld.Vibrate ();
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuEditNodeButton"), false);
			}
		}
	}

	//Creates a new node and navigates to the CreateNodeMenu
	public void onNewNodeButtonClick (){
		if (swiped == false) {
			if (changeNode == false) {
				if (menuPositionLevel == Vector3.left * 10400 && saveNodeButton == true) { //If the minigame isn't saved and the NewNodeButton from the ExitNodeMenu, delete created questions
					int k = 1;
					while(File.Exists (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + k + ".wav")) {
						File.Delete (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + k + ".wav");
						k++;
					}
				}
				saveNodeButton = true;
				nodeList = level.getList ();
				node = new Node ();
				nodeIndex = nodeList.Count;
				if (File.Exists (Application.persistentDataPath + "/Level_" + levelNumber + ".txt")) {  
					TTSManager.Speak (xmlReader.translate ("CreateLevelMenuNewNodeButton") + nodeList.Count + xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
					menuPositionLevel = navigationLevel.navigateTo ("CreateNodeMenu");
				} else {
					menuPositionLevel = navigationLevel.navigateTo ("BackgroundMenu");
				}
			} else { //If the changed node isn't saved
				Handheld.Vibrate ();
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuNewNodeButton3"), false); 
			}
		}
	}

	//Navigates to the ChangeNodeMenu 
	public void onNodeSelectButtonClick (int index){
		if (swiped == false) {
			node = nodeList [index];
			nodeIndex = index;
			menuPositionLevel = navigationLevel.navigateTo ("ChangeNodeMenu");
		}
	}

	//Deletes the properties from the selected node and navigates to the CreateNodeMenu
	public void onChangeNodeButtonClick (){
		if (swiped == false) {
			deleteNodeName(nodeIndex); //Deletes the recorded nodeName
			if (node.getQuiz () != null) { //Deletes the recorded questions
				deleteQuestions (node, nodeIndex);
			}
			node.setQuiz (null); 
			node.setSteeplechase (null);  
			node.setFight (null);  

			menuPositionLevel = navigationLevel.navigateTo ("CreateNodeMenu");
			changeNode = true;
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuChangeNodeButton"), false);
		}
	}

	//Deletes the selected node and navigates to the LevelOptionMenu
	public void onDeleteNodeButtonClick (){
		if (swiped == false) {
			Handheld.Vibrate ();
			deleteNodeName(nodeIndex); //Deletes the recorded nodeName
			if (node.getQuiz () != null) { //Deletes the recorded questions
				deleteQuestions (node, nodeIndex);
			}

			//If it isn't the last node, the questions and node names will be placed one index forward
			if (nodeIndex + 1 != nodeList.Count) {
				for (int i = nodeIndex + 1; i < nodeList.Count; i++) { 
					int k = i - 1;
					if (File.Exists (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + i + ".wav")) {
						File.Move (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + i + ".wav", 
							Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + k + ".wav");
					}
					Node nodeTemp = nodeList [i];
					if (nodeTemp.getQuiz () != null) {
						int questionCount = nodeTemp.getQuiz ().getList ().Count;
						for (int j =0; j <= questionCount; j++) {
							if (File.Exists (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + i + "Question" + j + ".wav")) { 
								File.Move (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + i + "Question" + j + ".wav", 
									Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + k + "Question" + j + ".wav");
							}
						}
					}
				}
			}
			nodeList.Remove (node); //Deletes node

			//If there is no node left, remove whole level. Otherwise the new list with nodes will be saved.
			if (nodeList.Count == 0) { 
				File.Delete (Application.persistentDataPath + "/Level_" + levelNumber + ".txt");
			} else {
				level.setList (nodeList);
				LoadSaveGame.saveLevel (level);
			}
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuDeleteNodeButton"), false);
			SceneManager.LoadScene("CreateLevelScene");
		}
	}

	//Sets the selected background music and navigates to the CreateNodeMenu
	public void onBackgroundSelectButtonClick(int backgroundMusic){
		if (swiped == false) {
			level.setBackgroundMusic (backgroundMusic);
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuNewNodeButton") + nodeList.Count + xmlReader.translate ("CreateLevelMenuNewNodeButton2"), false);
			menuPositionLevel = navigationLevel.navigateTo ("CreateNodeMenu");
		}
	}

	//Plays the selected background music
	public void onBackgroundPlayButtonClick(int backgroundMusic){
		if (swiped == false) {
			Handheld.Vibrate ();
			if (backgroundMusic == 1) {
				backgroundMusic1.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic1));
			} else if (backgroundMusic == 2) {
				backgroundMusic2.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic2));
			} else if (backgroundMusic == 3) {
				backgroundMusic3.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic3));
			} else if (backgroundMusic == 4) {
				backgroundMusic4.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic4));
			} else if (backgroundMusic == 5) {
				backgroundMusic5.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic5));
			} else if (backgroundMusic == 6) {
				backgroundMusic6.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic6));
			} else {
				backgroundMusic7.Play ();
				StartCoroutine (stopBackgroundMusic (backgroundMusic7));
			}
		}
	}

	//Stops the background music after 5 seconds
	IEnumerator stopBackgroundMusic(AudioSource audioSource) {
		yield return new WaitForSeconds (5);
		audioSource.Stop ();
	}

	//Navigates to the NodeNameMenu
	public void onWithMinigameButtonClick() {
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("NodeNameMenu");
		}
	}

	//Navigates to the ItemMenu
	public void onWithoutMinigameButtonClick() {
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("ItemMenu");
		}
	}

	//Sets the selected item and navigates to the ExitNodeMenu
	public void onItemButtonClick(string itemName) {
		if (swiped == false) {
			Item item = new Item(itemName);
			node.setItem (item);
			menuPositionLevel = navigationLevel.navigateTo ("ExitNodeMenu");
		}
	}
		
	//Saves the node
	public void onSaveButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			if (saveNodeButton == true) { //If the node isn't saved
				if (changeNode == false) { //If its a new node, the node will be placed at the end of the list. Otherwise the node will be changed.
					nodeList.Add (node);
				} 
				level.setList (nodeList);
				LoadSaveGame.saveLevel (level);
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuSaveButton"), false);
				changeNode = false;
				saveNodeButton = false;
			} else { //Node is already saved
				TTSManager.Speak (xmlReader.translate ("CreateLevelMenuSaveButton2"), false);
			}
		}
	}

	//Starts with the recording of a node name
	public void onStartNameButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			startNameButton.gameObject.SetActive (false);
			stopNameButton.gameObject.SetActive (true);
			voice.recordBegin ();
		}
	}

	//Stops the recording of a node name, save it and navigates to the MinigameMenu. 
	public void onStopNameButtonClick() {
		if (swiped == false) {
			stopNameButton.gameObject.SetActive (false);
			startNameButton.gameObject.SetActive (true);
			voice.recordEnd ();
			voice.saveVoice (levelNumber, nodeIndex);
			menuPositionLevel = navigationLevel.navigateTo ("MinigameMenu");
		}
	}

	//Starts a new steeplechase and navigates to the SteeplechaseMenu
	public void onSteeplechaseSelectButtonClick(){
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("SteeplechaseMenu");
			steepleChase = new Steeplechase ();
			obstacleList = new List <Obstacle> ();
		}
	}

	//Starts a new fight and navigates to the FightMenu
	public void onFightButtonSelectClick(){
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("FightMenu");
			fight = new Fight ();
			opponentList = new List <Opponent> (); 
		}
	}

	//Starts a new quiz and navigates to the QuizMenu
	public void onQuizButtonSelectClick(){
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("QuizMenu");
			quiz = new Quiz ();
			quizList = new List <KeyValuePair<int,int>> ();
			exitQuizButton.interactable = false;
		}
	}

	//Starts with the recording of a new question
	public void onStartQuestionButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			exitQuizButton.interactable = false;
			startQuestionButton.gameObject.SetActive (false);
			stopQuestionButton.gameObject.SetActive (true);
			voice.recordBegin ();
		}
	}

	//Stops the recording of a question and save it
	public void onStopQuestionButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			stopQuestionButton.gameObject.SetActive (false);
			startQuestionButton.gameObject.SetActive (true);
			stopQuestionButton.interactable = false;
			startQuestionButton.interactable = false;
			voice.recordEnd ();
			quiz.newQuestion ();
			voice.saveVoice (levelNumber, nodeIndex, quiz.getQuestionNumber());
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuStopQuestionButton"), false);
			StartCoroutine("answer");
		}
	}

	//Sets the answer for the recorded question
	IEnumerator answer() {
		direction = 0;
		while (direction == 0) { //wait for swipe
			yield return null;
		}
		Handheld.Vibrate ();
		if (direction == 1) { //left = 1(a)
			quizList.Add (new KeyValuePair<int,int> (quiz.getQuestionNumber(), 1));
		} else if (direction == 2) { //up = 2(b)
			quizList.Add (new KeyValuePair<int,int> (quiz.getQuestionNumber(), 2));
		} else if (direction == 3) { //right = 3(c)
			quizList.Add (new KeyValuePair<int,int> (quiz.getQuestionNumber(), 3));
		} else {
			StartCoroutine("answer"); //down = 4, starts IEnumerator again 
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuStopQuestionButton"), false);
		}
		TTSManager.Speak (xmlReader.translate ("CreateLevelMenuAnswer"), false);
		stopQuestionButton.interactable = true;
		startQuestionButton.interactable = true;
		exitQuizButton.interactable = true;
	}
		
	//Sets the quiz to the node and navigates to the ItemMenu
	public void onExitQuizButtonClick() {
		if (swiped == false) {
			quiz.setList (quizList);
			node.setQuiz (quiz);
			menuPositionLevel = navigationLevel.navigateTo ("ItemMenu");
		}
	}

	//Starts the time recording for an opponent
	public void onStartTimeOpponentButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			startTimeOpponentButton.gameObject.SetActive (false);
			stopTimeOpponentButton.gameObject.SetActive (true);
			opponent = new Opponent ();
			dateTimeStart1 = DateTime.Now; //Timestamp 1
		}
	}

	//Stops the time recording for an opponent and navigates to the OpponentMenu
	public void onStopTimeOpponentButtonClick() {
		if (swiped == false) {
			stopTimeOpponentButton.gameObject.SetActive (false);
			startTimeOpponentButton.gameObject.SetActive (true);
			dateTimeStart2 = DateTime.Now; //Timestamp 2
			TimeSpan timeSpan = dateTimeStart2 - dateTimeStart1; //Difference from timestamp 1 and 2
			int seconds = timeSpan.Seconds;
			if (seconds == 0) { //Minimum 1 second
				seconds = 1;
			}
			opponent.setTime (seconds);
			menuPositionLevel = navigationLevel.navigateTo ("OpponentMenu");
		}
	}

	//Plays the selected opponent scream
	public void onOpponentPlayClick(string opponentTyp){
		if (swiped == false) {
			Handheld.Vibrate ();
			if (opponentTyp == "Dungeon") {
				dungeon.Play ();
			} else if (opponentTyp == "Lion") {
				lion.Play ();
			} else {
				witch.Play ();
			}
		}
	}

	//Sets the selected opponent typ and navigates to the ExitFightMenu
	public void onOpponentSelectClick(string opponentTyp) {
		if (swiped == false) {
			opponent.setTyp (opponentTyp);
			menuPositionLevel = navigationLevel.navigateTo ("ExitFightMenu");
			newOpponentButton.interactable = false;
			exitFightButton.interactable = false;
			StartCoroutine("directionOpponent");
		}
	}

	//Sets the direction for the opponent and add the opponent to the list of opponents 
	IEnumerator directionOpponent() {
		direction = 0;
		while (direction == 0) { //wait for swipe
			yield return null;
		}
		Handheld.Vibrate ();
		if (direction == 1) { //left = 1(a)
			opponent.setDirection (1);
		} else if (direction == 2) { //up = 2(b)
			opponent.setDirection (2);
		} else if (direction == 3) { //right = 3(c)
			opponent.setDirection (3);
		} else {
			StartCoroutine("directionOpponent"); //down = 4, starts IEnumerator again 
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateExitFightMenu"), false);
		}
		opponentList.Add (opponent);
		TTSManager.Speak (xmlReader.translate ("CreateLevelMenuDirectionOpponent"), false);
		newOpponentButton.interactable = true;
		exitFightButton.interactable = true;
	}

	//Navigates to the FightMenu
	public void onNewOpponentButtonClick() {
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("FightMenu");
		}
	}

	//Sets the fight to the node and navigates to the ItemMenu
	public void onExitFightButtonClick() {
		if (swiped == false) {
			fight.setList (opponentList);
			node.setFight (fight);
			menuPositionLevel = navigationLevel.navigateTo ("ItemMenu");
		}
	}

	//Starts the time recording for an obstacle
	public void onStartTimeObstacleButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			startTimeObstacleButton.gameObject.SetActive (false);
			stopTimeObstacleButton.gameObject.SetActive (true);
			obstacle = new Obstacle ();
			dateTimeStart1 = DateTime.Now; //Timestamp 1
		}
	}

	//Stops the time recording for an opponent and navigates to the ObstacleMenu
	public void onStopTimeObstacleButtonClick() {
		if (swiped == false) {
			stopTimeObstacleButton.gameObject.SetActive (false);
			startTimeObstacleButton.gameObject.SetActive (true);
			dateTimeStart2 = DateTime.Now; //Timestamp 2
			TimeSpan timeSpan = dateTimeStart2 - dateTimeStart1; //Difference from timestamp 1 and 2
			int seconds = timeSpan.Seconds;
			if (seconds == 0) { //Minimum 1 second
				seconds = 1;
			}
			obstacle.setTime (seconds);
			menuPositionLevel = navigationLevel.navigateTo ("ObstacleMenu");
		}
	}

	//Sets the selected obstacle typ and navigates to the ExitSteeplechaseMenu
	public void onObstacleSelectClick(string obstacleTyp) {
		if (swiped == false) {
			obstacle.setTyp (obstacleTyp);
			menuPositionLevel = navigationLevel.navigateTo ("ExitSteeplechaseMenu");
			newObstacleButton.interactable = false;
			exitSteeplechaseButton.interactable = false;
			StartCoroutine("directionObstacle");
		}
	}

	//Sets the direction for the obstacle and add the obstacle to the list of obstacles 
	IEnumerator directionObstacle() {
		direction = 0;
		while (direction == 0) { //wait for swipe
			yield return null;
		}
		Handheld.Vibrate ();
		if (direction == 1) { //left = 1(a)
			obstacle.setDirection (1);
		} else if (direction == 2) { //up = 2(b)
			obstacle.setDirection (2);
		} else if (direction == 3) { //right = 3(c)
			obstacle.setDirection (3);
		} else {
			StartCoroutine("directionObstacle"); //down = 4, starts IEnumerator again 
			TTSManager.Speak (xmlReader.translate ("NavigationCreateLevelNavigateExitSteeplechaseMenu"), false);
		}
		obstacleList.Add (obstacle);
		TTSManager.Speak (xmlReader.translate ("CreateLevelMenuDirectionObstacle"), false);
		newObstacleButton.interactable = true;
		exitSteeplechaseButton.interactable = true;
	}

	//Navigates to the SteeplechaseMenu
	public void onNewObstacleButtonClick() {
		if (swiped == false) {
			menuPositionLevel = navigationLevel.navigateTo ("SteeplechaseMenu");
		}
	}

	//Sets the steeplechase to the node and navigates to the ItemMenu
	public void onExitSteeplechaseButtonClick() {
		if (swiped == false) {
			steepleChase.setList (obstacleList);
			node.setSteeplechase (steepleChase);
			menuPositionLevel = navigationLevel.navigateTo ("ItemMenu");
		}
	}

	//Sets the Yes and No Button invisible and deletes the complete Level 
	public void onYesButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			yesButton.gameObject.SetActive (false);
			noButton.gameObject.SetActive (false);
			for (int i = 0; i <= nodeList.Count; i++) { //Delete node names
				deleteNodeName (i);
			}
			for (int j = 0; j < nodeList.Count; j++) { //Delete questions
				Node node = nodeList [j];
				if (node.getQuiz () != null) {
					deleteQuestions (node, j);
				}
			}
			if (File.Exists (Application.persistentDataPath + "/Level_" + levelNumber + ".txt")) { 
				File.Delete (Application.persistentDataPath + "/Level_" + levelNumber + ".txt"); //Deletes level
			}
			deleteLevelButton = false;
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuYesButton"), false);
			SceneManager.LoadScene("CreateLevelScene"); 
		}
	}

	//Sets the Yes and No Button invisible
	public void onNoButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			yesButton.gameObject.SetActive (false);
			noButton.gameObject.SetActive (false);
			TTSManager.Speak (xmlReader.translate ("CreateLevelMenuNoButton"), false);
		}
	}

	//Deletes all questions of the node
	public void deleteQuestions(Node node, int index){
		int questionCount = node.getQuiz ().getList ().Count; 
		for (int k = 0; k <= questionCount; k++) {  
			if (File.Exists (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + index + "Question" + k + ".wav")) { 
				File.Delete (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + index + "Question" + k + ".wav");
			}
		}
	}

	//Deletes the recorded name of the node 
	public void deleteNodeName(int index){
		if (File.Exists (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + index + ".wav")) { 
			File.Delete (Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + index + ".wav");
		}
	}
}

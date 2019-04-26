using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//Author: Stadler Viktor
//This class manages the methods to play a level in the Network-Mode
public class PlayLevelMenuNetwork : MonoBehaviour {

	private int levelNumber;
	private Node node;
	private List <Node> nodeList = new List <Node> ();
	private Level level;
	private int nodeIndex = 0;
	public AudioSource source;
	public AudioClip clip;
	public AudioSource applause;
	public AudioSource walk;

	//Variables to navigate 
	public NavigationPlayLevelNetwork navigationPlay;
	public RectTransform playContainer;
	private Vector3 menuPosition;	

	//Variables to play a quiz
	private List<KeyValuePair<int,int>> quizList; //List with the question numbers and the appropriate answers
	private int currentQuestion; //Current question number

	//Variables to play a steeplechase
	private List <Obstacle> obstacleList; //List with the obstacles of a fight
	private int obstacleIndex; //Index of the obstacle

	//Variables to play a fight
	private List <Opponent> opponentList; //List with the opponents of a fight
	private int opponentIndex; //Index of the opponent
	private int directionOpponent; //Direction where the opponent appears
	public AudioSource dungeonScream; //Audio dungeon-scream
	public AudioSource lionScream; //Audio lion-scream
	public AudioSource witchScream; //Audio witch-scream

	//Variables for the background musics
	public AudioSource backgroundMusic1;
	public AudioSource backgroundMusic2;
	public AudioSource backgroundMusic3;
	public AudioSource backgroundMusic4;
	public AudioSource backgroundMusic5;
	public AudioSource backgroundMusic6;
	public AudioSource backgroundMusic7;

	private List <Item> collectedItems = new List <Item> (); //List of collected items
	private List <Item> necessaryItems = new List <Item> (); //List of necessary items
	public Button playNodeButton; //Button to play a node
	public Button restartMinigameButton; //Button to restart a minigame
	private string searchname;

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true
	public static int direction = 0; //Direction from the swipe: left = 1(a), up = 2(b), right = 3(c), down = 4

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language



	void Awake() {
		StartCoroutine (intro ());
		playNodeButton.interactable = false;
		int i = 1;
		PlayerPrefs.SetInt ("FromPlayLevelMenuNetwork", i);
	}

	IEnumerator intro() {
		AudioClip audioClip = Resources.Load ("Intro") as AudioClip; //Plays intro melody
		source = GetComponent<AudioSource> ();
		source.PlayOneShot (audioClip);
		yield return new WaitForSeconds (9);
		StartCoroutine (InitTTS ());
	}

	IEnumerator InitTTS() {
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
		playLevel ();
	}
		
	void Update () {
		playContainer.anchoredPosition3D = Vector3.Lerp (playContainer.anchoredPosition3D, menuPosition, 1f); //To navigate to a position

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
							int richtungBekommen = navigationPlay.swipeLeft (menuPosition);
							if (richtungBekommen == 1)
								direction = 1; //Sets the direction for an answer, opponent or obstacle
							if (richtungBekommen == 11)
								nextNode (11); //walk to previous node
						}
						//Swipe Right
						if (differenceX < 0) {
							int richtungBekommen = navigationPlay.swipeRight (menuPosition);
							if (richtungBekommen == 3)
								direction = 3; //Sets the direction for an answer, opponent or obstacle
							if (richtungBekommen == 33)
								nextNode (33); //walk to next node
						}
					}
					if (Mathf.Abs (differenceX) < Mathf.Abs (differenceY)) {
						//Swipe Down
						if (differenceY > 0) {
							navigationPlay.swipeDown (menuPosition);
						}
						//Swipe Up
						if (differenceY < 0) {
							navigationPlay.swipeUp (menuPosition);
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
		
	//Starts the background music and initializes the necessary and collected items lists
	public void playLevel() {
		levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Gets the current levelNumber from the player preferences
		searchname = PlayerPrefs.GetString ("NameGame");
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayLevel") + levelNumber, true);
		if(File.Exists(Application.persistentDataPath + "/" + searchname + "/Level_" + levelNumber + ".txt")) {  
				level = LoadSaveGameNetwork.loadLevel (); //Load level
				int backgroundMusicIndex = level.getBackgroundMusic ();
				if (backgroundMusicIndex == 1)
					backgroundMusic1.Play ();
				else if (backgroundMusicIndex == 2)
					backgroundMusic2.Play ();
				else if (backgroundMusicIndex == 3)
					backgroundMusic3.Play ();
				else if (backgroundMusicIndex == 4)
					backgroundMusic4.Play ();
				else if (backgroundMusicIndex == 5)
					backgroundMusic5.Play ();
				else if (backgroundMusicIndex == 6)
					backgroundMusic6.Play ();
				else
					backgroundMusic7.Play ();
			
				nodeList = level.getList ();

			for (int i = 0; i < nodeList.Count; i++) { //Which items are necessary to complete the level
				node = nodeList [i];
				necessaryItems.Add (node.getItem ());
			}
				
			for (int i = 0; i < nodeList.Count; i++) { //Which items are already collected
				node = nodeList [i];
				if (node.getNodeSolved() == true) {
					collectedItems.Add (node.getItem ());
				}
			}
			playNode ();
		}
	}
		
	//Starts playing a node if its not the end of the level
	public void playNode() {
		playNodeButton.interactable = false;
		if (nodeIndex == nodeList.Count) { //End of the level
			
			List <Item> necessaryItemsTemp = new List<Item> (necessaryItems);
			List <Item> collectedItemsTemp = new List<Item> (collectedItems);
			for (int i = 0; i < collectedItemsTemp.Count; i++) { //Looks if all items are collected
				Item necessaryItem = collectedItemsTemp [i];
				for (int j = 0; j < necessaryItemsTemp.Count; j++) {
					Item collectedItem = necessaryItemsTemp [j];
					if (collectedItem == necessaryItem) {
						necessaryItemsTemp.Remove (collectedItem);
						break;
					}
				}
			}
			if (necessaryItemsTemp.Count == 0) { //When all items are collected
				StartCoroutine (finishedLevel ());
			} else {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeLevelEnd"), false); //When items left -> audio output which ones missing
				for (int i = 0; i < necessaryItemsTemp.Count; i++)
					TTSManager.Speak (necessaryItemsTemp [i].getName (), true);
			}
		} else {
			node = nodeList [nodeIndex];
			if (node.getQuiz () != null || node.getFight () != null || node.getSteeplechase () != null) { //If its a node with a minigame
				if (File.Exists (Application.persistentDataPath + "/" + searchname + "/Level" + levelNumber + "NodeNumber" + nodeIndex + ".wav")) { 
					playNodeButton.interactable = true;
					StartCoroutine ("loadNodeName");
				}
			} else { //Only a node with an item -> collect it
				playNodeButton.interactable = false;
				if (node.getNodeSolved() == false) {
					Item item = node.getItem ();
					collectedItems.Add (item);
					if (((item.getName () == "Candle" || item.getName () == "Clock") && language == 1) || item.getName () == "Umbrella" && language == 0) { //Article "eine" or "an" 
						TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect") + itemTranslation(item.getName ()) + xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect3"), true);
					} else { //Article "ein" or "a"
						TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect2") + itemTranslation(item.getName ()) + xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect3"), true);
					}
					node.setNodeSolved(true);
					LoadSaveGameNetwork.saveLevel(level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeAgain"), true); 
				}
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeWalk"), true);
			}
		}
	}
		
	//Outputs the current node name
	IEnumerator loadNodeName(){
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect2"), true);
		while(TTSManager.IsSpeaking()){
			yield return null;
		}
		WWW wav = new WWW ("file://" + Application.persistentDataPath + "/" + searchname + "/Level" + levelNumber + "NodeNumber" + nodeIndex + ".wav"); 
		yield return wav;
		source.clip = wav.GetAudioClip(false);
		source.Play ();
		while(source.isPlaying){
			yield return null;
		}
		if (node.getNodeSolved() == false) { //If node is not solved
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuLoadNodeName"), false);
		} else { //Node solved
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuLoadNodeName2"), false);
		}
	}

	//Outputs an applause melody and saves the level as completed
	IEnumerator finishedLevel(){
		level.setLevelCompleted (true);
		LoadSaveGameNetwork.saveLevel(level);
		applause.Play ();
		while(applause.isPlaying){
			yield return null;
		}
		int nextLevelNumber = levelNumber + 1;
		if (!File.Exists (Application.persistentDataPath + "/" + searchname + "/Level_" + nextLevelNumber + ".txt")) { //If its the last level -> output: game completed
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFinishedLevelNetwork"), true);
		} else {
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFinishedLevelNetwork2"), true); //Another level exist -> level completed
		}
	}

	//Starts the minigame of the node and navigates to the PlayNodeMenu
	public void onPlayNodeButtonClick(){
		if (swiped == false) {
			menuPosition = navigationPlay.navigateTo ("PlayNodeMenu");
			restartMinigameButton.interactable = false;
			if (node.getQuiz () != null) { //minigame is a quiz
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeButton"), true);
				Quiz quiz = node.getQuiz ();
				quizList = quiz.getList ();
				currentQuestion = 1;
				StartCoroutine (question());
			} else if (node.getFight () != null) { //minigame is a fight
				StartCoroutine (waitToFight ());
			} else {
				StartCoroutine (waitToSteeplechase ()); //minigame is a steeplechase
			}
		}
	}

	//Outputs all collected items
	public void onInventoryButtonClick(){
		if (swiped == false) {
			Handheld.Vibrate ();
			if (collectedItems.Count != 0) {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuInventoryButton"), false);
				for (int i = 0; i < collectedItems.Count; i++)
					TTSManager.Speak (itemTranslation(collectedItems [i].getName ()), true);
			} else {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuInventoryButton2"), false); //No items collected
			}
		}
	}
		
	//Restarts the minigame of the node
	public void onRestartMinigameButtonClick(){
		if (swiped == false) {
			Handheld.Vibrate ();
			restartMinigameButton.interactable = false;
			if (node.getQuiz () != null) { //minigame is a quiz
				currentQuestion = 1;
				StartCoroutine (question());
			} else if (node.getFight () != null) { //minigame is a fight
				opponentIndex = 0;
				StartCoroutine (waitToOpponent (opponentList [opponentIndex].getTime ()));
			} else {
				obstacleIndex = 0; //minigame is a steeplechase
				StartCoroutine (waitToObstacle (obstacleList [obstacleIndex].getTime ()));
			}
		}
	}

	//Navigates to the NodeMenu
	public void onExitNodeButtonClick(){
		if (swiped == false) {
			menuPosition = navigationPlay.navigateTo ("NodeMenu");
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuExitNodeButton"), false);
		}
	}

	//Outputs that the starting minigame is a fight 
	IEnumerator waitToFight() {
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToFight"), true);
		while(TTSManager.IsSpeaking()){
			yield return null;
		}
		Fight fight = node.getFight ();
		opponentList = fight.getList ();
		opponentIndex = 0;
		StartCoroutine (waitToOpponent (opponentList [opponentIndex].getTime ()));
	}
		
	//Waits until the next opponent appears and outputs a scream from the direction left, right or forward 
	IEnumerator waitToOpponent(int secondsToWait) {
		yield return new WaitForSeconds (secondsToWait);
		string opponentTyp = opponentList [opponentIndex].getTyp ();
		directionOpponent = opponentList [opponentIndex].getDirection ();
		if (opponentTyp == "Dungeon") {
			if (directionOpponent == 1) {
				dungeonScream.panStereo = -1; //Dungeon-scream from left
			} else if (directionOpponent == 3) {
				dungeonScream.panStereo = 1; //Dungeon-scream from right
			} else {
				dungeonScream.panStereo = 0 ; //Dungeon-scream from forward
			}
			dungeonScream.Play ();
		} else if (opponentTyp == "Lion") {
			if (directionOpponent == 1) {
				lionScream.panStereo = -1; //Lion-scream from left
			} else if (directionOpponent == 3) {
				lionScream.panStereo = 1; //Lion-scream from right
			} else {
				lionScream.panStereo = 0 ; //Lion-scream from forward
			}
			lionScream.Play ();
		} else {
			if (directionOpponent == 1) {
				witchScream.panStereo = -1; //Witch-scream from left
			} else if (directionOpponent == 3) {
				witchScream.panStereo = 1; //Witch-scream from right
			} else {
				witchScream.panStereo = 0 ; //Witch-scream from forward
			}
			witchScream.Play ();
		}
		StartCoroutine (fightOpponent (opponentTyp));
	}

	//Waits for swipe and checks if the opponent is defeated
	IEnumerator fightOpponent(string opponentTyp) {
		direction = 0;
		yield return new WaitForSeconds (2); //2 seconds time to react on the scream

		if (directionOpponent == direction) { //opponent defeated
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent"), true);
			opponentIndex++; //Next opponent if exists
			if (opponentIndex < opponentList.Count) { //Next opponent
				StartCoroutine (waitToOpponent (opponentList [opponentIndex].getTime ()));
			} else { //Fight won
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent2"), true);
				if (node.getNodeSolved () == false) { //Node solved for the first time
					Item item = node.getItem ();
					collectedItems.Add (item); //New item collected
					itemOutput(item);
					node.setNodeSolved (true);
					LoadSaveGameNetwork.saveLevel (level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), true); //Item already collected
				}
				restartMinigameButton.interactable = true; //Fight won -> Restart if you want? 
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), true);
			}
		} else { //Fight lost -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent8") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), true);
		}
	}
		
	//Outputs that the starting minigame is a steeplechase 
	IEnumerator waitToSteeplechase() {
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToSteeplechase"), true);
		while(TTSManager.IsSpeaking()){
			yield return null;
		}
		Steeplechase steepleChase = node.getSteeplechase ();
		obstacleList = steepleChase.getList ();
		obstacleIndex = 0;
		StartCoroutine (waitToObstacle (obstacleList [obstacleIndex].getTime ()));
	}
		
	//Waits until the next obstacle appears and outputs where the obstacle come from: left, right or forward 
	IEnumerator waitToObstacle(int secondsToWait) {
		yield return new WaitForSeconds (secondsToWait);
		string obstacleTyp = obstacleList [obstacleIndex].getTyp ();
		int directionObstacle = obstacleList [obstacleIndex].getDirection ();
		if (directionObstacle == 1) {  //Obstacle from left
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToObstacle") + obstacleTranslation(obstacleTyp), true);
		}
		else if (directionObstacle == 3){ //Obstacle from right
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToObstacle2") + obstacleTranslation(obstacleTyp), true);
		}
		else{ //Obstacle from forward
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToObstacle3") + obstacleTranslation(obstacleTyp), true);
		}
		StartCoroutine (avoidObstacle (directionObstacle));
	}
		
	//Waits for swipe and checks if the obstacle is evaded
	IEnumerator avoidObstacle(int directionObstacle) {
		direction = 0;
		yield return new WaitForSeconds (3); //3 seconds time to react on the warning
		if (direction == directionObstacle || (directionObstacle == 2 && direction == 0)) { //Steeplechase lost -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), true);
		} else { //Obstacle evaded
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle2"), true);
			obstacleIndex++; //Next obstacle if exists
			if (obstacleIndex < obstacleList.Count) { //Next obstacle
				StartCoroutine (waitToObstacle (obstacleList [obstacleIndex].getTime ()));
			} else {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle3"), true);
				if (node.getNodeSolved() == false) { //Node solved for the first time
					Item item = node.getItem ();
					collectedItems.Add (item);
					itemOutput(item);
					node.setNodeSolved(true);
					LoadSaveGameNetwork.saveLevel(level);
				}
				else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), true); //Item already collected
				}
				restartMinigameButton.interactable = true;
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), true); //Steeplechase won -> Restart if you want?
			}
		}
	}

	//Outputs the question
	IEnumerator question() {
		if(File.Exists(Application.persistentDataPath + "/" + searchname + "/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + currentQuestion + ".wav")) {  
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuQuestion") + currentQuestion, true);
			while(TTSManager.IsSpeaking()){
				yield return null;
			}
			WWW wav = new WWW("file://" + Application.persistentDataPath + "/" + searchname + "/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + currentQuestion + ".wav"); 
			yield return wav;
			source.clip = wav.GetAudioClip(false);
			source.Play ();
			while(source.isPlaying){
				yield return null;
			}
			StartCoroutine (solutionQuestion ());
		}
	}

	//Waits for swipe and checks if the answer is correct
	IEnumerator solutionQuestion() {
		direction = 0;
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion"), false);
		while(TTSManager.IsSpeaking()){
			yield return null;
		}
		int solution = quizList [currentQuestion-1].Value;
		while (direction == 0) {
			yield return null;
		}
		if (direction == solution) { //Answer is correct
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion2"), false);
			currentQuestion++; //Next question if exists
			if (currentQuestion <= quizList.Count) { //Next question
				StartCoroutine (question());
			} else { //Quiz successfully completed
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion3"), true);
				if (node.getNodeSolved() == false) { //Node solved for the first time
					Item item = node.getItem ();
					collectedItems.Add (item); //New item collected
					itemOutput(item);
					node.setNodeSolved (true);
					LoadSaveGameNetwork.saveLevel(level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), true); //Item already collected
				}
				restartMinigameButton.interactable = true;
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), true); //Quiz successfully completed -> Restart if you want?
			}
		} 
		else { //Quiz not successfully completed -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion4") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), true);
		}
	}

	//Increase or decrease the nodeIndex
	public void nextNode(int direction) {
		if (direction == 33) { //Increase the nodeIndex
			if (nodeIndex < nodeList.Count) {
				nodeIndex++;
				StartCoroutine (walkToNode ());
			} 
		} 
		if (direction == 11) { //Decrease the nodeIndex if isn't zero
			if (nodeIndex != 0) {
				nodeIndex--;
				StartCoroutine (walkToNode ());
			} else {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuNextNode"), false); //NodeIndex is zero, play the current node
				playNode ();
			}
		}
	}

	//Outputs the walk melody
	IEnumerator walkToNode() {
		walk.Play ();
		while(walk.isPlaying){
			yield return null;
		}
		playNode (); 
	}

	//Outputs when an item is collected, depends on the correct article (a,an,eine,ein) from the item
	public void itemOutput(Item item){
		if (((item.getName () == "Candle" || item.getName () == "Clock") && language == 1) || item.getName () == "Umbrella" && language == 0) { //Article "eine" or "an"
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent3") + itemTranslation (item.getName ()), true);
		} else if ((item.getName () == "Rope" || item.getName () == "Book" || item.getName () == "Knife") && language == 1){ //Article "ein"
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent5") + itemTranslation (item.getName ()), true);
		} else {
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent4") + itemTranslation (item.getName ()), true); //Article "einen" or "a"
		} 
	}

	//Translation for the itemNames
	public string itemTranslation(string itemName){
		if (language == 1) { //german
			if(itemName == "Book") itemName = "Buch";
			else if(itemName == "Candle") itemName = "Kerze";
			else if(itemName == "Clock") itemName = "Uhr";
			else if(itemName == "Knife") itemName = "Messer";
			else if(itemName == "Compass") itemName = "Kompass";
			else if(itemName == "Umbrella") itemName = "Regenschirm";
			else if(itemName == "Rope") itemName = "Seil";
		}
		return itemName;
	}

	//Translation for the obstacleNames
	public string obstacleTranslation(string obstacleTyp){
		if (language == 1) { //german
			if(obstacleTyp == "TreeStump") obstacleTyp = "Baumstumpf";
			else if(obstacleTyp == "Cliff") obstacleTyp = "Klippe";
			else if(obstacleTyp == "Trap") obstacleTyp = "Falle";
			else if(obstacleTyp == "River") obstacleTyp = "Fluss";
			else if(obstacleTyp == "Rock") obstacleTyp = "Stein";
			else if(obstacleTyp == "Wall") obstacleTyp = "Mauer";
		}
		return obstacleTyp;
	}
}

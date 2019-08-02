using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

//Author: Stadler Viktor
//This class manages the methods to play a level
public class PlayLevelMenu : MonoBehaviour {

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
	public NavigationPlayLevel navigationPlay;
	public RectTransform playContainer;
	private Vector3 menuPosition;	

	//Variables to play a quiz
	private List<KeyValuePair<int,int>> quizList; //List with the question numbers and the appropriate answers
	private int currentQuestion; //Current question number

    //Variables to play a steeplechase
    private List<KeyValuePair<int, Obstacle>> obstacleList; //List with the obstacles of a fight
    private int obstacleIndex; //Index of the obstacle

    //Variables to play a fight
    private List<KeyValuePair<int, int>> opponentList; //List with the opponents of a fight
	private int opponentIndex; //Index of the opponent
	private int directionOpponent; //Direction where the opponent appears

	//Variables for the background musics
	public AudioSource backgroundMusic1;
	public AudioSource backgroundMusic2;
	public AudioSource backgroundMusic3;
	public AudioSource backgroundMusic4;
	public AudioSource backgroundMusic5;
	public AudioSource backgroundMusic6;
	public AudioSource backgroundMusic7;

	public Button playNodeButton; //Button to play a node
	public Button restartMinigameButton; //Button to restart a minigame

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
	}

	IEnumerator intro() {
		//AudioClip audioClip = Resources.Load ("Intro") as AudioClip; //Plays intro melody
		source = GetComponent<AudioSource> ();
		//source.PlayOneShot (audioClip);
		yield return new WaitForSeconds (1);
		StartCoroutine (InitTTS ());
	}

	IEnumerator InitTTS() {
		//TTSManager.Initialize (transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
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
		
	//Starts the background music
	public void playLevel() {
		levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Gets the current levelNumber from the player preferences
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayLevel") + levelNumber, false);
        while (TTSManager.IsSpeaking()) {}
        if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level_" + levelNumber + ".txt")) {  
				level = LoadSaveGame.loadLevel (); //Load level
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

			playNode ();
		}
	}
		
	//Starts playing a node if its not the end of the level
	public void playNode() {
		playNodeButton.interactable = false;
		if (nodeIndex == nodeList.Count) { //End of the level
            bool allItemsCollected = true;

            for (int i = 0; i < nodeList.Count; i++) //Looks if all items are collected
            {
                node = nodeList[i];
                if (!node.getNodeSolved())
                {
                    if(allItemsCollected)
                    {
                        TTSManager.Speak(xmlReader.translate("PlayLevelMenuPlayNodeLevelEnd"), false); //When items left -> audio output which ones missing
                        allItemsCollected = false;
                    }
                    if(File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + i + "Item" + ".wav"))
                    {
                        StartCoroutine(playAudioItem(i, false));
                        break;
                    }
                }
            }

            if(allItemsCollected)
                StartCoroutine(finishedLevel());

        } else {
			node = nodeList [nodeIndex];
			if (node.getQuiz () != null || node.getFight () != null || node.getSteeplechase () != null) { //If its a node with a minigame
				if (File.Exists (Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + ".wav")) { 
					playNodeButton.interactable = true;
					StartCoroutine ("loadNodeName");
				}
			} else { //Only a node with an item -> collect it
				playNodeButton.interactable = false;
				if (!node.getNodeSolved() && File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Item" + ".wav")) {
                    StartCoroutine(itemOutputWithoutMinigame());
                    node.setNodeSolved(true);
					LoadSaveGame.saveLevel(level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeAgain"), false); 
				}
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeWalk"), false);
			}
		}
	}
		
	//Outputs the current node name
	IEnumerator loadNodeName(){
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeItemCollect2"), false);
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }
        WWW wav = new WWW("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + ".wav");
        yield return wav;
        source.clip = wav.GetAudioClip(false);
        source.Play();
        while (source.isPlaying)
        {
            yield return null;
        }
        if (node.getNodeSolved() == false) { //If node is not solved
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuLoadNodeName"), false);
		} else { //Node solved
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuLoadNodeName2"), false);
		}
	}

    //Outputs persisent Audio File
    IEnumerator playAudio(string path)
    {
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }
        WWW wav = new WWW(path);
        yield return wav;
        source.clip = wav.GetAudioClip(false);
        source.Play();
        while (source.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator playAudioItem(int number, bool type)
    {
        node = nodeList[number];
        if (node.getNodeSolved() == type)
        {
            while (TTSManager.IsSpeaking())
            {
                yield return null;
            }
            UnityWebRequest wav = UnityWebRequestMultimedia.GetAudioClip(
                "file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + number + "Item" + ".wav", AudioType.WAV);
            yield return wav.SendWebRequest();
            if (wav.isNetworkError)
            {
                Debug.Log(wav.isNetworkError);
            }
            else
            {
                source.clip = DownloadHandlerAudioClip.GetContent(wav);
            }
            source.Play();
            while (source.isPlaying)
            {
                yield return null;
            }
        }
        if (number < nodeList.Count - 1)
            yield return StartCoroutine(playAudioItem(++number, type));
    }

    IEnumerator playAudioOpponents(int ind, bool scream)
    {
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }
        string fname = "file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Opponent" + (ind + 1).ToString();
        UnityWebRequest wav;
        if (scream)
        {
            wav = UnityWebRequestMultimedia.GetAudioClip(fname + "Scream.wav", AudioType.WAV);
            TTSManager.Speak(xmlReader.translate("PlayLevelMenuOpponentScreamIntroduction"), false);
        }    
        else
        {
            wav = UnityWebRequestMultimedia.GetAudioClip(fname + "Name.wav", AudioType.WAV);
            TTSManager.Speak(xmlReader.translate("PlayLevelMenuOpponentNameIntroduction"), false);
        }
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }

        yield return wav.SendWebRequest();
        if (wav.isNetworkError)
        {
            Debug.Log(wav.error);
        }
        else
        {
            source.clip = DownloadHandlerAudioClip.GetContent(wav);
        }
        source.Play();
        while (source.isPlaying)
        {
            yield return null;
        }
        if (scream)
        {
            ind++;
            if (ind < opponentList.Count)
                yield return StartCoroutine(playAudioOpponents(ind, false));
            else
                StartCoroutine(waitToRepeat());
        }
        else
        {
            TTSManager.Speak(xmlReader.translate("PlayLevelMenuOpponentDirection" + opponentList[ind].Value.ToString()), false);
            yield return StartCoroutine(playAudioOpponents(ind, true));
        }
    }

    //Outputs an applause melody and saves the level as completed
    IEnumerator finishedLevel(){
		level.setLevelCompleted (true);
		LoadSaveGame.saveLevel(level);
		applause.Play ();
		while(applause.isPlaying){
			yield return null;
		}
		int nextLevelNumber = levelNumber + 1;
		if (!File.Exists (Application.persistentDataPath + "/CurrentGame/Level_" + nextLevelNumber + ".txt")) { //If its the last level -> output: game completed
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFinishedLevel"), false);
		} else {
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFinishedLevel2"), false); //Another level exist -> level completed
		}
	}

	//Starts the minigame of the node and navigates to the PlayNodeMenu
	public void onPlayNodeButtonClick(){
		if (swiped == false) {
			menuPosition = navigationPlay.navigateTo ("PlayNodeMenu");
			restartMinigameButton.interactable = false;
			if (node.getQuiz () != null) { //minigame is a quiz
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuPlayNodeButton"), false);
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
            bool anyItemsCollected = false;
            for (int i = 0; i < nodeList.Count; i++) //Looks if all items are collected
            {
                node = nodeList[i];
                if (node.getNodeSolved())
                {
                    if (!anyItemsCollected)
                    {
                        TTSManager.Speak(xmlReader.translate("PlayLevelMenuInventoryButton"), false);
                        anyItemsCollected = true;
                    }

                    if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + i + "Item" + ".wav"))
                    {
                        StartCoroutine(playAudioItem(i, true));
                        break;
                    }
                }
            }
            if (!anyItemsCollected) {
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
				StartCoroutine (waitToFight());
			} else {
				obstacleIndex = 0; //minigame is a steeplechase
                StartCoroutine(waitToObstacle(obstacleList[obstacleIndex].Value.getTime()));
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
		TTSManager.Speak (xmlReader.translate ("PlayLevelMenuWaitToFight"), false);
		while(TTSManager.IsSpeaking()){
			yield return null;
		}
		Fight fight = node.getFight ();
		opponentList = fight.getList ();
        opponentIndex = 0;
        StartCoroutine(playAudioOpponents(0, false));
	}

    //Asks if Player wants to repeat the opponent introduction 
    IEnumerator waitToRepeat()
    {
        direction = 0;
        TTSManager.Speak(xmlReader.translate("PlayLevelMenuRepeatOpponentIntroduction"), false);
        while (direction == 0 || direction == 2 || direction == 4)
        {
            yield return null;
        }
        if (direction == 1)
            StartCoroutine(playAudioOpponents(0, false));
        if (direction == 3)
            StartCoroutine(waitToOpponent());
    }

    //Waits until the next opponent appears and outputs a scream from the direction left, right or forward 
    IEnumerator waitToOpponent() {
		yield return new WaitForSeconds (2);
        int index = Random.Range(0, opponentList.Count);
        StartCoroutine(playAudio("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Opponent" + (index + 1).ToString() + "Scream.wav"));
		StartCoroutine (fightOpponent (index));
	}

	//Waits for swipe and checks if the opponent is defeated
	IEnumerator fightOpponent(int index) {
		direction = 0;
		yield return new WaitForSeconds (5); //5 seconds time to react on the scream

		if (opponentList[index].Value == direction) { //opponent defeated
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent"), false);
			opponentIndex++; //Next opponent if exists
			if (opponentIndex < node.getFight().getOpponentCount()) { //Next opponent
				StartCoroutine (waitToOpponent ());
			} else { //Fight won
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent2"), false);
				if (node.getNodeSolved () == false) { //Node solved for the first time
                    yield return StartCoroutine(itemOutput());
                    node.setNodeSolved (true);
					LoadSaveGame.saveLevel (level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), false); //Item already collected
				}
				restartMinigameButton.interactable = true; //Fight won -> Restart if you want? 
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), false);
			}
		} else { //Fight lost -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent8") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), false);
		}
	}
		
	//Outputs that the starting minigame is a steeplechase 
	IEnumerator waitToSteeplechase() {
        TTSManager.Speak(xmlReader.translate("PlayLevelMenuWaitToSteeplechase"), false);
        while (TTSManager.IsSpeaking())
        {
            yield return null;
        }
        Steeplechase steepleChase = node.getSteeplechase();
        obstacleList = steepleChase.getList();
        obstacleIndex = 0;
        StartCoroutine(waitToObstacle(obstacleList[obstacleIndex].Value.getTime()));
    }
		
	//Waits until the next obstacle appears and outputs where the obstacle come from: left, right or forward 
	IEnumerator waitToObstacle(int secondsToWait) {
        yield return new WaitForSeconds(secondsToWait);
        int directionObstacle = obstacleList[obstacleIndex].Value.getDirection();
        int obstacleNumber = obstacleIndex + 1;
        if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Obstacle" + obstacleNumber + ".wav"))
        {
            WWW wav = new WWW("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Obstacle" + obstacleNumber + ".wav");
            yield return wav;
            source.clip = wav.GetAudioClip(false);
            source.Play();
            while (source.isPlaying)
            {
                yield return null;
            }

            if (directionObstacle == 1)
            {  //Obstacle from left
                TTSManager.Speak(xmlReader.translate("PlayLevelMenuWaitToObstacle"), false);
            }
            else if (directionObstacle == 3)
            { //Obstacle from right
                TTSManager.Speak(xmlReader.translate("PlayLevelMenuWaitToObstacle2"), false);
            }
            else
            { //Obstacle from forward
                TTSManager.Speak(xmlReader.translate("PlayLevelMenuWaitToObstacle3"), false);
            }
            while (TTSManager.IsSpeaking())
            {
                yield return null;
            }
            StartCoroutine(avoidObstacle(directionObstacle));
        }
    }
		
	//Waits for swipe and checks if the obstacle is evaded
	IEnumerator avoidObstacle(int directionObstacle) {
		direction = 0;
		yield return new WaitForSeconds (3); //3 seconds time to react on the warning
		if (direction == directionObstacle || (directionObstacle == 2 && direction == 0)) { //Steeplechase lost -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), true);
		} else { //Obstacle evaded
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle2"), false);
			obstacleIndex++; //Next obstacle if exists
			if (obstacleIndex < obstacleList.Count) { //Next obstacle
                StartCoroutine(waitToObstacle(obstacleList[obstacleIndex].Value.getTime()));
            } else {
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuAvoidObstacle3"), false);
				if (node.getNodeSolved() == false) { //Node solved for the first time
                    yield return StartCoroutine(itemOutput());
                    node.setNodeSolved(true);
					LoadSaveGame.saveLevel(level);
				}
				else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), false); //Item already collected
				}
				restartMinigameButton.interactable = true;
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), false); //Steeplechase won -> Restart if you want?
			}
		}
	}

	//Outputs the question
	IEnumerator question() {
		if(File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + currentQuestion + ".wav")) {  
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuQuestion") + currentQuestion, false);
			while(TTSManager.IsSpeaking()){
				yield return null;
			}
			WWW wav = new WWW("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Question" + currentQuestion + ".wav"); 
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
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion3"), false);
				if (node.getNodeSolved() == false) { //Node solved for the first time
					yield return StartCoroutine(itemOutput());
					node.setNodeSolved (true);
					LoadSaveGame.saveLevel(level);
				} else {
					TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent6"), false); //Item already collected
				}
				restartMinigameButton.interactable = true;
				TTSManager.Speak (xmlReader.translate ("PlayLevelMenuFightOpponent7"), false); //Quiz successfully completed -> Restart if you want?
			}
		} 
		else { //Quiz not successfully completed -> Restart?
			restartMinigameButton.interactable = true;
			TTSManager.Speak (xmlReader.translate ("PlayLevelMenuSolutionQuestion4") + xmlReader.translate ("PlayLevelMenuFightOpponent7"), false);
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
	public IEnumerator itemOutput(){
        PlayerPrefs.SetInt("GameSaved", 0);
        TTSManager.Speak(xmlReader.translate("PlayLevelMenuFightOpponent3"), false);
        if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Item" + ".wav"))
        {
            yield return StartCoroutine(playAudio("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Item" + ".wav"));
        }
        else
            yield return null;
	}

    //Outputs when an item is collected, depends on the correct article (a,an,eine,ein) from the item
    public IEnumerator itemOutputWithoutMinigame()
    {
        TTSManager.Speak(xmlReader.translate("PlayLevelMenuPlayNodeItemCollect"), false);
        if (File.Exists(Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Item" + ".wav"))
        {
            yield return StartCoroutine(playAudio("file://" + Application.persistentDataPath + "/CurrentGame/Level" + levelNumber + "NodeNumber" + nodeIndex + "Item" + ".wav"));
            TTSManager.Speak(xmlReader.translate("PlayLevelMenuPlayNodeItemCollect3"), false);
        }
        else
            yield return null;
        
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

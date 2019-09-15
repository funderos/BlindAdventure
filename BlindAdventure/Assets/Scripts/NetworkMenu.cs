using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Networking;
using System.Net;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

//Authors: Stadler Viktor, Funder Benjamin
//Was created with the assistance of https://www.codeproject.com/Tips/443588/Simple-Csharp-FTP-Class
//This class manages the methods to share and download a game
public class NetworkMenu : MonoBehaviour {

    public Text inputFieldEmail;
    public Text inputFieldPassword;
    public AudioSource source;
    public AudioClip clip;
    public string[] gameNames;
    public int gameNameIndex = -1;

    //Variables to navigate 
    private Vector3 menuPosition;	
	public RectTransform menuContainer;
	public NavigationNetworkMenu navigationNetworkMenu;

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true

    private string tempMail;

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language

    //Variables to record the node names
    public Button startGameNameButton; //Button to start recording the game name
    public Button stopGameNameButton; //Button to stop recording the game name
    public Voice voice;

    void Awake() {
        int fromPlayLevelMenuNetwork = PlayerPrefs.GetInt ("FromPlayLevelMenuNetwork");
		if (fromPlayLevelMenuNetwork == 1) { //Go to the Levelmenu if swiped up in the game
			menuPosition = navigationNetworkMenu.navigateTo ("LevelMenu");
		}
		StartCoroutine (outputAwake (fromPlayLevelMenuNetwork));
	}

	IEnumerator outputAwake(int fromPlayLevelMenuNetwork) {
        //TTSManager.Initialize (transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
        source = GetComponent<AudioSource>();
        while (!TTSManager.IsInitialized ()) {
			yield return null;
		}
		language = PlayerPrefs.GetInt ("Language");	//Gets the current language from the player preferences; german = 1 or english = 0
		if (language == 1) {
			xmlReader.setLanguage (1);
			TTSManager.SetLanguage (TTSManager.GERMAN);
		} else {
			xmlReader.setLanguage (0);
			TTSManager.SetLanguage (TTSManager.ENGLISH);
		}
		if (fromPlayLevelMenuNetwork != 1) {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuAwake"), false);
		} else {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuPlaySavedGameButton"), false);
		}
		int i = 0;
		PlayerPrefs.SetInt ("FromPlayLevelMenuNetwork", i);
	}

	void Update () {
		menuContainer.anchoredPosition3D = Vector3.Lerp (menuContainer.anchoredPosition3D, menuPosition, 1f); //To navigate to a position

		//Detects swipe direction 
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				startPosition = touch;
			} 
			else if (swiped == false && touch.phase == TouchPhase.Moved) {
                Handheld.Vibrate();
				float differenceX = startPosition.position.x - touch.position.x;
				float differenceY = startPosition.position.y - touch.position.y;
				if (Mathf.Abs (differenceX) > 30 || Mathf.Abs (differenceY) > 30) {
					if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
                        //Swipe Left
                        if (differenceX > 0)
                        {
                            if (gameNameIndex >= 0)
                            {
                                if (gameNameIndex == 0)
                                    gameNameIndex = gameNames.Length - 1;
                                else
                                    gameNameIndex--;
                                string path = gameNames[gameNameIndex];
                                if (path.Contains("SavedGames"))
                                    StartCoroutine(playAudio(path.Substring(path.LastIndexOf("SavedGames"))));
                                else
                                    StartCoroutine(playAudio(path.Substring(path.LastIndexOf("DownloadTemp"))));
                            }
                            else
                            {
                                if (menuPosition == Vector3.zero)
                                {
                                    menuPosition = navigationNetworkMenu.navigateTo("CredentialsMenu");
                                    TTSManager.Speak(xmlReader.translate("NetworkMenuCredentialMenu"), false);
                                }
                                    
                            }
                        }
                        //Swipe Right
                        if (differenceX < 0 && gameNameIndex >= 0)
                        {
                            if (gameNameIndex == gameNames.Length - 1)
                                gameNameIndex = 0;
                            else
                                gameNameIndex++;
                            string path = gameNames[gameNameIndex];
                            if (path.Contains("SavedGames"))
                                StartCoroutine(playAudio(path.Substring(path.LastIndexOf("SavedGames"))));
                            else
                                StartCoroutine(playAudio(path.Substring(path.LastIndexOf("DownloadTemp"))));
                        }
                    }
					if (Mathf.Abs (differenceX) < Mathf.Abs (differenceY)) {
						//Swipe Down
						if (differenceY > 0) {
							navigationNetworkMenu.swipeDown(menuPosition);
						}
						//Swipe Up
						if (differenceY < 0) {
							menuPosition = navigationNetworkMenu.swipeUp (menuPosition);
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

    //Outputs persisent Audio File
    IEnumerator playAudio(string path)
    {
        path.Replace("\\", "/");
        while (TTSManager.IsSpeaking() || source.isPlaying)
        {
            yield return null;
        }
        UnityWebRequest wav = UnityWebRequestMultimedia.GetAudioClip(
            "file://" + Application.persistentDataPath + "/" + path, AudioType.WAV);
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

    //Gets the email adress from the database
    IEnumerator getEmail(string username){
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/GetEmail.php", form);
		yield return www;
		if (www.text == "no email found") {
			Debug.Log ("no email found");
		} else {
			StartCoroutine (getPassword(username, www.text));
		}
	}

	//Gets the password from the database and sends email to the user
	IEnumerator getPassword(string username, string email){
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/GetPassword.php", form);
		yield return www;
		if (www.text == "no password found") {
			Debug.Log ("no password found");
		} else {
			MailMessage mail = new MailMessage ();
			mail.From = new MailAddress ("blindadventure1@gmail.com");
			mail.To.Add(email);
			mail.Subject = "BlindAdventure Password";
			mail.Body = "Username: " + username + "\r\n" + "Password: " + www.text;
			SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
			smtpServer.Port = 587;
			smtpServer.Credentials = new System.Net.NetworkCredential ("blindadventure1@gmail.com", "blind1adventure1") as ICredentialsByHost;
			smtpServer.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = delegate (object sim, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				return true;
			};
			smtpServer.Send (mail);
			TTSManager.Speak (xmlReader.translate ("NetworkMenuEmail"), false); 
		}
	}

	//Navigates to the RegisterNameMenu
	public void onSaveGameButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			menuPosition = navigationNetworkMenu.navigateTo ("SaveGameMenu");
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSaveUploadMenu"), false); 
		}
	}

    public void onSaveGameLocalButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            if (PlayerPrefs.HasKey("GameSaved") && PlayerPrefs.GetInt("GameSaved") < 1)
            { //If the game isn't saved
                if (!PlayerPrefs.HasKey("GameID") || !File.Exists(Application.persistentDataPath +
                            "/SavedGames/" + PlayerPrefs.GetString("GameID") + ".wav"))
                {
                    menuPosition = navigationNetworkMenu.navigateTo("GameNameMenu");
                    TTSManager.Speak(xmlReader.translate("NetworkMenuRecordGameName"), false);
                }
                else
                {
                    LoadSaveGame.saveGame();
                    TTSManager.Speak(xmlReader.translate("NetworkMenuGameLocallySaved"), false);
                }
                
            }
            else
            { //Game is already saved
                TTSManager.Speak(xmlReader.translate("NetworkMenuGameAlreadySaved"), false);
            }
        }
    }

    //Starts with the recording of a game name
    public void onStartGameNameButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            startGameNameButton.gameObject.SetActive(false);
            stopGameNameButton.gameObject.SetActive(true);
            voice.recordBegin();
        }
    }

    //Stops the recording of a game name, save it and navigates to the SaveMenu. 
    public void onStopGameNameButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            stopGameNameButton.gameObject.SetActive(false);
            startGameNameButton.gameObject.SetActive(true);
            voice.recordEnd();
            voice.saveGameName();
            LoadSaveGame.saveGame();
            TTSManager.Speak(xmlReader.translate("NetworkMenuGameLocallySaved"), false);
            menuPosition = navigationNetworkMenu.navigateTo("SaveGameMenu");
        }
    }

    public void onUploadGameButtonClick()
    {
        if (swiped == false)
        {
            if (PlayerPrefs.HasKey("GameID") && File.Exists(Application.persistentDataPath +
                            "/SavedGames/" + PlayerPrefs.GetString("GameID") + ".zip"))
            {
                LoadSaveGame.uploadGame();
                TTSManager.Speak(xmlReader.translate("NetworkMenuUploadedGame"), false); //uploaded
            }
            else
            {
                TTSManager.Speak(xmlReader.translate("NetworkMenuSaveGameFirst"), false); //game needs to be saved first
            }
        }
    }

    public void onBrowseGameButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            menuPosition = navigationNetworkMenu.navigateTo("BrowseGameMenu");
            TTSManager.Speak(xmlReader.translate("NetworkMenuBrowseGameMenu"), false);
        }
    }

    public void onBrowseLocalButtonClick()
    {
        if (swiped == false)
        {
            gameNames = Directory.GetFiles(Application.persistentDataPath +
                            "/SavedGames/", "*.wav");
            if (gameNames.Length < 1)
            {
                TTSManager.Speak(xmlReader.translate("NetworkMenuNoSavedGames"), false); //no saved games
            }
            else
            {
                TTSManager.Speak(xmlReader.translate("NetworkMenuSavedGameInstructions"), false); //instrution
                gameNameIndex = 0;
                string path = gameNames[gameNameIndex];
                StartCoroutine(playAudio(path.Substring(path.LastIndexOf("SavedGames"))));
                menuPosition = navigationNetworkMenu.navigateTo("BrowseLocalMenu");               
            }
        }
    }

    public void onOpenGameButtonClick()
    {
        if (swiped == false)
        {
            string path = gameNames[gameNameIndex];
            path = path.Substring(path.LastIndexOf("SavedGames") + 11);
            LoadSaveGame.loadGame(path.Substring(0, path.Length - 4));
            menuPosition = navigationNetworkMenu.navigateTo("BrowseGameMenu");
            gameNameIndex = -1;
            TTSManager.Speak(xmlReader.translate("NetworkMenuGameOpened"), false); // opened
            TTSManager.Speak(xmlReader.translate("NetworkMenuBrowseGameMenu"), true);
        }
    }

    public void onDeleteGameButtonClick() //TODO
    {
        if (swiped == false)
        {
            string path = gameNames[gameNameIndex];
            File.Delete(path);
            File.Delete(path.Substring(0, path.Length - 3) + "zip");
            menuPosition = navigationNetworkMenu.navigateTo("BrowseGameMenu");
            gameNameIndex = -1;
            TTSManager.Speak(xmlReader.translate("NetworkMenuGameDeleted"), false); // deleted
            TTSManager.Speak(xmlReader.translate("NetworkMenuBrowseGameMenu"), true);
        }
    }

    public void onDownloadGameButtonClick()
    {
        if (swiped == false)
        {
            TTSManager.Speak(xmlReader.translate("NetworkMenuWaitForDownloadNames"), false);
            LoadSaveGame.getUploadedGames();
            gameNames = Directory.GetFiles(Application.persistentDataPath +
                            "/DownloadTemp/", "*.wav");
            if (gameNames.Length < 1)
            {
                TTSManager.Speak(xmlReader.translate("NetworkMenuNoOnlineGames"), false); //no online games
            }
            else
            {
                TTSManager.Speak(xmlReader.translate("NetworkMenuNavigateOnlineGames"), false); //instrution
                gameNameIndex = 0;
                string path = gameNames[gameNameIndex];
                StartCoroutine(playAudio(path.Substring(path.LastIndexOf("DownloadTemp"))));
                menuPosition = navigationNetworkMenu.navigateTo("DownloadGameMenu");
            }
        }
    }

    public void onDownloadButtonClick()
    {
        if (swiped == false)
        {
            string id = gameNames[gameNameIndex];
            LoadSaveGame.downloadGame(int.Parse(id.Substring(id.LastIndexOf("DownloadTemp") + 13, 1)));
            menuPosition = navigationNetworkMenu.navigateTo("BrowseGameMenu");
            gameNameIndex = -1;
            Directory.Delete(Application.persistentDataPath + "/DownloadTemp/", true);
            Directory.CreateDirectory(Application.persistentDataPath + "/DownloadTemp/");
            TTSManager.Speak(xmlReader.translate("NetworkMenuDownloadedGame"), false); // downloaded
        }
    }

    public void onSetCustomMailCredentialsButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            menuPosition = navigationNetworkMenu.navigateTo("RegisterMailMenu");
            TTSManager.Speak(xmlReader.translate("NetworkMenuRegisterMail"), false);
        }
    }

    public void onSetDefaultMailCredentialsButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            LoadSaveGame.setStandardUser();
            TTSManager.Speak(xmlReader.translate("NetworkMenuDefaultSet"), false);
        }
    }

    //Starts StartCoroutine if user already exists
    public void onConfirmMailAdressButtonClick()
    {
		if (swiped == false) {
			Handheld.Vibrate ();
            tempMail = inputFieldEmail.text.ToString();
            menuPosition = navigationNetworkMenu.navigateTo("RegisterPasswordMenu");
            TTSManager.Speak(xmlReader.translate("NetworkMenuRegisterPW"), false);
        }
	}

    public void onConfirmPasswordButtonClick()
    {
        if (swiped == false)
        {
            Handheld.Vibrate();
            LoadSaveGame.setCustomUser(tempMail, inputFieldPassword.text.ToString());
            menuPosition = navigationNetworkMenu.navigateTo("RegisterPasswordMenu");
            TTSManager.Speak(xmlReader.translate("NetworkMenuCredentialsRegistered"), false);
            while (TTSManager.IsSpeaking()) { }
            SceneManager.LoadScene("NetworkScene"); //Loads scene "NetworkScene" 
        }
    }
}
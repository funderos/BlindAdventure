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

//Author: Stadler Viktor
//This class manages the methods to share and download a game
public class NetworkMenu : MonoBehaviour {

	public string[] users;
	public Text inputFieldName;
	public Text inputFieldPassword;
	public Text inputFieldEmail;
	public Text inputFieldNameLogin;
	public Text inputFieldPasswordLogin;
	public Text inputFieldSearchName;

	//Variables to navigate 
	private Vector3 menuPosition;	
	public RectTransform menuContainer;
	public NavigationNetworkMenu navigationNetworkMenu;

	//Varibales to swipe
	private Touch startPosition = new Touch (); //Startposition of the swipe
	private bool swiped = false; //If we swipe then true

	private string myUserName;
	private string searchName;

	//Varibales for the translation
	public XMLReader xmlReader;
	private int language; //Current language

	void Awake() {
		int fromPlayLevelMenuNetwork = PlayerPrefs.GetInt ("FromPlayLevelMenuNetwork");
		if (fromPlayLevelMenuNetwork == 1) { //Go to the Levelmenu if swiped up in the game
			menuPosition = navigationNetworkMenu.navigateTo ("LevelMenu");
		}
		StartCoroutine (outputAwake (fromPlayLevelMenuNetwork));
	}

	IEnumerator outputAwake(int fromPlayLevelMenuNetwork) {
		TTSManager.Initialize (transform.name, "OnTTSInit"); //Initializes the Text-to-Speech Plugin
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
				float differenceX = startPosition.position.x - touch.position.x;
				float differenceY = startPosition.position.y - touch.position.y;
				if (Mathf.Abs (differenceX) > 30 || Mathf.Abs (differenceY) > 30) {
					if (Mathf.Abs (differenceX) > Mathf.Abs (differenceY)) {
						//Swipe Right
						if (differenceX < 0 && menuPosition == Vector3.right * 1600) { //LoginPasswordMenu
							//Email to the user with the password
							Handheld.Vibrate ();
							string username = inputFieldNameLogin.text.ToString ();
							StartCoroutine (getEmail(username));
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
	public void onRegisterButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			menuPosition = navigationNetworkMenu.navigateTo ("RegisterNameMenu");
			TTSManager.Speak (xmlReader.translate ("NetworkMenuRegisterButton"), false); 
		}
	}

	//Starts StartCoroutine if user already exists
	public void onConfirmNameButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			StartCoroutine (registerName ());
		}
	}

	//Looks in the database if user already exists for registration
	IEnumerator registerName(){
		string username = inputFieldName.text.ToString ();
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/RegisterName.php", form);
		yield return www;
		if (www.text == "user doesn't exist") {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuConfirmNameButton"), false); 
			menuPosition = navigationNetworkMenu.navigateTo ("RegisterPasswordMenu");
		} else { //username exist
			TTSManager.Speak (xmlReader.translate ("NetworkMenuConfirmNameButton1"), false); 
		}
	}

	//Navigates to the RegisterEmailMenu
	public void onConfirmPasswordButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			TTSManager.Speak (xmlReader.translate ("NetworkMenuConfirmPasswordButton"), false); 
			menuPosition = navigationNetworkMenu.navigateTo ("RegisterEmailMenu");
		}
	}
		
	//Starts StartCoroutine if email address already exists
	public void onCreateButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			StartCoroutine (checkEmail ());
		}
	}

	//Looks in the database if email address already exists
	IEnumerator checkEmail(){
		string email = inputFieldEmail.text.ToString ();
		Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
		Match match = regex.Match(email);
		if (match.Success) {
			WWWForm form = new WWWForm ();
			form.AddField ("emailPost", email);
			WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/RegisterEmail.php", form);
			yield return www;
			if (www.text == "email doesn't exist") {
				StartCoroutine (createAccount ());
			} else { //email exists
				TTSManager.Speak (xmlReader.translate ("NetworkMenuCheckEmail"), false); 
			}
		} else {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuCheckEmail1"), false); 
		}
	}

	//Creates new account
	IEnumerator createAccount(){
		string username = inputFieldName.text.ToString ();
		string userPassword = inputFieldPassword.text.ToString ();
		string email = inputFieldEmail.text.ToString ();
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("userpasswordPost", userPassword);
		form.AddField ("emailPost", email);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/InsertUser.php", form);
		yield return www;
		if (www.text == "Everything ok.") {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuCreateAccount"), false); 
			menuPosition = navigationNetworkMenu.navigateTo ("LoginMenu");
		} else { //Create Account failed
			TTSManager.Speak (xmlReader.translate ("NetworkMenuCreateAccount1"), false); 
		}
	}

	//Navigates to the LoginMenu
	public void onLoginButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginButton"), false); 
			menuPosition = navigationNetworkMenu.navigateTo ("LoginMenu");
		}
	}

	//Starts StartCoroutine if user exists for login
	public void onConfirmNameLoginButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			StartCoroutine (checkName());
		}
	}

	//Looks in the database if user exists for login
	IEnumerator checkName() {
		string username = inputFieldNameLogin.text.ToString ();
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/RegisterName.php", form);
		//WWW www = new WWW ("http://localhost/BlindAdventure/Login.php", form);
		yield return www;
		if (www.text == "user exists") {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuCheckName"), false);
			menuPosition = navigationNetworkMenu.navigateTo ("LoginPasswordMenu");
		} else { //user doesn't exist
			TTSManager.Speak (xmlReader.translate ("NetworkMenuCheckName1"), false);
		}
	}

	//Starts StartCoroutine to login
	public void onLoginButton2Click() {
		if (swiped == false) {
			Handheld.Vibrate ();
			StartCoroutine (LoginToDB ());
		}
	}

	//Login if username and password correct
	IEnumerator LoginToDB(){
		string username = inputFieldNameLogin.text.ToString ();
		string userPassword = inputFieldPasswordLogin.text.ToString ();
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("userpasswordPost", userPassword);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/Login.php", form);
		yield return www;
		if (www.text == "login success") {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginToDB"), false);
			myUserName = username;
			menuPosition = navigationNetworkMenu.navigateTo ("UploadMenu");
		} else { //Password wrong
			TTSManager.Speak (xmlReader.translate ("NetworkMenuLoginToDB1"), false);
		}
	}

	//Uploads game to the server
	public void onUploadButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			if (File.Exists (Application.persistentDataPath + "/Level_1.txt")) {
				try {
					TTSManager.Speak (xmlReader.translate ("NetworkMenuUpload0"), false);
					//create Directory
					try {
						FtpWebRequest ftpRequest0 = (FtpWebRequest)WebRequest.Create ("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + myUserName);
						ftpRequest0.Credentials = new NetworkCredential ("blindadventure", "Abcdb1234");
						ftpRequest0.UseBinary = true;
						ftpRequest0.UsePassive = true;
						ftpRequest0.KeepAlive = true;
						ftpRequest0.Method = WebRequestMethods.Ftp.MakeDirectory;
						FtpWebResponse ftpResponse0 = (FtpWebResponse)ftpRequest0.GetResponse ();
						ftpResponse0.Close ();
						ftpRequest0 = null;
					} catch (Exception ex) {
						Console.WriteLine (ex.ToString ());
					}
					//Delete previous Files
					string[] downloadFiles = filesInDirectory (myUserName);
					for (int i = 2; i < downloadFiles.Length - 1; i++) {
						try {
							FtpWebRequest ftpRequest0 = (FtpWebRequest)WebRequest.Create ("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + myUserName + "/" + downloadFiles [i]);
							ftpRequest0.Credentials = new NetworkCredential ("blindadventure", "Abcdb1234");
							ftpRequest0.UseBinary = true;
							ftpRequest0.UsePassive = true;
							ftpRequest0.KeepAlive = true;
							ftpRequest0.Method = WebRequestMethods.Ftp.DeleteFile;
							FtpWebResponse ftpResponse0 = (FtpWebResponse)ftpRequest0.GetResponse ();
							ftpResponse0.Close ();
							ftpRequest0 = null;
						} catch (Exception ex) {
							Console.WriteLine (ex.ToString ());
						}
					}
					uploadLevel ();
					uploadNode ();
					TTSManager.Speak (xmlReader.translate ("NetworkMenuUpload"), false);
				} catch (Exception ex) {
					TTSManager.Speak (xmlReader.translate ("NetworkMenuUpload1"), false);
					Console.WriteLine (ex.ToString ());
				}
			} else {
				TTSManager.Speak (xmlReader.translate ("NetworkMenuUpload2"), false);
			}
		}
	}

	//Uploads all levels to the server
	public void uploadLevel() {
		int i = 1;
		while (File.Exists (Application.persistentDataPath + "/Level_" + i + ".txt")) {
			//Nodesolved = "false";
			PlayerPrefs.SetInt ("LevelNumber", i);
			Level level = LoadSaveGame.loadLevel ();
			List <Node> nodeList = new List <Node> ();
			nodeList = level.getList ();
			for (int j = 0; j < nodeList.Count; j++) { 
				Node node = nodeList [j];
				node.setNodeSolved (false);
			}
			level.setLevelCompleted (false);
			LoadSaveGame.saveLevel (level);
			try{
				FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + myUserName + "/" + "Level_" + i + ".txt");
				ftpRequest.Credentials = new NetworkCredential("blindadventure", "Abcdb1234");
				ftpRequest.UseBinary = true;
				ftpRequest.UsePassive = true;
				ftpRequest.KeepAlive = true;
				ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
				Stream ftpStream = ftpRequest.GetRequestStream ();
				FileStream localFileStream = new FileStream(Application.persistentDataPath + "/Level_" + i + ".txt",FileMode.Open);
				byte[] byteBuffer = new byte[2048];
				int bytesSent = localFileStream.Read (byteBuffer, 0, 2048);
				try{
					while(bytesSent != 0) {
						ftpStream.Write(byteBuffer,0,bytesSent);
						bytesSent = localFileStream.Read(byteBuffer,0,2048);
					}
				}
				catch(Exception ex){
					Console.WriteLine (ex.ToString ());
				}
				localFileStream.Close ();
				ftpStream.Close ();
				ftpRequest = null;
			}
			catch(Exception ex) {
				Console.WriteLine (ex.ToString ());
			}
			i++;
		}
	}

	//Uploads all node and question-wavs to the server
	public void uploadNode() {
		int i = 1;
		while (File.Exists (Application.persistentDataPath + "/Level_" + i + ".txt")) {
			PlayerPrefs.SetInt ("LevelNumber", i);
			Level level = LoadSaveGame.loadLevel ();
			List <Node> nodeList = new List <Node> ();
			nodeList = level.getList ();
			for (int j = 0; j <= nodeList.Count; j++) {
				if(File.Exists (Application.persistentDataPath + "/Level" + i + "NodeNumber" + j + ".wav")){
					try{
						FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + myUserName + "/" + "Level" + i + "NodeNumber" + j + ".wav");
						ftpRequest.Credentials = new NetworkCredential("blindadventure", "Abcdb1234");
						ftpRequest.UseBinary = true;
						ftpRequest.UsePassive = true;
						ftpRequest.KeepAlive = true;
						ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
						Stream ftpStream = ftpRequest.GetRequestStream ();
						FileStream localFileStream = new FileStream(Application.persistentDataPath + "/Level" + i + "NodeNumber" + j + ".wav",FileMode.Open);
						byte[] byteBuffer = new byte[2048];
						int bytesSent = localFileStream.Read (byteBuffer, 0, 2048);
						try{
							while(bytesSent != 0) {
								ftpStream.Write(byteBuffer,0,bytesSent);
								bytesSent = localFileStream.Read(byteBuffer,0,2048);
							}
						}
						catch(Exception ex){
							Console.WriteLine (ex.ToString ());
						}
						localFileStream.Close ();
						ftpStream.Close ();
						ftpRequest = null;
					}
					catch(Exception ex) {
						Console.WriteLine (ex.ToString ());
					}
				}
				if (File.Exists (Application.persistentDataPath + "/Level" + i + "NodeNumber" + j + "Question" + "1" + ".wav")) {
					int k = 1;
					while(File.Exists (Application.persistentDataPath + "/Level" + i + "NodeNumber" + j + "Question" + k + ".wav")){
						try{
							FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + myUserName + "/" + "Level" + i + "NodeNumber" + j + "Question" + k + ".wav");
							ftpRequest.Credentials = new NetworkCredential("blindadventure", "Abcdb1234");
							ftpRequest.UseBinary = true;
							ftpRequest.UsePassive = true;
							ftpRequest.KeepAlive = true;
							ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
							Stream ftpStream = ftpRequest.GetRequestStream ();
							FileStream localFileStream = new FileStream(Application.persistentDataPath + "/Level" + i + "NodeNumber" + j + "Question" + k + ".wav",FileMode.Open);
							byte[] byteBuffer = new byte[2048];
							int bytesSent = localFileStream.Read (byteBuffer, 0, 2048);
							try{
								while(bytesSent != 0) {
									ftpStream.Write(byteBuffer,0,bytesSent);
									bytesSent = localFileStream.Read(byteBuffer,0,2048);
								}
							}
							catch(Exception ex){
								Console.WriteLine (ex.ToString ());
							}
							localFileStream.Close ();
							ftpStream.Close ();
							ftpRequest = null;
						}
						catch(Exception ex) {
							Console.WriteLine (ex.ToString ());
						}
						k++;
					}
				}
			}
			i++;
		}
	}

	//Navigates to the SearchMenu
	public void onSearchButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSearchButton"), false);
			menuPosition = navigationNetworkMenu.navigateTo ("SearchMenu");
		}
	}

	//Starts the StartCoroutine to search a username
	public void onSearchNameButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			StartCoroutine (searchUser ());
		}
	}

	//Looks if username in the database exists
	IEnumerator searchUser(){
		searchName = inputFieldSearchName.text.ToString ();
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", searchName);
		WWW www = new WWW ("http://blindadventure.bplaced.net/PHP/RegisterName.php", form);
		yield return www;
		if (www.text == "user exists") {
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSearchUser"), false);
			PlayerPrefs.SetString ("NameGame", searchName);
			menuPosition = navigationNetworkMenu.navigateTo ("DownloadMenu");
		} else { //user doesn't exist
			TTSManager.Speak (xmlReader.translate ("NetworkMenuSearchUser1"), false);
		}
	}

	//Downloads a game from the user which was searched
	public void onDownloadButtonClick(){
		if (swiped == false) {
			Handheld.Vibrate ();
			//Look if Directory exists
			try{
				FtpWebRequest ftpRequest0 = (FtpWebRequest)WebRequest.Create ("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + searchName);
				ftpRequest0.Credentials = new NetworkCredential ("blindadventure", "Abcdb1234");
				ftpRequest0.UseBinary = true;
				ftpRequest0.UsePassive = true;
				ftpRequest0.KeepAlive = true;
				ftpRequest0.Method = WebRequestMethods.Ftp.MakeDirectory;
				FtpWebResponse ftpResponse0 = (FtpWebResponse)ftpRequest0.GetResponse ();
				ftpResponse0.Close ();
				ftpRequest0 = null;
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
			}
			string[] downloadFiles = filesInDirectory (searchName);
			//If a level exists
			if (downloadFiles.Length > 3) {
				TTSManager.Speak (xmlReader.translate ("NetworkMenuDownload0"), false);
				for (int i = 2; i < downloadFiles.Length - 1; i++) {
					try {
						Directory.CreateDirectory (Application.persistentDataPath + "/" + searchName);
						FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create ("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + searchName + "/" + downloadFiles [i]);
						ftpRequest.Credentials = new NetworkCredential ("blindadventure", "Abcdb1234");
						ftpRequest.UseBinary = true;
						ftpRequest.UsePassive = true;
						ftpRequest.KeepAlive = true;
						ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
						FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse ();
						Stream ftpStream = ftpResponse.GetResponseStream ();
						FileStream localFileStream = new FileStream (Application.persistentDataPath + "/" + searchName + "/" + downloadFiles [i], FileMode.Create);
						byte[] byteBuffer = new byte[2048];
						int bytesRead = ftpStream.Read (byteBuffer, 0, 2048);
						try {
							while (bytesRead > 0) {
								localFileStream.Write (byteBuffer, 0, bytesRead);
								bytesRead = ftpStream.Read (byteBuffer, 0, 2048);
							}
						} catch (Exception ex) {
							Console.WriteLine (ex.ToString ());
						}
						localFileStream.Close ();
						ftpStream.Close ();
						ftpResponse.Close ();
						ftpRequest = null;
					} catch (Exception ex) {
						Console.WriteLine (ex.ToString ());
					}
				}
				TTSManager.Speak (xmlReader.translate ("NetworkMenuDownload"), false);
			} else { //No Game available
				TTSManager.Speak (xmlReader.translate ("NetworkMenuDownload1"), false);
			}
		}
	}

	//Navigates to the LevelMenu if game is available
	public void onPlaySavedGameButtonClick() {
		if (swiped == false) {
			Handheld.Vibrate ();
			if(File.Exists(Application.persistentDataPath + "/" + searchName + "/" + "Level_1.txt")){
				TTSManager.Speak (xmlReader.translate ("NetworkMenuPlaySavedGameButton"), false);
				menuPosition = navigationNetworkMenu.navigateTo ("LevelMenu");
			}
			else { //No game downloaded
				TTSManager.Speak (xmlReader.translate ("NetworkMenuPlaySavedGameButton1"), false);
			}
		}
	}

	//Starts a level if it exists
	public void onPlayLevelNetworkButtonClick(int levelIndex) {
		if (swiped == false) {
			Handheld.Vibrate ();
			if (File.Exists (Application.persistentDataPath + "/" + searchName + "/Level_" + levelIndex + ".txt")) { //Level exist
				if (levelIndex != 1) {  
					PlayerPrefs.SetInt ("LevelNumber", levelIndex - 1);
					int previousLevelNumber = levelIndex - 1;
					if (File.Exists (Application.persistentDataPath + "/" + searchName + "/Level_" + previousLevelNumber + ".txt")) {
						Level level = LoadSaveGameNetwork.loadLevel ();
						if (level.getLevelCompleted () == true) { //if previous level successfully completed; Navigates to the Scene "PlayLevelScene", because level exists and previous level successfully completed
							PlayerPrefs.SetInt ("LevelNumber", levelIndex);
							StartCoroutine (SceneLoad ());
						} else {
							TTSManager.Speak (xmlReader.translate ("MainMenuPlayLevelButton"), false);
						}
					} else {
						TTSManager.Speak (xmlReader.translate ("NetworkMenuPlayLevel"), false); //Previous level doesn't exist
					}
				} else { //Navigates to the Scene "PlayLevelScene", because level exists and level 1
					PlayerPrefs.SetInt ("LevelNumber", levelIndex);
					StartCoroutine (SceneLoad ());
				}
			} else {
				TTSManager.Speak (xmlReader.translate ("NetworkMenuPlayLevel1"), false); //Level doesn't exist
			}
		}
	}

	//Navigates to the PlayLevelNetworkScene
	IEnumerator SceneLoad() {
		TTSManager.Speak (xmlReader.translate ("MainMenuWait2"), false);
		yield return new WaitForSeconds (5);
		SceneManager.LoadScene ("PlayLevelNetworkScene"); 
	}
		
	//Searchs for all filennames from a user on the server 
	public string[] filesInDirectory(string name){
		FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://blindadventure.bplaced.net" + "/" + "UserFiles" + "/" + name + "/");
		ftpRequest.Credentials = new NetworkCredential("blindadventure", "Abcdb1234");
		ftpRequest.UseBinary = true;
		ftpRequest.UsePassive = true;
		ftpRequest.KeepAlive = true;
		ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
		FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
		Stream ftpStream = ftpResponse.GetResponseStream ();
		StreamReader ftpReader = new StreamReader (ftpStream);
		string directoryRaw = null;
		try{while(ftpReader.Peek() != -1){directoryRaw += ftpReader.ReadLine() + "|";}}
		catch(Exception ex){
			Console.WriteLine (ex.ToString ());
		}
		ftpReader.Close ();
		ftpStream.Close ();
		ftpResponse.Close ();
		ftpRequest = null;
		try{string[]directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList;}
		catch(Exception ex){
			Console.WriteLine (ex.ToString ());
			return null;
		}
	}
}

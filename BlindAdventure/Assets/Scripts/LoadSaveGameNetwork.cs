using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Authors: Stadler Viktor
//This class manages the methods to load and save levels in the Network-Mode
public static class LoadSaveGameNetwork {

	//Method to save levels in the application data path
	public static void saveLevel (Level level) {
		int levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Get the current levelNumber from the player preferences
		string searchname = PlayerPrefs.GetString ("NameGame");
		BinaryFormatter binary = new BinaryFormatter ();
		FileStream fStream = File.Create (Application.persistentDataPath + "/" + searchname + "/Level_" + levelNumber + ".txt"); 
		binary.Serialize (fStream, level);
		fStream.Close ();
	}

	//Method to load levels from the application data path
	public static Level loadLevel () {
		int levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Get the current levelNumber from the player preferences
		string searchname = PlayerPrefs.GetString ("NameGame");
		Level loadedLevel = null;
		if (File.Exists (Application.persistentDataPath + "/" + searchname + "/Level_" + levelNumber + ".txt")) { 
			BinaryFormatter binary = new BinaryFormatter ();
			FileStream fStream = File.Open (Application.persistentDataPath + "/" + searchname + "/Level_" + levelNumber + ".txt", FileMode.Open); 
			loadedLevel = new Level ();
			loadedLevel = (Level)binary.Deserialize(fStream);
			fStream.Close ();
		}
		return loadedLevel;
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Author: Stadler Viktor
//This class manages the methods to load and save levels
public static class LoadSaveGame {

	//Method to save levels in the application data path
	public static void saveLevel (Level level) {
		int levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Get the current levelNumber from the player preferences
		BinaryFormatter binary = new BinaryFormatter ();
		FileStream fStream = File.Create (Application.persistentDataPath + "/Level_" + levelNumber + ".txt"); 
		binary.Serialize (fStream, level);
		fStream.Close ();
	}

	//Method to load levels from the application data path
	public static Level loadLevel () {
		int levelNumber = PlayerPrefs.GetInt ("LevelNumber"); //Get the current levelNumber from the player preferences
		Level loadedLevel = null;
		if (File.Exists (Application.persistentDataPath + "/Level_" + levelNumber + ".txt")) { 
			BinaryFormatter binary = new BinaryFormatter ();
			FileStream fStream = File.Open (Application.persistentDataPath + "/Level_" + levelNumber + ".txt", FileMode.Open); 
			loadedLevel = new Level ();
			loadedLevel = (Level)binary.Deserialize(fStream);
			fStream.Close ();
		}
		return loadedLevel;
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author: Stadler Viktor
//This class manages the methods to navigate in the NetworkScene
public class NavigationNetworkMenu : MonoBehaviour
{

    private Vector3 menuPosition; //returns the new vector3 position
    public XMLReader xmlReader;

    //Returns the new Vector3 position. Depends on which Button was pressed.
    public Vector3 navigateTo(string position)
    {
        if (position == "SaveGameMenu")
        {
            menuPosition = Vector3.left * 800;
            int i = 0;
            PlayerPrefs.SetInt("FromPlayLevelMenuNetwork", i);
        }
        else if (position == "GameNameMenu")
        {
            menuPosition = Vector3.left * 1600;
        }
        else if (position == "RegisterEmailMenu")
        {
            menuPosition = Vector3.left * 2400;
        }
        else if (position == "RegisterPasswordMenu")
        {
            menuPosition = Vector3.left * 3200;
        }
        else if (position == "BrowseGameMenu")
        {
            menuPosition = Vector3.left * -800;
        }
        else if (position == "BrowseLocalMenu")
        {
            menuPosition = Vector3.left * -1600;
        }
        else if (position == "DownloadGameMenu")
        {
            menuPosition = Vector3.left * -2400;
        }
        else if (position == "RegisterMailMenu")
        {
            menuPosition = Vector3.left * -3200;
        }
        else if (position == "CredentialsMenu")
        {
            menuPosition = Vector3.left * -4000;
        }
        else
        {
            menuPosition = Vector3.zero; //MainMenu
        }
        return menuPosition;
    }

    //Returns the new Vector3 position and gives an audio output which new functions are possible. Depends on which vector3 position the swipe-up was.
    public Vector3 swipeUp(Vector3 menuPosition)
    {
        Handheld.Vibrate();
        if (menuPosition == Vector3.zero)
        { //GameMenu
            SceneManager.LoadScene("MainScene"); //Loads scene "MainScene" 
        }
        else if (menuPosition == Vector3.left * 800)
        { //SaveGameMenu
            SceneManager.LoadScene("NetworkScene"); //Loads scene "NetworkScene"  
        }
        else if (menuPosition == Vector3.left * 2400)
        { //RegisterEmailMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuConfirmNameButton"), false);
            menuPosition = Vector3.left * 1600; //RegisterPasswordMenu
        }
        else if (menuPosition == Vector3.left * 3200)
        { //RegisterPWMenu
            menuPosition = Vector3.left * -4000; //CredentialsMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuNoCredentialsRegistered"), false);
        }
        else if (menuPosition == Vector3.left * -800)
        { // BrowseGameMenu
            SceneManager.LoadScene("NetworkScene"); //Loads scene "NetworkScene" 
        }
        else if (menuPosition == Vector3.left * -1600)
        { //BrowseLocalMenu
            menuPosition = Vector3.left * -800; //BrowseGameMenu
        }
        else if (menuPosition == Vector3.left * -2400)
        { //DownloadGameMenu
            menuPosition = Vector3.left * -800; //BrowseGameMenu
        }
        else if (menuPosition == Vector3.left * -3200)
        { //RegisterMailMenu
            menuPosition = Vector3.left * -4000; //CredentialsMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuNoCredentialsRegistered"), false);
        }
        else if (menuPosition == Vector3.left * -4000)
        { //CredentialsMenu
            SceneManager.LoadScene("NetworkScene"); //Loads scene "NetworkScene"  
        }
        else if (menuPosition == Vector3.left * -4800 && PlayerPrefs.GetInt("FromPlayLevelMenuNetwork") != 1)
        { //LevelMenu
            SceneManager.LoadScene("MainScene"); //Loads scene "MainScene"
        }
        return menuPosition;
    }

    //Gives an audio output which current functions are possible. Depends on which vector3 position the swipe-down was.
    public void swipeDown(Vector3 menuPosition)
    {
        Handheld.Vibrate();
        if (menuPosition == Vector3.zero)
        {
            TTSManager.Speak(xmlReader.translate("NetworkMenuLoginRegisterMenu"), false);
        }
        else if (menuPosition == Vector3.left * 800)
        { //SaveGameMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuRegisterNameMenu"), false);
        }
        else if (menuPosition == Vector3.left * 1600)
        { //GameNameMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuRegisterPasswordMenu"), false);
        }
        else if (menuPosition == Vector3.left * 2400)
        { //RegisterEmailMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuRegisterEmailMenu"), false);
        }
        else if (menuPosition == Vector3.left * -800)
        { //BrowseGameMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuLoginNameMenu"), false);
        }
        else if (menuPosition == Vector3.left * -1600)
        { //BrowseLocalMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuLoginPasswordMenu"), false);
        }
        else if (menuPosition == Vector3.left * -2400)
        { //DownloadGameMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuUploadMenu"), false);
        }
        else if (menuPosition == Vector3.left * -3200)
        { //SearchMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuSearchMenu"), false);
        }
        else if (menuPosition == Vector3.left * -4000)
        { //DownloadMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuDownloadMenu"), false);
        }
        else if (menuPosition == Vector3.left * -4800)
        { //LevelMenu
            TTSManager.Speak(xmlReader.translate("NetworkMenuLevelMenu"), false);
        }
    }
}
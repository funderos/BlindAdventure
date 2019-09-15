using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

//Authors: Stadler Viktor, Funder Benjamin
//This class manages the translation with the XML-File "Languages"
public class XMLReader : MonoBehaviour {

	public TextAsset dictionary;
	public int language; //german = 1 or english = 0

	public void setLanguage(int language){
		this.language = language;
	}
	public int getLanguage(){
		return language;
	}

	//Read XML-File and search for the name which should be translated
	public string translate(string name){
		XmlDocument xmlDocument = new XmlDocument ();
		xmlDocument.LoadXml (dictionary.text);
		XmlNodeList languageList = xmlDocument.GetElementsByTagName ("language");
		int i = 0;
		if (language < languageList.Count) { //german = 1 or english = 0
			i = language;
		}
		XmlNodeList xmlNodeList = languageList[i].ChildNodes;
		foreach(XmlNode value in xmlNodeList)
		{
			if(value.Name == name){ //name which should be translated was found
				name = value.InnerText;
				break;
			}
		}
		return name; //return translated name
	}
}

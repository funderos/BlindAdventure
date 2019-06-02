using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//Author: Stadler Viktor
//This class manages the methods to record an audio input
public class Voice : MonoBehaviour {

	private AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
	}

	public void recordBegin() {
		audioSource.clip = Microphone.Start (null,true,30,44100); //(deviceName, loop, length in sec, frequency)
	}

	public void recordEnd() {
		Microphone.End (null);
	}

	public void playVoice() {
		audioSource.Play (); 
	}

	//Saves a node name
	public void saveNodeName(int levelNumber, int nodeNumber) {
		trim ();
		string filename = Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeNumber;
		if(!filename.ToLower().EndsWith(".wav")){
			filename += ".wav";
		}
		settings (filename);
	}

    public void saveItemName(int levelNumber, int nodeNumber)
    {
        trim();
        string filename = Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeNumber + "Item";
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }
        settings(filename);
    }

    //Saves a question
    public void saveQuestion(int levelNumber, int nodeNumber, int questionNumber) {
		trim ();
		string filename = Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeNumber + "Question" + questionNumber;
		if(!filename.ToLower().EndsWith(".wav")){
			filename += ".wav";
		}
		settings (filename);
	}

    //Saves an obstacle
    public void saveObstacle(int levelNumber, int nodeNumber, int obstacleNumber)
    {
        trim();
        string filename = Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeNumber + "Obstacle" + obstacleNumber;
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }
        settings(filename);
    }

    //Saves an opponent
    public void saveOpponent(int levelNumber, int nodeNumber, int opponentNumber, bool scream)
    {
        trim();
        string filename = Application.persistentDataPath + "/Level" + levelNumber + "NodeNumber" + nodeNumber + "Opponent" + opponentNumber;
        if (scream)
            filename += "Scream";
        else
            filename += "Name";
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }
        settings(filename);
    }

    //Trims the silence 
    //Was created with the assistance of https://docs.unity3d.com/ScriptReference/AudioSource.SetScheduledEndTime.html and the contained URL: https://gist.github.com/darktable/2317063 
    public void trim(){
		int channels = audioSource.clip.channels;
		int hz = audioSource.clip.frequency;
		float[] samples = new float[audioSource.clip.samples];
		audioSource.clip.GetData (samples,0);
		List <float> samplesList = new List <float> (samples);
		int i;
		for (i = samplesList.Count-1; i > 0; i--) {	
			if(Mathf.Abs(samplesList[i]) > 0.0001f) {
				break;
			}
		}
		samplesList.RemoveRange (i,samplesList.Count - i);
		audioSource.clip = AudioClip.Create ("Audio",samplesList.Count, channels, hz, false);
		audioSource.clip.SetData (samplesList.ToArray(),0);
	}

	//Generates the settings from the input
	//Was created with the assistance of https://docs.unity3d.com/ScriptReference/AudioSource.SetScheduledEndTime.html and the contained URL: https://gist.github.com/darktable/2317063 
	public void settings (string path){
		FileStream fileStream = new FileStream(path,FileMode.Create);
		for(int i = 0; i < 44; i++) {
			fileStream.WriteByte(new byte ());
		}
		float[] samples = new float[audioSource.clip.samples];
		audioSource.clip.GetData (samples, 0);

		short [] shortSamples = new short[samples.Length];
		byte[] byteSamples = new byte[samples.Length * 2];
		for (int i = 0; i < samples.Length; i++) {
			shortSamples [i] = (short)(samples [i] * 32767);
		}
		System.Buffer.BlockCopy (shortSamples, 0, byteSamples, 0, byteSamples.Length);
		MemoryStream memoryStream = new MemoryStream ();
		memoryStream.Write (byteSamples, 0, byteSamples.Length);
		memoryStream.WriteTo (fileStream);

		int hz = audioSource.clip.frequency;
		int channels = audioSource.clip.channels;
		int samplesNeu = audioSource.clip.samples;
		fileStream.Seek(0, SeekOrigin.Begin);
		fileStream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
		fileStream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
		fileStream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes(16), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes(1), 0, 2);
		fileStream.Write(System.BitConverter.GetBytes(channels), 0, 2);
		fileStream.Write(System.BitConverter.GetBytes(hz), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes(hz * channels * 2), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes((ushort) (channels * 2)), 0, 2);
		fileStream.Write(System.BitConverter.GetBytes(16), 0, 2);
		fileStream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
		fileStream.Write(System.BitConverter.GetBytes(samplesNeu * channels * 2), 0, 4);
	}
}

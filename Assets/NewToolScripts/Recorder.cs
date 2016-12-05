using UnityEngine;
using System.Collections;
using System.IO;

public class Recorder : MonoBehaviour
{

    AudioClip myAudioClip;

    void Start()
    {
     
    }

    public void BeginRecording()
    {
		myAudioClip = Microphone.Start (null, false, 600, 44100);
    }

	public void EndRecord(string fileName)
	{
		var position = Microphone.GetPosition(null);
		Microphone.End(null);
		//var position = Microphone.GetPosition(null);

		var soundData = new float[myAudioClip.samples * myAudioClip.channels];
		myAudioClip.GetData(soundData, 0);

		//Create shortened array for the data that was used for recording
		var newData = new float[position * myAudioClip.channels];

		//Copy the used samples to a new array
		for (int i = 0; i < newData.Length; i++)
		{
			newData[i] = soundData[i];
		}

		//One does not simply shorten an AudioClip,
		//    so we make a new one with the appropriate length
		var newClip = AudioClip.Create(myAudioClip.name, position, myAudioClip.channels, myAudioClip.frequency, false);

		newClip.SetData(newData, 0);        //Give it the data from the old clip

		//Replace the old clip
		AudioClip.Destroy(myAudioClip);
		myAudioClip = newClip;
		string filepath = Path.Combine(Application.dataPath + "/Audio/", fileName + ".wav");
		Debug.Log ("Saved file at: " + filepath);
		SavWav.Save(filepath, newClip);
	}

	public void Play(string fileName)
    {
		string filepath = Path.Combine(Application.dataPath + "/Audio/", fileName + ".wav");
        AudioClip clip = LoadFile(filepath);
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
		source.time = 0;
		//Debug.Log ("Playing audio at: " + clip.time);
        source.Play();
    }

	public void PlayAt(string fileName, float time)
    {
		string filepath = Path.Combine(Application.dataPath + "/Audio/", fileName + ".wav");
        AudioClip clip = LoadFile(filepath);
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;

        source.time = time;
        source.Play();
    }

    AudioClip LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        Debug.Log("loading " + path);

        AudioClip clip = www.GetAudioClip(false);
		while (clip.loadState != AudioDataLoadState.Loaded) 
		{
			//int i = 0;
		}
       // Debug.Log("done loading");
        //clip.name = Path.GetFileName(path);
        return clip;
    }

    public void Pause()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Pause();
    }

    public void Resume()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.UnPause();
    }
}

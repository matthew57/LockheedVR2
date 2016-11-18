using UnityEngine;
using System.Collections;
using System.IO;

public class Recorder : MonoBehaviour
{

	AudioClip myAudioClip;
	int clipNum;

	void Start()
	{
		clipNum = 0;
	}


	public void BeginRecording()
	{
		myAudioClip = Microphone.Start(null, false, 10, 44100);
	}

	public void EndRecord()
	{
		clipNum++;
		Microphone.End(null);
		string fileName = "myFile" + clipNum.ToString();
		SavWav.Save(fileName, myAudioClip);
	}

	public void Play()
	{
		AudioClip clip = Resources.Load("myFile1") as AudioClip;
		AudioSource source = GetComponent<AudioSource>();
		source.clip = clip;
		Debug.Log("Clip Assigned");
		source.Play();
	}

	public void PlayAt(float time)
	{
		AudioClip clip = Resources.Load("myFile1") as AudioClip;
		AudioSource source = GetComponent<AudioSource>();
		source.clip = clip;
		source.time = time;
		source.Play();
	
	}
}
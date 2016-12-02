using UnityEngine;
using System.Collections;
using System.IO;

public class Recorder : MonoBehaviour
{

    AudioClip myAudioClip;
    int clipNum;
    string filepath;

    void Start()
    {
        clipNum = 0;
    }

    public void BeginRecording()
    {
		myAudioClip = Microphone.Start (null, false, 10, 44100);
    }

    public void EndRecord()
    {
        clipNum++;
        Microphone.End(null);
        string fileName = "myFile" + clipNum.ToString();
        filepath = Path.Combine(Application.dataPath + "/Audio/", fileName + ".wav");
        SavWav.Save(filepath, myAudioClip);
		//File.Write
    }

    public void Play()
    {
        AudioClip clip = LoadFile(filepath);
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        Debug.Log("Clip Assigned");
        source.Play();
    }

    public void PlayAt()
    {
        AudioClip clip = LoadFile(filepath);
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        source.time = 3;
        source.Play();
    }

    AudioClip LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        print("loading " + path);

        AudioClip clip = www.GetAudioClip(false);
        while (clip.loadState != AudioDataLoadState.Loaded)
        {
            int i = 0;
        }

        print("done loading");
        //clip.name = Path.GetFileName(path);
        return clip;
    }
}

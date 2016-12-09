using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlaybackManager : MonoBehaviour {

    string[] recordings;

	public GameObject playbackPanel;

	// Use this for initialization
	void Awake () {
        //First, I need to go into the file system and grab all the scenes that have been saved
        //Save those names to a list
        //Create Panels for each scene
        //Make each panel have a tag with it's name
        //Load the scene with that tag
        LoadPlayBackScenes();
        DisplayPlayBackScenes();
	}

    private void LoadPlayBackScenes()
    {
        string filepath = Application.dataPath + "/PlaybackFiles/";
        recordings = System.IO.Directory.GetFiles(filepath, "*.UVR");
		Debug.Log ("I found " + recordings.Length + " recordings");
    }

    private void DisplayPlayBackScenes()
    {
        Vector3 playerLocation = GameObject.FindObjectOfType<Camera>().transform.position;
        float xPos = -6.31f;
        float yPos = 3.19f;
        float zPos = 5.73f;
        float yRotation = -53.7f;
        int numRecordings = recordings.Length;
        for (int i = 0; i < numRecordings; i++)
        {
			Debug.Log ("Trying to display: " + recordings [i]);
			Color panelColor = Color.gray;
            //odds on top evens on bottom
            if (i > 1)
            {
                xPos += 4;
                zPos += 3;
                yRotation += 33.6f;
            } 
            if (i + 1 % 2 == 0)
            {
                yPos -= 2.13f;
                panelColor = Color.blue;
            }
			GameObject go = (GameObject)Instantiate (playbackPanel);
			go.transform.position = new Vector3 (xPos, yPos, zPos);
			go.transform.eulerAngles = new Vector3(0, yRotation, 0);
			Canvas[] canvii =  go.GetComponentsInChildren<Canvas> ();
			foreach (Canvas c in canvii) 
			{
				Text[] texts = c.GetComponentsInChildren<Text> ();
				foreach (Text t in texts)
					t.text = recordings [i];
			}
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}

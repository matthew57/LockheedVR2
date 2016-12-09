using UnityEngine;
using System.Collections;
using System;

public class PlaybackManager : MonoBehaviour {

    string[] recordings;

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
    }

    private void DisplayPlayBackScenes()
    {
        Vector3 playerLocation = GetComponent<Camera>().transform.position;
        float xPos = -6.31f;
        float yPos = 3.19f;
        float zPos = 5.73f;
        float yRotation = -53.7f;
        int numRecordings = recordings.Length;
        for (int i = 0; i < numRecordings; i++)
        {
            Color panelColor = Color.gray;
            //odds on top evens on bottom
            if (i > 1)
            {
                xPos += 4;
                zPos += 3;
                yRotation += 33.6f;
            } 
            if (i % 2 == 0)
            {
                yPos -= 2.13f;
                panelColor = Color.blue;
            }
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.position = new Vector3(playerLocation.x + xPos, playerLocation.y + yPos, playerLocation.z + zPos);
            quad.transform.rotation = new Quaternion(0, yRotation, 0, 0);
            TextMesh text = quad.AddComponent<TextMesh>();
            text.text = recordings[i];
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}

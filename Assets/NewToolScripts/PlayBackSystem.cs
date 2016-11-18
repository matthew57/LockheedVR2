﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//CREATED BY MATTHEW JOHNSON ON 10-29-16: PROTOTYPE SYSTEM FOR PLAYING BACK STUFF FROM RECORDING SYSTEM

public class PlayBackSystem :Tools {

	public float playBackRate = 1;
	[Tooltip("This number determines how fastthe initial fast forward and rewind happens")]
	public float fastMultiplier;
	float initialFastMultiplier;

	RecordingSystem.SavedData savedData;
	float currentPlaybackTime;

	public GameObject leftHand;
	public GameObject rightHand;
	public GameObject Head;


	RecordingSystem.movementFrame previousFrame;
	RecordingSystem.movementFrame nextFrame;


	Text countDown;
	float frameTime;

	//THIS is a temprorary hack, implement this better later
	List<PlayBackInputController> playControllers = new List<PlayBackInputController>();


	PlayBackState currentState;
	//==============================================================================================================
	// pressing the fastforward/rewind keys [left/right arrows] while already in those states will accelerate how fast time is moving for the play back
	// pressing the play/pause button [down arrow] will reset the playback rate and do the play/pause
	//
	//
	//
	//==============================================================================================================

	// Use this for initialization
	void Start () {

		foreach (PlayBackInputController pbic in GameObject.FindObjectsOfType<PlayBackInputController>() )
			{
				playControllers.Add(pbic);	
			}

		initialFastMultiplier = fastMultiplier;
		countDown = GameObject.Find ("SecondToolText").GetComponent<Text> ();
	
	}
	
	// Update is called once per frame
	void Update () {



		if (currentState != null) {
			if (Input.GetKeyDown (KeyCode.LeftArrow) && !(currentState is RewindState)) {
				fastMultiplier = initialFastMultiplier;
				currentState = new RewindState ();
			} else if (Input.GetKeyDown (KeyCode.RightArrow) && !(currentState is FastForwardState)) {
				currentState = new FastForwardState ();
				fastMultiplier = initialFastMultiplier;
			}


			currentState.update (this);

			countDown.text = "" + TimeConverter.getClockTime(currentPlaybackTime);

		}
	}


	public void rewind()
	{if (savedData!= null) {
		if (!(currentState is RewindState)) {
			fastMultiplier = initialFastMultiplier;
			currentState = new RewindState ();
		} else {
			fastMultiplier += .5f;
		}
	}
	
	}
	public void fastForward()
	{
		if (savedData!= null) {
			if (!(currentState is FastForwardState)) {
				currentState = new FastForwardState ();
				fastMultiplier = initialFastMultiplier;
			} else {
				fastMultiplier+= .5f;
			}
		}
	}


	public void playPause()
	{if (savedData != null) {
			fastMultiplier =  initialFastMultiplier;
			if (!(currentState is PauseState)) {
				currentState = new PauseState ();
			} else {
				currentState = new RegularPlayState ();
			}


		
		}
	}


	public void updateObjTransforms()
	{//Because we only record at X (usually 30) frames a second, we need to interpolate between points to create smooth motion for the playback system.
		float lerpAmount =( currentPlaybackTime - previousFrame.RT )/ frameTime;

		leftHand.transform.position = Vector3.Lerp (previousFrame.lP, nextFrame.lP, lerpAmount);
		leftHand.transform.rotation = Quaternion.Slerp (previousFrame.lQ, nextFrame.lQ, lerpAmount);

		rightHand.transform.position = Vector3.Lerp (previousFrame.rP, nextFrame.rP, lerpAmount);
		leftHand.transform.rotation = Quaternion.Slerp (previousFrame.rQ, nextFrame.rQ, lerpAmount);

		Head.transform.position = Vector3.Lerp (previousFrame.hP, nextFrame.hP, lerpAmount);
		Head.transform.rotation = Quaternion.Slerp (previousFrame.hQ, nextFrame.hQ, lerpAmount);
	}


	public void play(RecordingSystem.SavedData sData)
	{
		currentState = new RegularPlayState ();
		frameTime = 1 / sData.frameRate;
		GameObject.FindObjectOfType<Recorder> ().Play ();
		savedData = sData;

		previousFrame = savedData.savedFrames [0];
		nextFrame = savedData.savedFrames[1];
		currentPlaybackTime = savedData.savedFrames [0].RT;

		countDown.color = Color.green;
		countDown.enabled = true;

		foreach (PlayBackInputController pbic in playControllers) {
			foreach (ITool currentTool in pbic.myTools) {
				if (currentTool is NewMenuSelector) {
					((NewMenuSelector)currentTool).setMenuIndex (sData.startingMenuIndex);
					((NewMenuSelector)currentTool).setMenuOnOff (sData.menuOn);
				}
			}
		}


	}



	// does the normal thing to your objects per one frame of playback
	public void PlayIt(float playRate)
	{

		//Debug.Log ("Playing it ");
		currentPlaybackTime += Time.deltaTime * playRate;
		// Going forward in Time
		if (playRate > 0) {

			while (currentPlaybackTime >= nextFrame.RT) {
				
				previousFrame = nextFrame;
				nextFrame = savedData.getNextFrame (previousFrame);

				if (nextFrame.RT == previousFrame.RT) {


					currentState = null;
					return;
				}

				foreach (ClickedEventArgs ea in nextFrame.myC) {
					foreach (PlayBackInputController pbic in playControllers) {
					//	Debug.Log ("Caliing actions " + ea.butClicked + "   " + pbic.gameObject);
						pbic.callButtonAction (ea);
					}
				}

			}

			//Rewinding Time
		} else if (playRate < 0) {
			while (currentPlaybackTime <= previousFrame.RT) {
				nextFrame = previousFrame;

				previousFrame = savedData.getGetPrevFrame (previousFrame);

				if (nextFrame.RT == previousFrame.RT) {


					currentState = null;
					return;
				}

				//IMPLEMENT UNDO ACTIONS FOR ALL TOOLS!

				foreach (ClickedEventArgs ea in nextFrame.myC) {
				//	Debug.Log ("I clicked " + ea.controllerIndex + "   " + ea.butClicked);
				}

			}
		}


		updateObjTransforms ();
	}









	// STATE MACHINE STATES FOR PLAY BACK

	public class RegularPlayState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				pbs.currentState = new PauseState ();
			}

			pbs.PlayIt (1);

		}
	}



	public class PauseState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				pbs.currentState = new RegularPlayState();
			}
		}
	}


	public class FastForwardState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{

		//	Debug.Log ("in fastforward state");
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				pbs.currentState = new PauseState ();
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				pbs.fastMultiplier++;
			} 


			pbs.PlayIt (pbs.fastMultiplier);
		}

	}

	public class RewindState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{	//Debug.Log ("in rewind state");
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				pbs.currentState = new PauseState ();
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				pbs.fastMultiplier++;
			} 
			
			pbs.PlayIt (-1 * pbs.fastMultiplier);
		}

	}


}



public interface PlayBackState{

	void update(PlayBackSystem pbs);

}






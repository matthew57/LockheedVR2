using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	float frameTime;

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
		frameTime = 1 / 30;// GameObject.FindObjectOfType<RecordingSystem> ().frameRate;
		initialFastMultiplier = fastMultiplier;
	
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
		savedData = sData;
		Debug.Log ("PLAYING");
		previousFrame = savedData.savedFrames [0];
		nextFrame = savedData.savedFrames[1];
		currentPlaybackTime = savedData.savedFrames [0].RT;

	}



	// does the normal thing to your objects per one frame of playback
	public void PlayIt(float playRate)
	{
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
					Debug.Log ("I clicked " + ea.controllerIndex + "   " + ea.butClicked);
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

				foreach (ClickedEventArgs ea in nextFrame.myC) {
					Debug.Log ("I clicked " + ea.controllerIndex + "   " + ea.butClicked);
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






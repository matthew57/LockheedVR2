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

	public RecordingSystem.SavedData savedData;
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
	int currentFrameIndex;

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
			//Debug.Log ("Updating " + currentState);

			currentState.update (this);

			countDown.text = "" + TimeConverter.getClockTime (currentPlaybackTime);
		}
	}


	public RecordingSystem.movementFrame getCurrentFrame(bool timeNormal)
	{
		if (timeNormal) {
			return previousFrame;}
		return nextFrame;
	}

	public bool isPlaying()
	{if (currentState == null) {
			return false;
		}
		else {
			return true;
	}
	}

	public void rewind()
	{if (savedData!= null) {
		if (!(currentState is RewindState)) {
			GameObject.FindObjectOfType<Recorder> ().Pause ();
			fastMultiplier = initialFastMultiplier;
			currentState = new RewindState ();
				ErrorPrompt.instance.showError ("Rewind X"+ fastMultiplier);
		} else {
			fastMultiplier += 1f;
				ErrorPrompt.instance.showError ("Rewind X"+ fastMultiplier);
		}
	}
	
	}
	public void fastForward()
	{
		if (savedData!= null) {
			if (!(currentState is FastForwardState)) {
				GameObject.FindObjectOfType<Recorder> ().Pause ();
				currentState = new FastForwardState ();
				fastMultiplier = initialFastMultiplier;
				ErrorPrompt.instance.showError ("Fast Forward X"+ fastMultiplier);
			} else {
				fastMultiplier+= 1f;
				ErrorPrompt.instance.showError ("Fast Forward X"+ fastMultiplier);
			}
		}
	}


	public void playPause()
	{if (savedData != null) {
			fastMultiplier =  initialFastMultiplier;
			if (!(currentState is PauseState)) {
				currentState = new PauseState ();
				ErrorPrompt.instance.showError ("Paused");
				GameObject.FindObjectOfType<Recorder> ().Pause ();
			} else {
				currentState = new RegularPlayState ();
				ErrorPrompt.instance.showError ("Play");

				GameObject.FindObjectOfType<Recorder> ().PlayAt ("testSaveFile",currentPlaybackTime);
			}


		
		}
	}


	public void updateObjTransforms()
	{//Because we only record at X (usually 30) frames a second, we need to interpolate between points to create smooth motion for the playback system.
		float lerpAmount =( currentPlaybackTime - previousFrame.RT )/ frameTime;

		leftHand.transform.position = Vector3.Lerp (previousFrame.lP, nextFrame.lP, lerpAmount);
		leftHand.transform.rotation = Quaternion.Slerp (previousFrame.lQ, nextFrame.lQ, lerpAmount);

		rightHand.transform.position = Vector3.Lerp (previousFrame.rP, nextFrame.rP, lerpAmount);
		rightHand.transform.rotation = Quaternion.Slerp (previousFrame.rQ, nextFrame.rQ, lerpAmount);

		Head.transform.position = Vector3.Lerp (previousFrame.hP, nextFrame.hP, lerpAmount);
		Head.transform.rotation = Quaternion.Slerp (previousFrame.hQ, nextFrame.hQ, lerpAmount);
	}


	public void play(RecordingSystem.SavedData sData)
	{

		clearPlayBack ();
		currentState = new RegularPlayState ();
		frameTime = 1 / sData.frameRate;
		GameObject.FindObjectOfType<Recorder> ().Play ("testSaveFile");
		savedData = sData;

		previousFrame = savedData.savedFrames [0];
		nextFrame = savedData.savedFrames[1];
		currentPlaybackTime = savedData.savedFrames [0].RT;

		countDown.color = Color.green;
		countDown.enabled = true;

		foreach (RecordingSystem.interactedObject rio in sData.interObjects) {
			GameObject foundTHingy = GameObject.Find (rio.ob);

			GameObject newObj = (GameObject)Instantiate (foundTHingy, rio.or + new Vector3 (.0000001f,.0000001f,.0000001f), rio.Rot);
			newObj.tag = "SceneObject";
			newObj.layer = 10;
		}

		foreach (PlayBackInputController pbic in playControllers) {
			foreach (ITool currentTool in pbic.myTools) {
				if (currentTool is NewMenuSelector) {
					((NewMenuSelector)currentTool).setMenuIndex (sData.startingMenuIndex);
					((NewMenuSelector)currentTool).setMenuOnOff (sData.menuOn);
				}
			}
		}

		StartCoroutine (setIKRig ());
	}


	//Calling this is causing weird glitching rig issues.
	IEnumerator setIKRig(){
		yield return new WaitForSeconds (.05f);
		foreach (IKControl ik in GameObject.FindObjectsOfType<IKControl>()) {
		ik.OnAnimatorIK ();
		}
	}



	//Reset the scene so all playback items are moved/removed
	public void clearPlayBack()
	{
		GameObject.FindObjectOfType<Recorder> ().Pause ();

		currentState = null;
	
		//Delete any objects from previous recordings
		foreach (GameObject oldStuff in GameObject.FindGameObjectsWithTag("SceneObject")) {

			if (oldStuff != null) {
				if (oldStuff.gameObject.layer == 10 ) {
					Destroy (oldStuff.gameObject);
				}
			}
		}

		foreach (GameObject oldStuff in GameObject.FindGameObjectsWithTag("Measurement")) {

			if (oldStuff != null) {
				if (oldStuff.gameObject.layer == 10 ) {
					Destroy (oldStuff.gameObject);
				}
			}
		}

		leftHand.transform.position = new Vector3( -1000, -1000, -1000);
		rightHand.transform.position = new Vector3( -1000, -1000, -1000);
		Head.transform.position =new Vector3( -1000, -1000, -1000);

	
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

				foreach (RecordingSystem.ActionClick ea in nextFrame.myC) {
					foreach (PlayBackInputController pbic in playControllers) {
						
						StartCoroutine(PerformAction(pbic, ea));
					}
				}
				currentFrameIndex = savedData.savedFrames.IndexOf (previousFrame);
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


				foreach (RecordingSystem.ActionClick ea in nextFrame.myC) {
					foreach (PlayBackInputController pbic in playControllers) {
						
						StartCoroutine(PerformBackwardsAction(pbic, ea));
					}
				}
				//IMPLEMENT UNDO ACTIONS FOR ALL TOOLS!
			}
			currentFrameIndex = savedData.savedFrames.IndexOf (nextFrame);
		}



		// THIS PART HANDLES THINGS YOU MOVED WHILE USING THE CUTTING PLANE AND AXIS ROTATION TOOLS
		foreach (RecordingSystem.movingObject mo in  savedData.movingObject) {

				if (mo.sFrame == currentFrameIndex || mo.EFrame == currentFrameIndex) {

					foreach (GameObject potential in GameObject.FindGameObjectsWithTag("SceneObject")) {

						if (potential.gameObject.layer == 10 ) {
						// THIS NEEDS TO BE FIXED IF THERE ARE MULTIPLE OBJECTS WITH THE SAME NAME!!!!!
							if (potential.name.Contains (mo.name)) {
								Debug.Log ("Setting it to "+ potential.name);
								mo.myObj = potential;
								break;
								}
							}
						}
					}


		
			if (mo.sFrame < currentFrameIndex && mo.EFrame > currentFrameIndex) {

				mo.myObj.transform.position = mo.myTrans [currentFrameIndex - mo.sFrame].or;
				mo.myObj.transform.rotation = mo.myTrans [currentFrameIndex - mo.sFrame].Rot;
			}
		
		}

		updateObjTransforms ();
	}


	IEnumerator PerformAction(PlayBackInputController pbic, RecordingSystem.ActionClick ac)
	{
		yield return new WaitForSeconds (ac.actionTime - currentPlaybackTime);
		pbic.callButtonAction(ac.myAction, true);
	}

	//Used for rewinding
	IEnumerator PerformBackwardsAction(PlayBackInputController pbic, RecordingSystem.ActionClick ac)
	{
		yield return new WaitForSeconds (currentPlaybackTime -ac.actionTime);
		pbic.callButtonAction(ac.myAction, false);
	}


	public void stopPlayback (){
		currentState = null;
		savedData = null;
	}





	// STATE MACHINE STATES FOR PLAY BACK

	public class RegularPlayState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{


			pbs.PlayIt (1);

		}
	}



	public class PauseState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{

		}
	}


	public class FastForwardState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{
			pbs.PlayIt (pbs.fastMultiplier);
		}

	}

	public class RewindState : PlayBackState
	{
		public void update(PlayBackSystem pbs)
		{	
			pbs.PlayIt (-1 * pbs.fastMultiplier);
		}

	}


}



public interface PlayBackState{

	void update(PlayBackSystem pbs);

}






using UnityEngine;
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
			//Debug.Log ("Updating " + currentState);

			currentState.update (this);

			countDown.text = "" + TimeConverter.getClockTime (currentPlaybackTime);
		}
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
			} else {
				currentState = new RegularPlayState ();
				ErrorPrompt.instance.showError ("Play");
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
		GameObject.FindObjectOfType<Recorder> ().Play ();
		savedData = sData;

		previousFrame = savedData.savedFrames [0];
		nextFrame = savedData.savedFrames[1];
		currentPlaybackTime = savedData.savedFrames [0].RT;

		countDown.color = Color.green;
		countDown.enabled = true;



		foreach (RecordingSystem.interactedObject rio in sData.interObjects) {
			GameObject foundTHingy = GameObject.Find (rio.obj);

			Debug.Log ("Searched for " + rio.obj + "   " + foundTHingy);
			GameObject newObj = (GameObject)Instantiate (foundTHingy, rio.origin + new Vector3 (.0000001f,.0000001f,.0000001f), rio.originRotation);
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


	}

	//Reset the scene so all playback items are moved/removed
	public void clearPlayBack()
	{
		currentState = null;
		Debug.Log ("Starting to clear");
		//Delete any objects from previous recordings
		foreach (GameObject oldStuff in GameObject.FindGameObjectsWithTag("SceneObject")) {
			Debug.Log ("Checking " + oldStuff.gameObject);
			if (oldStuff != null) {
				if (oldStuff.gameObject.layer == 10 ) {
					Debug.Log ("Deleting " + oldStuff.gameObject);
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
					//	Debug.Log ("I clicked " + ea.controllerIndex + "   " + ea.butClicked + "   " + pbic);
						StartCoroutine(PerformAction(pbic, ea));
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

	

			}
		}


		updateObjTransforms ();
	}


	IEnumerator PerformAction(PlayBackInputController pbic, RecordingSystem.ActionClick ac)
	{
		yield return new WaitForSeconds (ac.actionTime - currentPlaybackTime);
		pbic.callButtonAction(ac.myAction);
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






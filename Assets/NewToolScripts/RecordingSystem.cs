using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;


using UnityEngine.Networking;
using Valve.VR;


//CREATED BY MATTHEW JOHNSON ON 10-29-16: PROTOTYPE SYSTEM FOR RECORDING THE TRANSFORMS OF YOUR OBJECTS AND THE BUTTONS CLICKED


public class RecordingSystem : ITool{

	[Serializable]
	public struct ActionClick{

		public ActionClick(ClickedEventArgs cea, float time)
		{
			myAction = cea;
			actionTime = time;
		}

		public ClickedEventArgs myAction;
		public float actionTime;
	}



	// Records the position of the head/hands at a single moment in time, plus all of the button events that are clicked between this and the next
	[Serializable]
	public class movementFrame{
		//l == left   These have been abreviated to save on file storage space.
		//r = right
		//h = head
		//P = position
		//Q= quaternion

		public float RT; //recording Time

		public Vector3 hP;
		public Quaternion hQ;

		public Vector3 lP;
		public Quaternion lQ;

		public Vector3 rP;
		public Quaternion rQ;

		public List<ActionClick> myC; // My clicks
		public List<interactedObject> inter;

		public void changeTime(float t)
		{
			RT += t;}


		public void setTime(float t)
		{
			RT = t;}

	}
		

	//Keep track of all objects that have been touched so we can put them back when we replay
	[Serializable]
	public struct interactedObject{

		public string ob;
		public Vector3 or;
		public Quaternion Rot;

	}
		
	//USed for things that move but cannot properly be done so through inverse commands.
	[Serializable]
	public class movingObject{

		public string name;
		public float sTime;
		public float eTime;
		public int sFrame;
		public int EFrame;

		public Vector3 SP;
		public Quaternion SR;
		public Vector3 EP;
		public Quaternion ER;
		public List<TransformData> myTrans = new List<TransformData>();
		public GameObject myObj;

	}

	[Serializable]
	public struct TransformData{

		public Vector3 or;
		public Quaternion Rot;

	}

	// a container for all of the frames for a given action sequance (shot)
	[Serializable]
	public class SavedData{

		public int startingMenuIndex;
		public bool menuOn;

		public SavedData(int frameR)
		{frameRate = frameR;}

		public int frameRate;
		public List<movementFrame> savedFrames = new List<movementFrame>();
		public List<interactedObject> interObjects = new List<interactedObject> ();

		//these exist as subsequances for objects that are only moved for a short time.
		public List<movingObject> movingObject = new List<movingObject> ();

		public void addObject(GameObject obj)
		{
			foreach (interactedObject io in interObjects) {
				if (io.ob == obj.name) { // We have already moved this object, so its already saved;
					return;}
			
			}

			interactedObject newIO = new interactedObject ();
			newIO.ob = obj.name;
			newIO.or = obj.transform.position;
			newIO.Rot = obj.transform.rotation;
			interObjects.Add (newIO);
		
		}
		// should be calling this roughly 30 frames a second, while recording
		public void addFrame(movementFrame mFrame)
		{
			savedFrames.Add (mFrame);
		}

		//A controller button was pressed, Record it
		public void addAction(ClickedEventArgs ea, float t)
		{
			if (savedFrames.Count > 0) {
				savedFrames [savedFrames.Count - 1].myC.Add (new ActionClick (ea,t));
			}
		}

		public movementFrame getNextFrame(movementFrame mf)
		{
			int x = savedFrames.IndexOf (mf);
			if (x < savedFrames.Count - 1) {
				return savedFrames [x + 1];
			} else {
				return mf;}
		}

		public movementFrame getGetPrevFrame(movementFrame mf)
		{
			int x = savedFrames.IndexOf (mf);
			if (x >0 ) {
				return savedFrames [x - 1];
			} else {
				return mf;}

		}
		// used for gettign what action type a pad down was when rewinding
	}

	//I touched an object, mark it so we can make a copy on playback!
	public void recordInteractedObj(GameObject obj)
	{
		if (isRecording) {
			myData.addObject (obj);
		}
	}


	List<movingObject> currentlyTracking = new List<movingObject> ();


	//================================================================================================================
	// These two methods should be called from continuous indistinct modifer tools, like the cutting plane pad and axis rotation pad commands.
	// and commands where the previous location of a give nthing is not known/ recorded
	public void trackObject(GameObject obj)
	{
		if (isRecording) {

			movingObject mo = new movingObject ();
			mo.sFrame = myData.savedFrames.Count - 1;
			mo.name = obj.name;
			mo.SP = obj.transform.position;
			mo.SR = obj.transform.rotation;
			mo.myObj = obj;
			mo.sTime = recordingTime;

			currentlyTracking.Add (mo);
			myData.movingObject.Add (mo);
		}

	}

	public void stopTrackObject(GameObject obj)
	{
		if (isRecording) {
			movingObject theMO = null;
			foreach (movingObject mo in currentlyTracking) {
		
				if (mo.myObj == obj && mo.EFrame == 0) {
					mo.EFrame = myData.savedFrames.Count - 1;
					mo.EP = obj.transform.position;
					mo.ER = obj.transform.rotation;
					mo.eTime = recordingTime;
					currentlyTracking.Remove (mo);
					theMO = mo;
					break;
				}
			}
			currentlyTracking.Remove (theMO);
		}
	}
	//================================================================================================================


	public movementFrame getStartingAction(movementFrame mf, uint controllerIndex, Tools.button myButt)
	{
		for (int i = myData.savedFrames.LastIndexOf (mf); i > 0; i--) {
			foreach (ActionClick ac in  myData.savedFrames[i].myC) {
				if (ac.myAction.butClicked == myButt && ac.myAction.controllerIndex == controllerIndex) {
					return  myData.savedFrames [i];
				}
			}
		}
		return null;
	}

	public static ClickedEventArgs getActionFromFrame(movementFrame mf, uint controllerIndex, Tools.button myButt)
	{
		foreach (ActionClick ac in  mf.myC) {
			if (ac.myAction.butClicked == myButt && ac.myAction.controllerIndex == controllerIndex) {
				return  ac.myAction;
			}
		}
		return new ClickedEventArgs();
	}



	private Transform eyeC;
	private Transform leftC;
	private Transform rightC;
	PlayBackSystem myPlayBack;


	//This extenstion will be added onto your files for identification (like a .txt )
	string fileExtension = ".UVR";


	bool isRecording;


	public int frameRate = 30;
	float frameLength;        //Time between each frame
	float nextRecordingFrame; // next Real Time that a frame will be recorded

	float recordingTime; // Time since start of recording

	SavedData myData;


	protected SteamVR_TrackedController controllerRight;
	protected SteamVR_TrackedController controllerLeft;

	private float dist;

	//Keeps track of coroutine that has the countdown for start recording;
	Coroutine InDelayedRecording;

	Text countDown;

	void Start()
	{	countDown = GameObject.Find ("SecondToolText").GetComponent<Text> ();
		myPlayBack = GameObject.FindObjectOfType<PlayBackSystem> ();
		myData = new SavedData (frameRate);
		StartCoroutine ( delayedInitialize());

	}


	IEnumerator delayedInitialize()
	{// its delayed because its waiting for other scene objects to initialize
		yield return new WaitForSeconds(2);


		leftC = GameObject.Find("Controller (left)").transform;
		rightC = GameObject.Find("Controller (right)").transform;

		controllerLeft = leftC.GetComponent<SteamVR_TrackedController>();

		controllerRight = rightC.GetComponent<SteamVR_TrackedController>();


		eyeC = GameObject.Find("Camera (eye)").transform;


		frameLength = 1 / frameRate;


	}



	void Update()
	{
		if (isRecording) {
			recordingTime += Time.deltaTime;


			if (Time.time > nextRecordingFrame) {
				countDown.text = "Recording: " + TimeConverter.getClockTime(recordingTime) + "  \n(Trigger to Stop)";
				nextRecordingFrame = Time.time + frameLength;
				RecordFrame ();
			}
		}

	}


	public bool currentlyRecording()
	{
		return isRecording;
	}


	//Speeds up /Slows down up a section of a recording so you wont have to dothrough during playtime

	public void changeSpeedOfSection(SavedData theData,  float startTime, float endTime, float speedModifier )
	{

		if (speedModifier <= 0) {
			throw new Exception (" Cannot change the speed to be less than 0, that would cause time travel.");
		}

		if (startTime > endTime) {
			float temp = endTime;
			endTime = startTime;
			startTime = temp;

		}

		float totalTimeShift =(endTime - startTime) -  (endTime - startTime) * speedModifier; 
		
	
		for (int i = 0 ; i < theData.savedFrames.Count ; i++) {
			if (theData.savedFrames [i].RT > startTime && theData.savedFrames [i].RT < endTime) {
				theData.savedFrames [i].setTime( startTime + (theData.savedFrames[i].RT - startTime) * speedModifier); 


			} else if (theData.savedFrames [i].RT > endTime) {
				
				theData.savedFrames [i].changeTime(-totalTimeShift);
			}


		}

	}




	public void RecordFrame()
	{
		movementFrame mFrame = new movementFrame ();
		mFrame.myC = new List<ActionClick> ();
		System.Math.Round ((decimal)eyeC.position.x, 3);

		mFrame.hP = eyeC.position; 
		mFrame.hQ = eyeC.rotation;

		mFrame.lP = leftC.position;
		mFrame.lQ = leftC.rotation;

		mFrame.rP =rightC.position;
		mFrame.rQ = rightC.rotation;
		mFrame.RT = recordingTime; 

		foreach (movingObject obj in currentlyTracking) {
			TransformData td = new TransformData ();
			td.or = obj.myObj.transform.position;
			td.Rot = obj.myObj.transform.rotation;
			obj.myTrans.Add (td);
		}

		myData.addFrame (mFrame);

	}



	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal){
		if (playBackDevice) {
			return false;
		}
		if (myPlayBack.isPlaying()) {
			ErrorPrompt.instance.showError ( "Stop Playback First (Pad Up)");
			return true;
		}

		if (isRecording) {
		
			stopRecording ();
			saveToFile ();
		} else {
			InDelayedRecording = StartCoroutine (delayStartRecording ());
		}


		return true;}





	IEnumerator delayStartRecording()
	{	
		myData = new SavedData (frameRate);
		recordingTime = 0;
		myPlayBack.stopPlayback ();

		countDown.color = Color.green;
		countDown.enabled = true;
		countDown.fontSize = 20;
		countDown.text = "Starting in 3";

		yield return new WaitForSeconds (1);
		countDown.text = "Starting in 2";

		yield return new WaitForSeconds (1);
		countDown.text = "Starting in 1";

		yield return new WaitForSeconds (1);
		countDown.fontSize = 12;
		countDown.text = "Recording";
		countDown.color = Color.red;
		//AUDIO Recording
		GetComponent<Recorder> ().BeginRecording ();

		myData.startingMenuIndex = GetComponent<NewMenuSelector> ().getMenuIndex ();
		myData.menuOn = GetComponent<NewMenuSelector> ().isMenuOn ();
		foreach (ImprovedController ic in  GameObject.FindObjectsOfType<ImprovedController>()) {
			ic.setRecorder (this);

		}

		isRecording = true;


		nextRecordingFrame = Time.time + .00001f;
		recordingTime = 0;
		InDelayedRecording = null;

	}

	protected void stopRecording()
	{
		GetComponent<Recorder> ().EndRecord ("testSaveFile");
		countDown.enabled = false;
		isRecording = false;

		foreach (ImprovedController ic in  GameObject.FindObjectsOfType<ImprovedController>()) {
			ic.setRecorder (null);

		}
	}


	protected void saveToFile()
	{//Debug.Log ("Saving to file");

		isRecording = false;
		string json = JsonUtility.ToJson (myData);

		File.WriteAllText ("testSaveFile" + fileExtension, json);


	}


	public SavedData loadFromFile()
	{//Debug.Log ("loading from file");

		string path = "testSaveFile" + fileExtension;
		string info = File.ReadAllText (path);

		SavedData toReturn = JsonUtility.FromJson<SavedData> (info);
		return toReturn;
	}



	// Start playback
	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal){
		if (playBackDevice) {
			return false;
		}

		if (!isRecording && InDelayedRecording == null) {
			SavedData sd = loadFromFile ();
			if (sd != null) {
				myPlayBack.play (sd);
			} else {

				ErrorPrompt.instance.showError ("No Data Files Found");
			}
			return true;
		} else {
			ErrorPrompt.instance.showError ("Finish Recording First (Trigger)");
		}
		return false;
	}

	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}


	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){
		if (playBackDevice) {
			return false;
		}

		if (!isRecording) {

			//UP- Clear PlayBack
			if(e.padY > Mathf.Abs(e.padX)) {
				myPlayBack.clearPlayBack ();
				ErrorPrompt.instance.showError ("Play Back Closed");
				countDown.enabled = false;
			}

			//DPAD DOWN - PLay/Pause
			else if(e.padY < -1*Mathf.Abs(e.padX)) {
				myPlayBack.playPause ();

			}

		//DPAD LEFT - Rewind
			else if(e.padX <  -1 * Mathf.Abs(e.padY)) {
				myPlayBack.rewind ();

			}

		//DPAD RIGHT - Fast forward
			else if(e.padX > Mathf.Abs(e.padY)) {
				myPlayBack.fastForward ();
			}

			return true;
		}

		return false;
	}

	public void recordClick(ClickedEventArgs ea, bool TimeNormal)
	{
		if (isRecording) {
			myData.addAction (ea, recordingTime);
		}

	}

	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool Grip (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}








}

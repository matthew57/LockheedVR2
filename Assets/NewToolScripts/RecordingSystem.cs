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




	// Records the position of the head/hands at a single moment in time, plus all of the button events that are clicked between this and the next
	[Serializable]
	public struct movementFrame{
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
		public List<ClickedEventArgs> myC; // My clicks

		public void changeTime(float t)
		{
			RT += t;}


		public void setTime(float t)
		{
			RT = t;}

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

		public void addFrame(movementFrame mFrame)
		{
			savedFrames.Add (mFrame);
		}

		public void addAction(ClickedEventArgs ea)
		{
			if (savedFrames.Count > 0) {
				savedFrames [savedFrames.Count - 1].myC.Add (ea);
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

	Text countDown;

	void Start()
	{	countDown = GameObject.Find ("SecondToolText").GetComponent<Text> ();
		myPlayBack = GameObject.FindObjectOfType<PlayBackSystem> ();
		myData = new SavedData (frameRate);
		StartCoroutine ( delayedInitialize());

	}


	IEnumerator delayedInitialize()
	{// its delayed because its waiting for other scene objects to initialize
		yield return new WaitForSeconds(.1f);

		controllerLeft = GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>();

		controllerRight = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>();


		eyeC = GameObject.Find("Camera (eye)").transform;
		leftC = GameObject.Find("Controller (left)").transform;
		rightC = GameObject.Find("Controller (right)").transform;

		frameLength = 1 / frameRate;


	}




	protected void startRecordingIn()//object sender, ClickedEventArgs e)
	{
		StartCoroutine (delayStartRecording ());
			
	}



	IEnumerator delayStartRecording()
	{countDown.color = Color.green;
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


		//SubScribeControllers ();
		isRecording = true;


		nextRecordingFrame = Time.time + .00001f;
		recordingTime = 0;

	}

	protected void stopRecording()
	{
		GetComponent<Recorder> ().EndRecord ();
		countDown.enabled = false;
		isRecording = false;

		foreach (ImprovedController ic in  GameObject.FindObjectsOfType<ImprovedController>()) {
			ic.setRecorder (null);

		}

		//UnSubScribeControllers ();

	}

	void Update()
	{
 
		if (isRecording) {
			recordingTime += Time.deltaTime;


			if (Time.time > nextRecordingFrame) {


				countDown.text = "Recording: " + TimeConverter.getClockTime(recordingTime);
				nextRecordingFrame = Time.time + frameLength;
				RecordFrame ();
			}
		}

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
		mFrame.myC = new List<ClickedEventArgs> ();


		mFrame.hP = eyeC.position;
		mFrame.hQ = eyeC.rotation;

		mFrame.lP = leftC.position;
		mFrame.lQ = leftC.rotation;

		mFrame.rP = rightC.position;
		mFrame.rQ = rightC.rotation;
		mFrame.RT = recordingTime;
		myData.addFrame (mFrame);

	}







	protected void saveToFile()//object sender, ClickedEventArgs e)
	{Debug.Log ("Saving to file");

		isRecording = false;
		string json = JsonUtility.ToJson (myData);

		File.WriteAllText ("testSaveFile" + fileExtension, json);
	
	
	}


	public SavedData loadFromFile()
	{Debug.Log ("loading from file");

		string path = "testSaveFile" + fileExtension;
		string info = File.ReadAllText (path);
	
		SavedData toReturn = JsonUtility.FromJson<SavedData> (info);
		return toReturn;
	}




	public override bool TriggerClick(ClickedEventArgs e){
		if (isRecording) {
		
			stopRecording ();
			saveToFile ();
		} else {
			startRecordingIn ();
		}


		return true;}


	public override bool TriggerUnclick (ClickedEventArgs e){return false;}

	public override bool MenuClick (ClickedEventArgs e){
		if (!isRecording) {
			SavedData sd = loadFromFile ();
			if (sd != null) {
				myPlayBack.play (sd);
			}
			return true;
		}
		return false;
	}

	public override bool MenuUnclick (ClickedEventArgs e){return false;}


	public override bool PadClick (ClickedEventArgs e){

		if (!isRecording) {
			if (e.padX < -.33) {
				//Debug.Log ("Rwind");
				myPlayBack.rewind ();
			} else if (e.padX > .33) {
				//Debug.Log ("fast forward");
				myPlayBack.fastForward ();
			} else {
				//	Debug.Log ("play pause");
				myPlayBack.playPause ();
			}
			return true;
		}
		return false;
	}


	public override bool PadUnclick (ClickedEventArgs e){return false;}
	public override bool Grip (ClickedEventArgs e){return false;}
	public override bool UnGrip(ClickedEventArgs e){return false;}
	public override bool PadTouched(ClickedEventArgs e){return false;}
	public override bool PadUntouched(ClickedEventArgs e){return false;}
	public override bool SteamClicked (ClickedEventArgs e){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}



	//We might be able to get rid of all of these functions except for

	public void recordClick(ClickedEventArgs ea)
	{
		if (isRecording) {
			myData.addAction (ea);
		}
	
	}





}

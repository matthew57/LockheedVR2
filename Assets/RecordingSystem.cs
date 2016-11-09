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

	private Transform eyeC;
	private Transform leftC;
	private Transform rightC;



	//This extenstion will be added onto your files for identification (like a .txt )
	string fileExtension = ".UVR";


	bool isRecording;


	public int frameRate = 30;
	float frameLength;        //Time between each frame
	float nextRecordingFrame; // next Real Time that a frame will be recorded

	float recordingTime; // Time since start of recording




	SavedData myData = new SavedData ();


	protected SteamVR_TrackedController controllerRight;
	protected SteamVR_TrackedController controllerLeft;

	private float dist;

	Text countDown;

	void Start()
	{	countDown = GameObject.Find ("SecondToolText").GetComponent<Text> ();
		
		//record.clicked = startRecordingIn;
		//record.unclicked = null;

	
		//save.clicked = saveToFile;
		//save.unclicked = null;

		//buttonMap.Add(recordButton, record);
		//buttonMap.Add(saveButton, save);


	
		StartCoroutine ( delayedInitialize());

	}


	IEnumerator delayedInitialize()
	{// its delayed because its waiting for other scene objects to initialize
		yield return new WaitForSeconds(.1f);

	//	foreach (KeyValuePair<button, ButtonSubHandler> assignedButton in buttonMap) {
		//	Debug.Log ("XXXXXXXXXXXXXXX" + assignedButton.Key + "   " + assignedButton.Value.clicked);
		//}

		controllerLeft = GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>();

		controllerRight = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>();


		eyeC = GameObject.Find("Camera (eye)").transform;
		leftC = GameObject.Find("Controller (left)").transform;
		rightC = GameObject.Find("Controller (right)").transform;

		frameLength = 1 / frameRate;


	}




	protected void startRecordingIn()//object sender, ClickedEventArgs e)
	{Debug.Log ("Start recording in " +3);
		StartCoroutine (delayStartRecording (3));
			
	}



	IEnumerator delayStartRecording(float timeBeforeStart)
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



		//SubScribeControllers ();
		isRecording = true;


		nextRecordingFrame = Time.time + .00001f;
		recordingTime = 0;

	}

	protected void stopRecording()
	{

		countDown.enabled = false;
		isRecording = false;
		//UnSubScribeControllers ();

	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			
			isRecording = !isRecording;
			if (isRecording) {
				startRecordingIn ();

			} else {
				stopRecording ();
			}
		
		}

		if (Input.GetKeyDown (KeyCode.G)) {

			saveToFile ();

		}

		if (Input.GetKeyDown (KeyCode.R)) {
			SavedData sd = loadFromFile ();
			GameObject.FindObjectOfType<PlayBackSystem> ().play (sd);

		}


		if (!isRecording && Input.GetKeyDown (KeyCode.A)) {
			GameObject.FindObjectOfType<PlayBackSystem> ().play (myData);
		
		}
		if (isRecording) {
			recordingTime += Time.deltaTime;



			if (Time.time > nextRecordingFrame) {


				nextRecordingFrame = Time.time + frameLength;
				RecordFrame ();
			}
		}

	}


	//WARNING : CALLING THIS WILL CUT OUT ALL ACTIONS DONE IN THIS TIME FRAME WHICH COULD TOTALLY BREAK STUFF!
	//Im not quite sure why I am writing this! COnsider using changeSpeedOfSection Instead!
	public void cutFrames(SavedData theData,  float startTime, float endTime)
	{

		float timeChange = endTime - startTime;
		for (int i = theData.savedFrames.Count - 1; i > -1; i--) {
			if (theData.savedFrames [i].RT > startTime && theData.savedFrames [i].RT < endTime) {
				theData.savedFrames.Remove (theData.savedFrames [i]);
			
			} else if (theData.savedFrames [i].RT > endTime) {
				theData.savedFrames [i].changeTime (-timeChange);
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

		public List<movementFrame> savedFrames = new List<movementFrame>();

		public void addFrame(movementFrame mFrame)
		{
			savedFrames.Add (mFrame);
		}

		public void addAction(ClickedEventArgs ea)
		{
			savedFrames [savedFrames.Count - 1].myC.Add (ea);
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

	private void SubScribeControllers ()
	{/*
		controllerLeft.TriggerClicked += ControllerLeft_TriggerClicked;
		controllerLeft.TriggerUnclicked += ControllerLeft_TriggerUnclicked;
		controllerLeft.MenuButtonClicked += ControllerLeft_MenuButtonClicked;
		controllerLeft.MenuButtonUnclicked += ControllerLeft_MenuButtonUnclicked;
		controllerLeft.Gripped += ControllerLeft_Gripped;
		controllerLeft.Ungripped += ControllerLeft_Ungripped;
		controllerLeft.PadClicked += ControllerLeft_PadClicked;
		controllerLeft.PadUnclicked += ControllerLeft_PadUnclicked;

		controllerRight.TriggerClicked += ControllerRight_TriggerClicked;
		controllerRight.TriggerUnclicked += ControllerRight_TriggerUnclicked;
		controllerRight.MenuButtonClicked += ControllerRight_MenuButtonClicked;
		controllerRight.MenuButtonUnclicked += ControllerRight_MenuButtonUnclicked;
		controllerRight.Gripped += ControllerRight_Gripped;
		controllerRight.Ungripped += ControllerRight_Ungripped;
		controllerRight.PadClicked += ControllerRight_PadClicked;
		controllerRight.PadUnclicked += ControllerRight_PadUnclicked;
*/

	}


	private void UnSubScribeControllers ()
	{/*
		controllerLeft.TriggerClicked -= ControllerLeft_TriggerClicked;
		controllerLeft.TriggerUnclicked -= ControllerLeft_TriggerUnclicked;
		controllerLeft.MenuButtonClicked -= ControllerLeft_MenuButtonClicked;
		controllerLeft.MenuButtonUnclicked -= ControllerLeft_MenuButtonUnclicked;
		controllerLeft.Gripped -= ControllerLeft_Gripped;
		controllerLeft.Ungripped -= ControllerLeft_Ungripped;
		controllerLeft.PadClicked -= ControllerLeft_PadClicked;
		controllerLeft.PadUnclicked -= ControllerLeft_PadUnclicked;

		controllerRight.TriggerClicked -= ControllerRight_TriggerClicked;
		controllerRight.TriggerUnclicked -= ControllerRight_TriggerUnclicked;
		controllerRight.MenuButtonClicked -= ControllerRight_MenuButtonClicked;
		controllerRight.MenuButtonUnclicked -= ControllerRight_MenuButtonUnclicked;
		controllerRight.Gripped -= ControllerRight_Gripped;
		controllerRight.Ungripped -= ControllerRight_Ungripped;
		controllerRight.PadClicked -= ControllerRight_PadClicked;
		controllerRight.PadUnclicked -= ControllerRight_PadUnclicked;
*/

	}


	public override bool TriggerClick(ClickedEventArgs e){
		if (isRecording) {
		
			stopRecording ();
		} else {
			startRecordingIn ();
		}


		return true;}


	public override bool TriggerUnclick (ClickedEventArgs e){return false;}
	public override bool MenuClick (ClickedEventArgs e){

		SavedData sd = loadFromFile ();
		GameObject.FindObjectOfType<PlayBackSystem> ().play (sd);
		return true;
	}
	public override bool MenuUnclick (ClickedEventArgs e){return false;}
	public override bool PadClick (ClickedEventArgs e){
		saveToFile ();

		return true;}
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


	void ControllerLeft_PadUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_PadClicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_Ungripped (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_Gripped (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_MenuButtonUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_MenuButtonClicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);

	}

	void ControllerLeft_TriggerUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerLeft_TriggerClicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}






	void ControllerRight_PadUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);

	}

	void ControllerRight_PadClicked (object sender, ClickedEventArgs e)
	{
		if (isRecording) {
			recordClick (e);

		}
	}

	void ControllerRight_Ungripped (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerRight_Gripped (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerRight_MenuButtonUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerRight_MenuButtonClicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerRight_TriggerUnclicked (object sender, ClickedEventArgs e)
	{
			recordClick (e);
	}

	void ControllerRight_TriggerClicked (object sender, ClickedEventArgs e)
	{	
			recordClick (e);

	}











}

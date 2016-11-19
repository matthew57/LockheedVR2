using UnityEngine;
using System.Collections;

public class controllerIDMatcher : MonoBehaviour {


	public ImprovedController myController;


	// Use this for initialization
	void Start () {
		StartCoroutine (DelayedWait());
	
	}



	// FIX THIS LATER

	IEnumerator DelayedWait()
	{
		while (true) {
			yield return new WaitForSeconds (1);
			GetComponent<PlayBackInputController> ().controllerIndex = myController.controllerIndex;

		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

	Button play;
	// Use this for initialization
	void Start () {
		play = GameObject.Find("MenuItem180Shooter").GetComponent<Button>();
		Debug.Log ("assigned");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

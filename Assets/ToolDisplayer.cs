using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ToolDisplayer : MonoBehaviour {



	[Serializable]
	public struct ToolDisplayInfo
	{
		public string padT;
		public string MenuT;
		public string GripT;
		public string TriggerT;
	}


	public Text PadText;
	public Text MenuText;
	public Text GripText;
	public Text TriggerText;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void loadDescription(ToolDisplayInfo info)
	{
		PadText.text = info.padT;
		MenuText.text = info.MenuT;
		GripText.text = info.GripT;
		TriggerText.text = info.TriggerT;
	}


}

using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject optionsMenu;
	public GameObject sceneMenu;

	void Start() {
		mainMenu.SetActive (false);
		sceneMenu.SetActive (true);
	}

	public void ProcessSelection(string selectionTag) {
		switch (selectionTag) {
		case "Options":
			mainMenu.SetActive (false);
			optionsMenu.SetActive (true);
			break;
		case "Scenes":
			mainMenu.SetActive (false);
			sceneMenu.SetActive (true);
			break;
		case "Back":
			optionsMenu.SetActive (false);
			sceneMenu.SetActive (false);
			mainMenu.SetActive (true);
			break;
		default:
			break;
		}
	}
}

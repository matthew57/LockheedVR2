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
}

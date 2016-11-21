using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class ErrorPrompt : MonoBehaviour {

	public static ErrorPrompt instance;



	public Text ErrorBox;

	Coroutine currentError;

	void Start()
	{
		instance = this;
	}

	public void showError(string mess)
	{
		if (currentError != null) {
			StopCoroutine (currentError);
		}

		currentError = StartCoroutine (showMessage(mess));

	}




	IEnumerator showMessage(string message)
	{
		ErrorBox.enabled = true;
		ErrorBox.text = message;
		yield return new WaitForSeconds (4);

		ErrorBox.enabled = false;



	}


}

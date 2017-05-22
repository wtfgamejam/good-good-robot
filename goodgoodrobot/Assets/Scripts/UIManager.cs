using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

	ComputerDisplay computerDisplay;
	[SerializeField] Text objectLabel;

	public void DisplayText(string text)
	{
		Debug.Log("Display Text: " + text);
		if (computerDisplay == null) {
			computerDisplay = FindObjectOfType<ComputerDisplay> ();
		}

		if(computerDisplay)
			computerDisplay.DisplayText (text);
	}

	public void DisplaySenderName(s3DBButton_sender sender)
	{
		if (objectLabel) {
			objectLabel.canvas.transform.position = sender.gameObject.transform.position;

			objectLabel.text = sender.GetName ();
			objectLabel.transform.LookAt (Camera.main.transform);
		}
	}
}

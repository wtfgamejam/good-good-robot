using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

	ComputerDisplay computerDisplay;
	[SerializeField] Text objectLabel;
	float labelDistance = 0.5f;

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
			objectLabel.text = sender.GetName ();

			objectLabel.canvas.transform.position = sender.transform.position;
			Transform c = Camera.main.transform;

			objectLabel.canvas.transform.LookAt( c.position );
			Vector3 rot = objectLabel.canvas.transform.eulerAngles;
			rot.x = 0;
			rot.y += 180;
			objectLabel.canvas.transform.eulerAngles = rot;
		}
	}
}

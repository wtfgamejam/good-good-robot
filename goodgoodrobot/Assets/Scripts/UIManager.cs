using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

	ComputerDisplay computerDisplay;
	[SerializeField] Text objectLabel;

	void Start()
	{
		computerDisplay = FindObjectOfType<ComputerDisplay> ();
	}

	public void DisplayText(string text)
	{
		if(computerDisplay)
			computerDisplay.DisplayText (text);
	}

	public void DisplaySenderName(s3DBButton_sender sender)
	{
		if (objectLabel) {
			objectLabel.canvas.transform.position = sender.gameObject.transform.position;

			objectLabel.text = sender.GetName ();
		}
	}
}

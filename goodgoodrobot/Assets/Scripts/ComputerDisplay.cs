using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerDisplay : MonoBehaviour {
	Text screenText;
	List<string> textLines = new List<string>();
	public int maxLines = 10;
	void Awake()
	{
		screenText = GetComponentInChildren<Text> ();
	}

	public void DisplayText(string text)
	{
		textLines.Add (text);
		if (textLines.Count > maxLines) {
			textLines.RemoveAt (0);
		}

		UpdateDisplay ();
	}

	void UpdateDisplay()
	{
		string t = "";
		for (int i = 0; i < textLines.Count; i++) {
			t += textLines [i] + "\n";
		}
		if (screenText != null) {
			screenText.text = t;
		}
	}
}

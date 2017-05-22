using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerDisplay : MonoBehaviour {
	Text screenText;

	void Awake()
	{
		screenText = GetComponentInChildren<Text> ();
	}

	public void DisplayText(string text)
	{
		if (screenText != null) {
			screenText.text = text;
		}
	}

}

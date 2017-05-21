using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
	string[] names = { "Bob", "Steve", "Gunter", "Sam", "Vent" };
	s3DBButton_sender[] objects;
	public enum RoundState
	{
		Setup,
		Playing,
		Success,
		Failure,
		GameOver
	}
	RoundState currentState;
	int currentObject = 0;

	IEnumerator sayCoroutine;
	Text objectLabel;
	// Use this for initialization
	void Start () {
		objects = FindObjectsOfType<s3DBButton_sender> ();
		objectLabel = FindObjectOfType<Text> ();
		for (int i = 0; i < objects.Length; i++) {
			objects[i].SetName(names[Random.Range(0,names.Length - 1)] + i);
		}

		currentState = RoundState.Setup;
		StartCoroutine (Round ());
	}

	public void Init()
	{
		UnityEngine.Debug.Log ("Init");
		// I guess put game startup things here
	}

	IEnumerator Round()
	{
		while (currentState != RoundState.GameOver) {
			switch (currentState) {
			case RoundState.Setup:
				currentObject = Random.Range (0, objects.Length - 1);
				currentState = RoundState.Playing;
				if(CheckObjects())
					Say ("Interact with " + objects [currentObject].GetName ());
				break;
			case RoundState.Playing:

				break;
			case RoundState.Success:
				Say ("Success");
				currentState = RoundState.Setup;
				break;
			case RoundState.Failure:
				Say ("Failure");

				currentState = RoundState.GameOver;
				break;
			}

			yield return null;
		}

		UnityEngine.Debug.Log ("Game Over");
	}

	bool CheckObjects()
	{
		return objects != null && objects.Length > 0;
	}

	// See if the sender is the object we want to interact with
	public void CheckMessage(s3DBButton_sender sender)
	{
		if (currentState == RoundState.Playing) {
			if (sender.gameObject == objects [currentObject].gameObject) {
				currentState = RoundState.Success;
			} else {
				UnityEngine.Debug.Log ("Nope " + sender.GetName() + " find " + objects [currentObject].GetName());
			}
		}
	}

	public void DisplayName(string name)
	{
		if(objectLabel != null)
			objectLabel.text = name;
	}

	private void Say(string command)
	{
		if (sayCoroutine != null) {
			StopCoroutine (sayCoroutine);
		}
		sayCoroutine = Speak (command);
		StartCoroutine (sayCoroutine);
		UnityEngine.Debug.Log("Say: " + command); 
	}

	private IEnumerator Speak (string command){
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash");
		startInfo.WorkingDirectory = "/";
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardInput = true;
		startInfo.RedirectStandardOutput = true;

		Process process = new Process();
		process.StartInfo = startInfo;
		process.Start();

		process.StandardInput.WriteLine("say " + command);
		process.StandardInput.WriteLine("exit");  // if no exit then WaitForExit will lockup your program
		process.StandardInput.Flush();

		process.StandardOutput.ReadLine();

		process.WaitForExit();
#endif
		yield return null;
	}

	public s3DBButton_sender GetCurrentTarget()
	{
		if (CheckObjects()) {
			return objects [currentObject];
		} 

		return null;
	}
}

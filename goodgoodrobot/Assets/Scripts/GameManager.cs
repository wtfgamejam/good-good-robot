using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
	string[] names = { "Bob", "Steve", "Gunter", "Sam", "Vent" };
	List<s3DBButton_sender> objectPool = new List<s3DBButton_sender>();

	InteractionPanel[] panels;
	int panelIndex = 0;

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
		objectLabel = FindObjectOfType<Text> ();
		Init ();

		currentState = RoundState.Setup;
		StartCoroutine (Round ());
	}

	public void Init()
	{
		// Load up the GameObject Components we need
		objectPool = new List<s3DBButton_sender>();
		panels = FindObjectsOfType<InteractionPanel> ();

		AddObjectFromPanels ();
	}

	void AddObjectFromPanels()
	{
		if (panels.Length - 1 >= panelIndex) {
			s3DBButton_sender[] objects = panels [panelIndex].objects;
			for (int i = 0; i < objects.Length; i++) {
				objects[i].SetName(names[Random.Range(0,names.Length - 1)] + i);
			}
			objectPool.AddRange (objects);
		}
		panelIndex++;
	}

	IEnumerator Round()
	{
		while (currentState != RoundState.GameOver) {
			switch (currentState) {
			case RoundState.Setup: // Set up a new Round
				currentObject = Random.Range (0, objectPool.Count - 1);
				currentState = RoundState.Playing;
				if(CheckObjects())
					Say ("Interact with " + objectPool [currentObject].GetName ());
				break;
			case RoundState.Playing:

				break;
			case RoundState.Success:
				//Say ("Success");
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
		return objectPool != null && objectPool.Count > 0;
	}

	// See if the sender is the object we want to interact with
	public void CheckMessage(s3DBButton_sender sender)
	{
		if (currentState == RoundState.Playing) {
			if (sender.gameObject == objectPool [currentObject].gameObject) {
				currentState = RoundState.Success;
			} else {
				UnityEngine.Debug.Log ("Nope " + sender.GetName() + " find " + objectPool [currentObject].GetName());
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
			return objectPool [currentObject];
		} 

		return null;
	}
}

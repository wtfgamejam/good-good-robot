using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIManager))]
public class GameManager : Singleton<GameManager> {
	string[] names = { "Bob", "Steve", "Gunter", "Sam", "Vent" };
	int[] successRequirements = { 3, 1 };
	int[] panelsPerRound = { 1, 2 };
	int round = 0;
	int successCount = 0;
	List<s3DBButton_sender> objectPool = new List<s3DBButton_sender>();
	ComputerDisplay display;
	InteractionPanel[] panels;
	int panelIndex = 0;

	public enum RoundState
	{
		StartRound,
		NextRound,
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
	void Awake () {
		UnityEngine.Debug.Log ("GM object " + gameObject.name); 
		objectLabel = FindObjectOfType<Text> ();
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		SceneManager.LoadScene ("macOSPlayer", LoadSceneMode.Additive);
#else
		SceneManager.LoadScene ("vivePlayer", LoadSceneMode.Additive);
#endif

		Init ();

		currentState = RoundState.StartRound;
		StartCoroutine (Round ());
	}

	public void Init()
	{
		// Load up the GameObject Components we need
		objectPool = new List<s3DBButton_sender>();
		panels = FindObjectsOfType<InteractionPanel> ();
		round = 0;
		successCount = 0;
		AddObjectFromPanels ();
	}

	void AddObjectFromPanels()
	{
		if (panelsPerRound.Length <= round) {
			round = 0;
		}
		UnityEngine.Debug.Log ("AddObjects for Round " + round);
		objectPool.Clear ();
		if (panelsPerRound.Length > round) {
			panelIndex = panelsPerRound [round];
			UnityEngine.Debug.Log ("Panel count " + panels.Length + " panelIndex " + panelIndex);

			for (int i = 0; i < panelIndex && i < panels.Length ; i++) 
			{
				s3DBButton_sender[] objects = panels [i].objects;
				for (int j = 0; j < objects.Length; j++) {
					if(objects[j].GetName() == "")
						objects [j].SetName (names [Random.Range (0, names.Length - 1)] + j);	
				}
				objectPool.AddRange (objects);
			}
		}
	}

	IEnumerator Round()
	{
		while (currentState != RoundState.GameOver) {
			switch (currentState) {
			case RoundState.NextRound: // increment the round
				UnityEngine.Debug.Log("Next Round");
				round++;
				successCount = 0;
				AddObjectFromPanels ();
				currentState = RoundState.StartRound;
				break;
			case RoundState.StartRound: // Set up a new Round
				if (CheckObjects ()) {
					currentObject = Random.Range (0, objectPool.Count - 1);
					string objective = "Interact with " + objectPool [currentObject].GetName ();
					UIManager.Instance.DisplayText (objective);
					currentState = RoundState.Playing;
				} else {
					UnityEngine.Debug.Log ("GameManger didn't find any panels in the scene");
					currentState = RoundState.GameOver;
				}

				break;
			case RoundState.Playing: // if we want stuff to happen during normal play

				break;
			case RoundState.Success: // the player hit the target
				successCount++;
				UnityEngine.Debug.Log ("Success Count : " + successCount);
				if (successRequirements.Length > round && successCount >= successRequirements [round]) {
					currentState = RoundState.NextRound;
				} else {
					currentState = RoundState.StartRound;
				}
				break;
			case RoundState.Failure:

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
		if (currentState == RoundState.Playing && CheckObjects()) {
			if (sender.gameObject == objectPool [currentObject].gameObject) {
				currentState = RoundState.Success;
			} else {
				UnityEngine.Debug.Log ("Nope " + sender.GetName());
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

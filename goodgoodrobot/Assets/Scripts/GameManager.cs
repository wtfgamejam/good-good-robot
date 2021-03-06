﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIManager))]
public class GameManager : Singleton<GameManager> {
	int currentRound = 0;
	int successCount = 0;
	List<s3DBButton_sender> objectPool = new List<s3DBButton_sender>();
	List<string[]> names = new List<string[]> ();
	ComputerDisplay display;
	InteractionPanel[] panels;
	int panelIndex = 0;
	string macOSScene = "macOSPlayer";
	string viveScene = "vivePlayer";

	public RuntimeAnimatorController animatorController;
	public AudioClip successAudio;

	public enum RoundState
	{
		StartRound,
		NextRound,
		CompleteRound,
		Playing,
		Success,
		Failure,
		GameOver,
		Win
	}

	public struct RoundData
	{
		public int successRequirements;
		public int activePanels;
	}
	List<RoundData> rounds = new List<RoundData> ();

	RoundState currentState;
	int currentObject = 0;

	IEnumerator sayCoroutine;
	Text objectLabel;
	// Use this for initialization
	void Awake () {
		UnityEngine.Debug.Log ("GM object " + gameObject.name); 
		SceneManager.sceneLoaded += OnSceneLoaded;
		objectLabel = FindObjectOfType<Text> ();
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		SceneManager.LoadScene ("macOSPlayer", LoadSceneMode.Additive);
#else
		SceneManager.LoadScene ("vivePlayer", LoadSceneMode.Additive);
#endif
		UIManager.Instance.DisplayText ("Loading Scenes");
		names.Add(ShipSystems.engineSystems);
		names.Add(ShipSystems.lifeSupportSystems);
		names.Add(ShipSystems.sensorSystem);
		names.Add(ShipSystems.flightSystem);

		rounds.Add (new RoundData () { activePanels = 1, successRequirements = 3});
		rounds.Add (new RoundData () { activePanels = 2, successRequirements = 3});
		rounds.Add (new RoundData () { activePanels = 3, successRequirements = 5});
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UnityEngine.Debug.Log ("Scene loaded " + scene.name);
		if (scene.name == macOSScene || scene.name == viveScene) {
			GameObject player = GameObject.Find("Player");
			if(player)
			{
				player.transform.SetParent(transform);
			}
			Init ();

			currentState = RoundState.StartRound;
			StartCoroutine (Round ());
		}
	}

	public void Init()
	{
		// Load up the GameObject Components we need
		objectPool = new List<s3DBButton_sender>();
		panels = FindObjectsOfType<InteractionPanel> ();
		currentRound = 0;
		successCount = 0;
		AddObjectFromPanels ();
	}

	void AddObjectFromPanels()
	{
		UnityEngine.Debug.Log ("AddObjects for Round " + currentRound);
		objectPool.Clear ();

		panelIndex = rounds[currentRound].activePanels;
		UnityEngine.Debug.Log ("Panel count " + panels.Length + " panelIndex " + panelIndex);

		for (int i = 0; i < panelIndex && i < panels.Length ; i++) 
		{
			s3DBButton_sender[] objects = panels [i].objects;
			string[] potentialNames = names [i];
			for (int j = 0; j < objects.Length; j++) {
				if(objects[j].GetName() == "")
					objects [j].SetName (potentialNames [Random.Range (0, potentialNames.Length - 1)] + j);	
			}
			objectPool.AddRange (objects);
		}
	}

	IEnumerator Round()
	{
		while (currentState != RoundState.GameOver) {
			switch (currentState) {
			case RoundState.NextRound: // set up the round
				AddObjectFromPanels ();
				currentState = RoundState.StartRound;
				break;
			case RoundState.StartRound: // Set up a new Round
				if (CheckObjects ()) {
					currentObject = Random.Range (0, objectPool.Count - 1);
					string targetState = "";
					s3DBButton_sender.SenderState s = objectPool [currentObject].GetState ();
					if (s == s3DBButton_sender.SenderState.On) {
						targetState = "off";
					} else {
						targetState = "on";
					}

					string objective = "Turn " + objectPool [currentObject].GetName () + " " + targetState;
					UIManager.Instance.DisplayText (objective);
					currentState = RoundState.Playing;
				} else {
					UnityEngine.Debug.Log ("GameManger didn't find any panels in the scene");
					currentState = RoundState.GameOver;
				}

				break;
			case RoundState.CompleteRound:
				currentRound++;
				successCount = 0;
				if (rounds.Count <= currentRound) {
					currentState = RoundState.Win;
				} else {
					currentState = RoundState.NextRound;
				}
				break;
			case RoundState.Playing: // if we want stuff to happen during normal play

				break;
			case RoundState.Success: // the player hit the target
				successCount++;
				if (rounds [currentRound].successRequirements <= successCount) {
					currentState = RoundState.CompleteRound;
				} else {
					currentState = RoundState.StartRound;
				}
				break;
			case RoundState.Failure:

				currentState = RoundState.GameOver;
				break;
			case RoundState.Win:
				
				currentState = RoundState.GameOver;
				break;
			}

			yield return null;
		}

		SceneManager.LoadScene ("Win", LoadSceneMode.Single);
	}

	bool CheckObjects()
	{
		return objectPool != null && objectPool.Count > 0;
	}

	// See if the sender is the object we want to interact with
	public void CheckMessage(s3DBButton_sender sender)
	{
		if (currentState == RoundState.Playing && CheckObjects()) {
			if (5 == Random.Range (1, 6)) {
				StoryEvents evs = GetComponent<StoryEvents> ();
				if (evs != null) {
					evs.TriggerEvent ();
				}
			}

			if (sender.gameObject == objectPool [currentObject].gameObject) {
				AudioSource source = GetComponent<AudioSource> ();
				if (source != null && successAudio != null) {
					source.PlayOneShot (successAudio);
				}
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

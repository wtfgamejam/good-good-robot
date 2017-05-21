using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private static GameManager _instance;

	public static GameManager Instance { get { return _instance; } }
	string[] names = { "Bob", "Steve", "Gunter" };
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

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

	// Use this for initialization
	void Start () {
		objects = FindObjectsOfType<s3DBButton_sender> ();
		for (int i = 0; i < objects.Length; i++) {
			objects[i].SetName(names[Random.Range(0,names.Length - 1)] + i);
		}

		currentState = RoundState.Setup;
		StartCoroutine (Round ());
	}

	IEnumerator Round()
	{
		while (currentState != RoundState.GameOver) {
			switch (currentState) {
			case RoundState.Setup:
				currentObject = Random.Range (0, objects.Length - 1);
				Debug.Log ("Object " + objects [currentObject].GetName());
				currentState = RoundState.Playing;
				break;
			case RoundState.Playing:
			
				break;
			case RoundState.Success:
				Debug.Log ("Success");
				currentState = RoundState.Setup;
				break;
			case RoundState.Failure:
				Debug.Log ("Failure");
				currentState = RoundState.GameOver;
				break;
			}

			yield return null;
		}

		Debug.Log ("Game Over");
	}


	// See if the sender is the object we want to interact with
	public void CheckMessage(s3DBButton_sender sender)
	{
		if (currentState == RoundState.Playing) {
			if (sender.gameObject == objects [currentObject].gameObject) {
				currentState = RoundState.Success;
			} else {
				Debug.Log ("Nope " + sender.GetName() + " find " + objects [currentObject].GetName());
			}
		}
	}
}

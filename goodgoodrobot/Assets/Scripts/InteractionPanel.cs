using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPanel : MonoBehaviour {
	public s3DBButton_sender[] objects { get; private set;}
	// Use this for initialization
	void Awake () {
		objects = GetComponentsInChildren<s3DBButton_sender> ();
	}
}

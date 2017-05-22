using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTracker : MonoBehaviour {

	// Contains a HMD tracked object that we can use to find the user's gaze
	Transform hmdTrackedObject = null;

	// Use this for initialization
	void Start () {
		hmdTrackedObject = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (hmdTrackedObject) {
			Ray r = new Ray (hmdTrackedObject.position, hmdTrackedObject.forward);
			RaycastHit hit;

			if (Physics.SphereCast(r, 0.1f, out hit, 0.5f)) {
				s3DBButton_sender target = hit.collider.GetComponent<s3DBButton_sender> ();
				if (target != null) {
					target.OnGaze ();
				}
			}

		}
	}
}

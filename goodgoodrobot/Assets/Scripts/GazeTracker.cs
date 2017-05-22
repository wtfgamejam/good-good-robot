using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTracker : MonoBehaviour {

	// Contains a HMD tracked object that we can use to find the user's gaze
	Transform hmdTrackedObject = null;
	public float gazeInCutoff = 0.15f;
	public float gazeOutCutoff = 0.4f;
	public bool isInGaze = false;

	// Use this for initialization
	void Start () {
		hmdTrackedObject = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// If we haven't set up hmdTrackedObject find what the user is looking at
		if (hmdTrackedObject == null) {
			SteamVR_TrackedObject[] trackedObjects = FindObjectsOfType<SteamVR_TrackedObject> ();
			foreach (SteamVR_TrackedObject tracked in trackedObjects) {
				if (tracked.index == SteamVR_TrackedObject.EIndex.Hmd) {
					hmdTrackedObject = tracked.transform;
					break;
				}
			}
		}

		if (hmdTrackedObject) {
			Ray r = new Ray (hmdTrackedObject.position, hmdTrackedObject.forward);
			RaycastHit hit;

			if (Physics.Raycast(r, out hit)) {
				s3DBButton_sender target = hit.collider.GetComponent<s3DBButton_sender> ();
				if (target != null) {
					target.OnGaze ();
				}
			}

		}
	}
}

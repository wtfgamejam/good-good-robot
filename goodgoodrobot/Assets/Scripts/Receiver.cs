using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using s3DBakers.Buttons;

public class Receiver : s3DBButton_receiver {

	[SerializeField] AudioSource audio;
	[SerializeField] AudioClip[] clips;

	public MotionCurve[] motions;
	private int currentMotion = 0;
	private IEnumerator motionCoroutine;

public void button (str3DBbMessage msg) {
		base.button (msg);

		AudioClip playClip = null;
		if (clips.Length > 0) {
			playClip = clips [Random.Range(0,clips.Length - 1)];
		} else if(audio != null) {
			playClip = audio.clip;
		}

		if (audio != null && playClip != null) {
			audio.PlayOneShot (playClip);
		}

		if (msg.actions.position) {
			if (motionCoroutine != null) {
				StopCoroutine (motionCoroutine);
			}
			motionCoroutine = Movement (motions[currentMotion]);
			StartCoroutine (motionCoroutine);
		}
}
		
	IEnumerator Movement(MotionCurve motion)
	{
		transform.localPosition = motion.start;
		float duration = motion.motionDuration;
		float elapsed = 0;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			transform.localPosition = motion.GetValue (elapsed / duration);
			yield return null;
		}

		EndMotion (motion);
		yield return null;
	}

	void EndMotion(MotionCurve motion)
	{
		transform.localPosition = motion.end;
		currentMotion++;
		if (currentMotion > motions.Length - 1) {
			currentMotion = 0;
		}
	}

}

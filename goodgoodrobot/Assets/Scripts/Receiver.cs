using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using s3DBakers.Buttons;

public class Receiver : s3DBButton_receiver {

	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip[] clips;

	public MotionCurve[] motions;
	private int currentMotion = 0;
	private IEnumerator motionCoroutine;

	public MotionCurve[] rotations;
	private int currentRotation = 0;
	private IEnumerator rotationCoroutine;

	public AnimationClip[] animationClips;
	private int currentAnimation = 0;
	private IEnumerator animationCoroutine;

	Animator anim;

public void button (str3DBbMessage msg) {
		base.button (msg);

		AudioClip playClip = null;
		if (clips.Length > 0) {
			playClip = clips [Random.Range(0,clips.Length)];
		} else if(audioSource != null) {
			playClip = audioSource.clip;
		}

		if (audioSource != null && playClip != null) {
			audioSource.PlayOneShot (playClip);
		}

		if (msg.actions.position) {
			if (motionCoroutine != null) {
				StopCoroutine (motionCoroutine);
				motionCoroutine = null;
			}

			if (motions.Length > 0) {
				motionCoroutine = Movement (motions [currentMotion]);
				StartCoroutine (motionCoroutine);
			}
		}

		if (msg.actions.rotation) {
			if (rotationCoroutine != null) {
				StopCoroutine (rotationCoroutine);
				rotationCoroutine = null;
			}

			if (rotations.Length > 0) {
				rotationCoroutine = Rotation (rotations [currentRotation]);
				StartCoroutine (rotationCoroutine);
			}
		}

		if (msg.actions.animation) {
			if (rotationCoroutine != null) {
				StopCoroutine (animationCoroutine);
				animationCoroutine = null;
			}

			if (animationClips.Length > 0) {
				animationCoroutine = Animate (animationClips [currentAnimation]);
				StartCoroutine (animationCoroutine);
			}
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

	IEnumerator Rotation(MotionCurve rotation)
	{
		transform.localRotation = Quaternion.Euler(rotation.start.x, rotation.start.y, rotation.start.z);
		float duration = rotation.motionDuration;
		float elapsed = 0;

		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			Vector3 current = rotation.GetValue (elapsed / duration);
			transform.localRotation = Quaternion.Euler(current.x, current.y, current.z);
			yield return null;
		}

		EndRotation (rotation);
		yield return null;
	}

	void EndRotation(MotionCurve rotation)
	{
		transform.localRotation = Quaternion.Euler(rotation.end.x, rotation.end.y, rotation.end.z);
		currentRotation++;
		if (currentRotation > rotations.Length - 1) {
			currentRotation = 0;
		}
	}

	IEnumerator Animate(AnimationClip animation)
	{
		if (anim == null) {
			anim = gameObject.AddComponent<Animator> ();
			anim.runtimeAnimatorController = GameManager.Instance.animatorController;
		}

		anim.Play (animation.name);

		while (anim.GetCurrentAnimatorStateInfo (0).IsName (animation.name)) {
			yield return null;
		}

		StopAnimation (animation);
	}

	void StopAnimation(AnimationClip animation)
	{
		
		currentAnimation++;
		if (currentAnimation > animationClips.Length - 1) {
			currentAnimation = 0;
		}
	}
}

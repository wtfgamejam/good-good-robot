using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using s3DBakers.Buttons;

public class Receiver : s3DBButton_receiver {

	[SerializeField] AudioSource audio;
	[SerializeField] AudioClip[] clips;

public void button (str3DBbMessage msg) {
		base.button (msg);

		AudioClip playClip;
		if (clips.Length > 0) {
			playClip = clips [0];
		} else {
			playClip = audio.clip;
		}

		if (audio != null && playClip != null) {
			audio.PlayOneShot (playClip);
		}
}

}

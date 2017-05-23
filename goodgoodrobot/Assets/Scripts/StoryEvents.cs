using UnityEngine;
using System.Collections;

public class StoryEvents : MonoBehaviour
{
	public AudioClip[] backgroundAudio;
	public StoryEvent[] eventList;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (NextBackgroundAudio ());
	}

	IEnumerator PlayEvent(StoryEvent e) {
		int currentClip = 0;
		while (currentClip < e.eventAudio.Length) {
			AudioClip clip = e.eventAudio [currentClip];
			GetComponent<AudioSource> ().PlayOneShot (clip);
			yield return new WaitForSeconds (clip.length);
			currentClip++;
		}
	}

	public void TriggerEvent() {
		StartCoroutine(PlayEvent (eventList [Random.Range (0, eventList.Length)]));
	}

	IEnumerator NextBackgroundAudio() {
		
		yield return new WaitForSeconds(Random.Range(10f, 30f));

		AudioSource source = GetComponent<AudioSource> ();
		if (source != null && backgroundAudio.Length > 0) {
			source.clip = backgroundAudio [Random.Range (0, backgroundAudio.Length)];
			source.Play();
		}

		StartCoroutine(NextBackgroundAudio ());
	}

	// for testing
	void OnMouseOver(){
		if (Input.GetMouseButtonDown (0) && eventList.Length > 0) {
			TriggerEvent ();
		}

	}

}


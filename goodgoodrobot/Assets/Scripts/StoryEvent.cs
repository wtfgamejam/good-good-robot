﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class StoryEvent
{
	public AudioClip[] eventAudio;

	public AudioClip GetClip(int index)
	{
		return eventAudio [index];
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MotionCurve {

	public AnimationCurve motionCurve = AnimationCurve.Linear(0, 0, 1, 1);
	public Vector3 start = Vector3.zero;
	public Vector3 end = Vector3.zero;
	public float motionDuration = 1.0f;

	public void SetStart(Vector3 s)
	{
		start = s;
	}

	public void SetEnd(Vector3 e)
	{
		end = e;
	}

	public Vector3 GetValue(float percent)
	{
		float curvedValue = motionCurve.Evaluate(percent);
		Vector3 newPos = Vector3.Lerp (start, end, curvedValue);
		return newPos;
	}

}

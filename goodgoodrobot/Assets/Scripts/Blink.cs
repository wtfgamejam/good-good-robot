using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour
{
	public Color baseColor;
	public float Speed = 1;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		Renderer renderer = GetComponent<Renderer> ();
		Material mat = renderer.material;

		float emission = Mathf.PingPong (Time.time * Speed, 1.0f);
		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);

		mat.SetColor ("_EmissionColor", finalColor);
	}
}


using UnityEngine;
using System.Collections;
using System;

public class Sinus : MonoBehaviour {

	public double frequency = 20;
	public double gain = 0.50;
	public float offset = 0.0f;
	
	private double increment;
	private double phase;
	private double sampling_frequency = 44100;
	private System.Random random = new System.Random ();
	private AudioLowPassFilter lowPassFilter;

	// Use this for initialization
	void Start () {
		//Debug.Log ("onStart cube");
		lowPassFilter = gameObject.AddComponent <AudioLowPassFilter>() as AudioLowPassFilter;
		lowPassFilter.cutoffFrequency = 500;

		AudioSource audioSource = GetComponent<AudioSource> ();
		audioSource.clip = Microphone.Start ("Built-in Microphone", true, 4, 44100);
		audioSource.loop = true;
		audioSource.Play (44100);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Update cube");

		/*lowPassFilter.cutoffFrequency += 10f;
		if (lowPassFilter.cutoffFrequency > 12000) lowPassFilter.cutoffFrequency = 0;*/
	}

	void OnAudioFilterRead(float[] data, int channels) {


		/*Debug.Log ("onAudioFilterRead: " + data.Length);
		// update increment in case frequency has changed
		increment = frequency * 2 * Math.PI / sampling_frequency;
		for (var i = 0; i < data.Length; i = i + channels) {
			phase = phase + increment;
			// this is where we copy audio data to make them “available” to Unity
			//data[i] = (float)(gain*Math.Sin(phase)); // generates sinus sweep
			data[i] = (float) offset + (float)random.NextDouble () * 2.0f - 1.0f;



			frequency = frequency + 0.1;
			if (frequency > 22000) frequency = 20;
			// if we have stereo, we copy the mono data to each channel
			if (channels == 2) data[i + 1] = data[i];
			if (phase > 2 * Math.PI) phase = 0;
		}
*/
	}
}

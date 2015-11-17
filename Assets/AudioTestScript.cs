using UnityEngine;
using System.Collections;

public class AudioTestScript : MonoBehaviour {

	public AudioClip testAudioClip;
	private AudioSource audioSource;
	private float[] samples;
	public int numberOfSamples = 1024;

	private float rms;
	private float db;
	public float reference = 0.1f; // rms value for 0db


	// Use this for initialization
	void Start () {
		this.audioSource = this.gameObject.AddComponent<AudioSource> () as AudioSource;
		this.testAudioClip.LoadAudioData ();
		this.audioSource.clip = this.testAudioClip;
		this.samples = new float[this.numberOfSamples];
		/*this.audioSource.Play ();
		this.audioSource.Pause ();*/

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Decibel: " + AudioAnalyzer.Instance.getDecibel ());
		Debug.Log ("RMS: " + AudioAnalyzer.Instance.getRMS ());
		/*this.audioSource.GetOutputData (this.samples, 0); // (samples, channel)

		float amplitudeSum = 0.0f;
		for (int i = 0; i < this.numberOfSamples; i++) {
			amplitudeSum += this.samples[i] * this.samples[i]; // sum the square volumes for RMS value
		}

		this.rms = (float) Mathf.Sqrt (amplitudeSum / (float) this.numberOfSamples);
		this.db = 20 * Mathf.Log10(rms / reference);
		this.db = Mathf.Max (-160, this.db); // clip it to -160 on the bottom edge
		
		Debug.Log ("decibel: " + this.db);*/
	}
}

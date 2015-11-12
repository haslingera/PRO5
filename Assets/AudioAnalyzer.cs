using UnityEngine;
using System.Collections;

public class AudioAnalyzer : MonoBehaviour {

	public int numberOfSamples = 1024;
	public float reference = 0.1f; // rms value for 0db
	public float threshold = 0.01f; // minimum amplitude for calculating the pitch 

	private AudioSource audioSource;
	private float[] samples;
	private float[] spectrum;
	private int sampleRate;

	private float rms = 0.0f;
	private float db = 0.0f;
	private float pitch = 0.0f;

	// visualization
	public bool drawLines = true;
	private LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
		//this.audioSource = this.gameObject.AddComponent<AudioSource>() as AudioSource;
		//this.audioSource = this.gameObject.GetComponent<AudioSource> ();
		this.audioSource = this.gameObject.AddComponent<AudioSource> () as AudioSource;
		this.samples = new float[this.numberOfSamples];
		this.spectrum = new float[this.numberOfSamples];
		this.sampleRate = 44100;

		this.audioSource.clip = Microphone.Start ("Built-in Microphone", true, 1, this.sampleRate);
		this.audioSource.loop = true;
		this.audioSource.Play ((ulong) this.sampleRate);

		//this.lineRenderer = this.gameObject.GetComponent<LineRenderer> ();
		this.lineRenderer = this.gameObject.AddComponent<LineRenderer> () as LineRenderer;
		this.lineRenderer.SetVertexCount (this.numberOfSamples);

		Material lineMaterial = Resources.Load("LineMaterial", typeof(Material)) as Material;
		this.lineRenderer.material = lineMaterial;
		this.lineRenderer.useWorldSpace = true;
		this.lineRenderer.useLightProbes = false;
		this.lineRenderer.SetWidth (0.05f, 0.05f);
		this.lineRenderer.SetColors (Color.magenta, Color.magenta);
		this.lineRenderer.enabled = drawLines;
	}
	
	// Update is called once per frame
	void Update () {
		this.analyzeAudio ();
	}


	private void analyzeAudio() {
		// analyze volume
		this.audioSource.GetOutputData (this.samples, 0); // (samples, channel)

		float amplitudeSum = 0.0f;
		for (int i = 0; i < this.numberOfSamples; i++) {
			amplitudeSum += this.samples[i] * this.samples[i]; // sum the square volumes for RMS value
		}

		this.rms = Mathf.Sqrt (amplitudeSum / (float) this.numberOfSamples);
		this.db = 20 * Mathf.Log10(rms / reference);
		this.db = Mathf.Max (-160, this.db); // clip it to -160 on the bottom edge

		Debug.Log ("decibel: " + this.db);

		// analyze spectrum
		this.audioSource.GetSpectrumData (this.spectrum, 0, FFTWindow.BlackmanHarris); // (spectrum, channel, FFTWindow)

		int maxFrequencyIndex = 0;
		float maxAmount = 0.0f;

		// for visual feedback: calculate position in front of the camera
		Vector3 frontOfCamera = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * 20,
		                                    Camera.main.transform.position.y + Camera.main.transform.forward.y * 20,
		                                    Camera.main.transform.position.z + Camera.main.transform.forward.z * 20);

		for (int i = 0; i < this.numberOfSamples; i++) {

			// add point to line Renderer to show the visual feedback of the spectrum
			this.lineRenderer.SetPosition (i, new Vector3(frontOfCamera.x + Camera.main.transform.right.x * (i / 10.0f) + Camera.main.transform.up.x * this.spectrum[i] * 30, 
			                                         frontOfCamera.y + Camera.main.transform.right.y * (i / 10.0f) + Camera.main.transform.up.y * this.spectrum[i] * 30,
			                                         frontOfCamera.z + Camera.main.transform.right.z * (i / 10.0f) + Camera.main.transform.up.z * this.spectrum[i] * 30));

			if (this.spectrum[i] > maxAmount) {
				maxAmount = this.spectrum[i];
				maxFrequencyIndex = i; 
			}
		}

		float frequencyIndex = maxFrequencyIndex;
		if (maxFrequencyIndex > 0 && maxFrequencyIndex < this.numberOfSamples - 1) {
			float dL = this.spectrum[maxFrequencyIndex - 1] / this.spectrum[maxFrequencyIndex];
			float dR = this.spectrum[maxFrequencyIndex + 1] / this.spectrum[maxFrequencyIndex];
			frequencyIndex += 0.5f * (dR * dR - dL * dL);
		}

		float pitch = frequencyIndex * (this.sampleRate / 2.0f) / this.numberOfSamples;
		this.pitch = pitch;

		//Debug.Log ("pitch: " + pitch);
	}

	public double getDecibel() {
		return this.db;
	}

	public double getFrequency() {
		return this.pitch;
	}
}

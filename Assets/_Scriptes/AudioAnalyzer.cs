using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Threading;
using Pitch;
using AssemblyCSharp;

// Singleton Code from: http://clearcutgames.net/home/?p=437
// Analyze Code from: http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html

public class AudioAnalyzer : MonoBehaviour {
	// Static singleton instance
	private static AudioAnalyzer instance;

	private float MicLoudness;
	
	private string _device;
	private bool _isInitialized;
	private static float refValue = 0.01f;
	static int _sampleWindow = 1024;
	static AudioClip _clipRecord = new AudioClip ();
	
	
	// YIN Frequency Detection variables
	public bool trackFrequency = false;
	public int yinSampleWindow = 1024;
	private int yinBufferSize;
	private int yinHalfBufferSize;
	private float yinProbability;
	private float yinThreshold;
	private float yinPitch;
	
	private float[] yinBuffer;
	private int[] dataBuffer;
	// YIN Frequency Detection variables end

	private YIN yin;
	private Thread yinThread;

	private PitchTracker pitchTracker;
	private float latestPitch;

	private MIT mit;
	private Queue<float> pitchQueue;

	// Static singleton property
	public static AudioAnalyzer Instance {
		// Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		// otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameObject ("AudioAnalyzer").AddComponent<AudioAnalyzer> ()); }
	}

	public void Init() {

	}

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}

	void Start ()
	{
		this.mit = new MIT ();
		this.yinPitch = -1.0f;
		this.dataBuffer = new int[this.yinSampleWindow];
	
	
		// start the yin Thread in Background
		this.yin = new YIN ();
		this.yinThread = new Thread (new ThreadStart (yin.startYIN));
		this.yinThread.IsBackground = true;
		this.yinThread.Start ();

		this.pitchTracker = new PitchTracker();
		this.pitchTracker.SampleRate = 44100.0;
		this.latestPitch = -1.0f;
		this.pitchQueue = new Queue<float> ();
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
		this.pitchQueue.Enqueue (0.0f);
	}

//mic initialization
	void InitMic () {
		if (_device == null)
			_device = Microphone.devices [0];
		_clipRecord = Microphone.Start (_device, true, 999, 44100);
		iPhoneSpeaker.ForceToSpeaker ();
	}

	void StopMicrophone () {
		Microphone.End (_device);
	}

	//get data from microphone into audioclip
	private float LevelMax () {
		float levelMax = 0;
		float[] waveData = new float[_sampleWindow];
		int micPosition = Microphone.GetPosition (null) - (_sampleWindow + 1); // null means the first microphone
		if (micPosition < 0)
			return 0;
		_clipRecord.GetData (waveData, micPosition);
	
		// Getting a peak on the last 128 samples
		for (int i = 0; i < _sampleWindow; i++) {
			float wavePeak = waveData [i] * waveData [i];
			if (levelMax < wavePeak) {
				levelMax = wavePeak;
			}
		}
	
		levelMax = 20 * Mathf.Log10 (levelMax / refValue); // calculate dB
	
		if (levelMax < -160) {
			levelMax = -160; // clamp it to -160dB min
		}
		return levelMax;
	}

	void Update () {
		// levelMax equals to the highest normalized value power 2, a small number because < 1
		// pass the value to a static var so we can access it from anywhere
		MicLoudness = LevelMax ();
		/*
		MIT._dywapitchtracker pitchtracker;
		pitchtracker._pitchConfidence = 0;
		pitchtracker._prevPitch = 0.0;
		this.mit.dywapitch_inittracking(pitchtracker);

		// For each available audio buffer, call 'dywapitch_computepitch'
		float[] waveData = new float[this.yinSampleWindow];
		int micPosition = Microphone.GetPosition (null) - (this.yinSampleWindow + 1); // null means the first microphone
		if (micPosition < 0) return;

		_clipRecord.GetData (waveData, micPosition);
		double[] doubleWaveData = new double[waveData.Length];
		waveData.CopyTo (doubleWaveData, 0);
		double thepitch = this.mit.dywapitch_computepitch(pitchtracker, doubleWaveData, 0, waveData.Length);

		this.latestPitch = (float)thepitch;
		Debug.Log ("thepitch: " + thepitch);

		// only use the pitch if the micLoudness is >= 10
		if (thepitch <= 1.0 || MicLoudness < 10.0)
			thepitch = -1.0;

		// enqueue the latest pitch
		this.pitchQueue.Enqueue ((float)this.latestPitch);

		// iterate over all pitches
		float[] pitches = this.pitchQueue.ToArray();
		int negValuesCounter = 0;
		float pitchSum = 0.0f;
		foreach (float pitch in pitches) {
			// count the number of values which are <= 1 (where no pitch was detected)
			if (pitch <= 1.0f)
				negValuesCounter++;

			pitchSum += pitch;
		}

		this.pitchQueue.Dequeue ();
		float pitchAverage = pitchSum / pitches.Length;

		// if more valid pitches have been detected, then use the average pitch as the detected pitch
		if (negValuesCounter < pitches.Length / 2) {
			this.latestPitch = pitchAverage;
		} else {
			this.latestPitch = -1.0f;
		}

		Debug.Log ("pitchAVG: " + pitchAverage + ", negValues: " + negValuesCounter + "loudness: " + MicLoudness + "pitch: " + thepitch);
		//Debug.Log ("thepitch: " + thepitch + ", loudness: " + MicLoudness);
	*/

		// new version
		/*
		float[] waveData = new float[this.yinSampleWindow];
		int micPosition = Microphone.GetPosition (null) - (this.yinSampleWindow + 1); // null means the first microphone
		if (micPosition < 0)
			return;
		
		_clipRecord.GetData (waveData, micPosition);
		pitchTracker.ProcessBuffer (waveData, this.yinSampleWindow);
		PitchTracker.PitchRecord record = pitchTracker.CurrentPitchRecord;
		this.latestPitch = record.Pitch;

		if (this.latestPitch < 1.0f) {
			this.latestPitch = -1.0f;
		}
				Debug.Log ("pitch: " + this.latestPitch);
*/


		// old YIN algorithm
		 if (!this.yinThread.IsAlive) {
			// read mic data
			float[] waveData = new float[this.yinSampleWindow];
			int micPosition = Microphone.GetPosition (null) - (this.yinSampleWindow + 1); // null means the first microphone
			if (micPosition < 0)
				return;
			_clipRecord.GetData (waveData, micPosition);
		
			
			//Debug.Log ("pitch: " + record.Pitch);

			// restart thread
			this.yin.setupSamples (waveData);
			this.yinThread = new Thread (new ThreadStart (yin.startYIN));
			this.yinThread.IsBackground = true;
			this.yinThread.Start ();
		} 




		//pitchTracker.ProcessBuffer(audioBuffer);
	}

// start mic when scene starts
	void OnEnable ()
	{
		InitMic ();
		_isInitialized = true;
	}

//stop mic when loading a new level or quit application
	void OnDisable ()
	{
		StopMicrophone ();
	}

	void OnDestroy ()
	{
		StopMicrophone ();
	}

// make sure the mic gets started & stopped when application gets focused
	void OnApplicationFocus (bool focus)
	{
		if (focus) {
			//Debug.Log("Focus");
			if (!_isInitialized) {
				//Debug.Log("Init Mic");
				InitMic ();
				_isInitialized = true;
			}
		}    
	
		if (!focus) {
			//Debug.Log("Pause");
			StopMicrophone ();
			//Debug.Log("Stop Mic");
			_isInitialized = false;	
		}
	}

	public float getPitch() {
		//return this.latestPitch;
		return this.yin.getPitch ();
	}

	public float getMicLoudness() {
		return this.MicLoudness;
	}

}

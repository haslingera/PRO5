using UnityEngine;
using System.Collections;
using System.Threading;

public class InputAnalyser : MonoBehaviour {
	
	public float MicLoudness;
	
	private string _device;
	private bool _isInitialized;
	private static float refValue = 0.01f;
	static int _sampleWindow = 128;
	static AudioClip _clipRecord = new AudioClip();


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



	// -------------------------
	//         Methods
	// -------------------------
	
	// Called when before first update begins
	void Start () {
		this.yinPitch = -1.0f;
		this.dataBuffer = new int[this.yinSampleWindow];


		// start the yin Thread in Background
		this.yin = new YIN();
		this.yinThread = new Thread(new ThreadStart(yin.startYIN));
		this.yinThread.IsBackground = true;
		this.yinThread.Start ();
	}
	
	//mic initialization
	void InitMic() {
		if(_device == null) _device = Microphone.devices[0];
		_clipRecord = Microphone.Start(_device, true, 999, 44100);
	}
	
	void StopMicrophone() {
		Microphone.End(_device);
	}

	//get data from microphone into audioclip
	public float LevelMax() {
		float levelMax = 0;
		float[] waveData = new float[_sampleWindow];
		int micPosition = Microphone.GetPosition(null)-(_sampleWindow+1); // null means the first microphone
		if (micPosition < 0) return 0;
		_clipRecord.GetData(waveData, micPosition);

		// Getting a peak on the last 128 samples
		for (int i = 0; i < _sampleWindow; i++) {
			float wavePeak = waveData[i] * waveData[i];
			if (levelMax < wavePeak) {
				levelMax = wavePeak;
			}
		}

		levelMax = 20*Mathf.Log10(levelMax/refValue); // calculate dB

		if (levelMax < -160) {
			levelMax = -160; // clamp it to -160dB min
		}
		return levelMax;
	}

	void Update() {
		// levelMax equals to the highest normalized value power 2, a small number because < 1
		// pass the value to a static var so we can access it from anywhere
		MicLoudness = LevelMax ();

		if (!this.yinThread.IsAlive) {
			// read mic data
			float[] waveData = new float[this.yinSampleWindow];
			int micPosition = Microphone.GetPosition(null)-(this.yinSampleWindow+1); // null means the first microphone
			if (micPosition < 0) return;
			_clipRecord.GetData(waveData, micPosition);
			
			// restart thread
			this.yin.setupSamples(waveData);
			this.yinThread = new Thread(new ThreadStart(yin.startYIN));
			this.yinThread.IsBackground = true;
			this.yinThread.Start ();
		} 

		// if frequency should be tracked do this now
		/*if (this.trackFrequency) {

			// read samples from the microphone
			float[] waveData = new float[this.yinSampleWindow];
			int micPosition = Microphone.GetPosition(null)-(this.yinSampleWindow+1); // null means the first microphone
			if (micPosition < 0) return;
			_clipRecord.GetData(waveData, micPosition);

			// scale the data to [-255 | 255]
			for (int i = 0; i < this.yinSampleWindow; i++) {
				this.dataBuffer[i] = (int) (waveData[i] * 255);
			}

			float pitch = 0.0f;
			int bufferSize = 100;
			while (pitch < 10.0f && bufferSize < this.yinSampleWindow) {
				this.yinInit(bufferSize);
				pitch = this.yinGetPitch();
				bufferSize += 2;
			}

			this.yinPitch = pitch;
			Debug.Log ("pitch: " + pitch + ", bufferSize: " + bufferSize);
		}*/
	}
	
	// start mic when scene starts
	void OnEnable()
	{
		InitMic();
		_isInitialized=true;
	}
	
	//stop mic when loading a new level or quit application
	void OnDisable()
	{
		StopMicrophone();
	}
	
	void OnDestroy()
	{
		StopMicrophone();
	}

	// make sure the mic gets started & stopped when application gets focused
	void OnApplicationFocus(bool focus) {
		if (focus) {
			//Debug.Log("Focus");
			if(!_isInitialized) {
				//Debug.Log("Init Mic");
				InitMic();
				_isInitialized=true;
			}
		}    

		if (!focus) {
			//Debug.Log("Pause");
			StopMicrophone();
			//Debug.Log("Stop Mic");
			_isInitialized=false;	
		}
	}




	// -----------------------
	//     YIN Algorith 
	// -----------------------

	// Init the YIN variables according to buffer size
	private void yinInit(int bufferSize) {
		this.yinBufferSize = bufferSize;
		this.yinHalfBufferSize = this.yinBufferSize / 2;
		this.yinProbability = 0.0f;
		this.yinThreshold = 0.05f;
		
		this.yinBuffer = new float[this.yinHalfBufferSize];
	}
	
	// step 1 of YIN Algorithm
	private void yinDifference() {
		// step 1: Calculates the squared difference of the signal with a shifted version of itself.
		// @param buffer Buffer of samples to process. 
		
		float delta;
		
		for (int tau = 0; tau < this.yinHalfBufferSize; tau++) {
			for (int i = 0; i < this.yinHalfBufferSize; i++) {
				delta = this.dataBuffer[i] - this.dataBuffer[i + tau];
				this.yinBuffer[tau] += delta * delta;
			}
		}
	}
	
	// step2 of YIN Algorithm: calculate the cumulative mean on the normalized difference calculated in step 1
	// this goes through the Yin autocorellation values and finds out roughly where shift is which produced the smallest difference
	private void yinCumulativeMeanNormalizedDifference() {
		float runningSum = 0.0f;
		this.yinBuffer[0] = 1;
		
		for (int tau = 1; tau < this.yinHalfBufferSize; tau++) {
			runningSum += this.yinBuffer[tau];
			this.yinBuffer[tau] *= tau / runningSum;
			//Debug.Log ("yinBuffer[" + tau + "]: " + this.yinBuffer [tau]);
		}
		
	}
	
	
	// step3 of YIN Algorithm: search through the normalized cumulative mean array and find values that are over the threshold
	// return shift (tau) which caused the best approximate autocorrelation. -1 if no suitable value is found over the threshold
	private int yinAbsoluteThreshold() {
		int tau;
		
		for (tau = 2; tau < this.yinHalfBufferSize; tau++) {
			if (this.yinBuffer[tau] < this.yinThreshold) {
				while ((tau + 1) < this.yinHalfBufferSize && this.yinBuffer[tau + 1] < this.yinBuffer[tau]) {
					tau++;
				}
				
				/* found tau, exit loop and return
			 * store the probability
			 * From the YIN paper: The yin->threshold determines the list of
			 * candidates admitted to the set, and can be interpreted as the
			 * proportion of aperiodic power tolerated
			 * within a periodic signal.
			 *
			 * Since we want the periodicity and and not aperiodicity:
			 * periodicity = 1 - aperiodicity */
				
				this.yinProbability = 1 - this.yinBuffer[tau];
				break;
			}
		}
		
		// if no pitch found, tau = -1
		if (tau == this.yinHalfBufferSize || this.yinBuffer[tau] >= this.yinThreshold) {
			tau = -1;
			this.yinProbability = 0;
		}
		
		return tau;
	}
	
	// step4 of YIN Algorithm
	private float yinParabolicInterpolation(int tauEstimate) {
		float betterTau;
		int x0;
		int x2;
		
		// Calculate the first polynomial coefficient based on the current estimate of tau
		if (tauEstimate < 1) {
			x0 = tauEstimate;
		} else {
			x0 = tauEstimate - 1;
		}
		
		// Calculate the second polynomial coefficient based on the current estimate of tau
		if (tauEstimate + 1 < this.yinHalfBufferSize) {
			x2 = tauEstimate + 1;
		} else {
			x2 = tauEstimate;
		}
		
		// Algorithm to parabolically interpolate the shift value tau to find a better estimate
		if (x0 == tauEstimate) {
			if (this.yinBuffer [tauEstimate] <= this.yinBuffer [x2]) {
				betterTau = tauEstimate;
			} else {
				betterTau = x2;
			}
		} else if (x2 == tauEstimate) {
			if (this.yinBuffer [tauEstimate] <= this.yinBuffer [x0]) {
				betterTau = tauEstimate;
			} else {
				betterTau = x0;
			}
		} else {
			float s0, s1, s2;
			s0 = this.yinBuffer[x0];
			s1 = this.yinBuffer[tauEstimate];
			s2 = this.yinBuffer[x2];
			// fixed AUBIO implementation, thanks to Karl Helgason:
			// (2.0f * s1 - s2 - s0) was incorrectly multiplied with -1
			betterTau = tauEstimate + (s2 - s0) / (2 * (2 * s1 - s2 - s0));
		}
		
		return betterTau;
		return tauEstimate;
	}
	
	// Method to call to start the calculation
	private float yinGetPitch() {
		int tauEstimate = -1;
		float pitchInHertz = -1;
		
		/* Step 1: Calculates the squared difference of the signal with a shifted version of itself. */
		yinDifference ();
		
		/* Step 2: Calculate the cumulative mean on the normalised difference calculated in step 1 */
		yinCumulativeMeanNormalizedDifference ();
		
		/* Step 3: Search through the normalised cumulative mean array and find values that are over the threshold */
		tauEstimate = this.yinAbsoluteThreshold ();
		
		/* Step 5: Interpolate the shift value (tau) to improve the pitch estimate. */
		if(tauEstimate != -1){
			pitchInHertz = 44100.0f / this.yinParabolicInterpolation(tauEstimate);
		}
		
		return pitchInHertz;
	}

	public float getPitch () {
		return this.yin.getPitch ();
	}


	// --------------------
	// YIN Algorithm Ends
	// --------------------

}
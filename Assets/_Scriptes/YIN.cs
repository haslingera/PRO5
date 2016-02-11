using UnityEngine;
using System.Collections;
using System.Threading;

public class YIN {


	private int yinSampleWindow;
	private int yinBufferSize;
	private int yinHalfBufferSize;
	private float yinProbability;
	private float yinThreshold;
	private float yinPitch;
	
	private float[] yinBuffer;
	private int[] dataBuffer;
	private float[] dirtySamples;
	private bool gotSamples;

	public YIN() {
		this.gotSamples = false;
	}

	public void setupSamples(float[] samples) {
		this.gotSamples = true;
		this.dirtySamples = samples;
		this.yinSampleWindow = samples.Length;
		
		this.dataBuffer = new int[this.yinSampleWindow];
	}

	public void startYIN() {
		if (!this.gotSamples) {
			return;
		}
		// copy the dirtySamples to int array (and scale them)
		for (int i = 0; i < this.yinSampleWindow; i++) {
			this.dataBuffer[i] = (int) (this.dirtySamples[i] * 255);
		}

		// perform the yin algorithm
		float pitch = 0.0f;
		int bufferSize = 1024;
		//while (pitch < 10.0f && bufferSize < this.yinSampleWindow) {
			this.yinInit(bufferSize);
			pitch = this.yinGetPitch();
			bufferSize += 4;
		//4}
		Debug.Log("piiiitch: " + pitch);
		lock (this) {
			this.yinPitch = pitch;
		}
	}

	public float getPitch() {
		lock (this) {
			return this.yinPitch;
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
}

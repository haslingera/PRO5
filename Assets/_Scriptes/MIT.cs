using System;

namespace AssemblyCSharp {
	// the MIT audio analyzer class, found here: http://www.schmittmachine.com/dywapitchtrack.html
	public class MIT {
		public MIT () {
		}

		//**********************
		//       Structs
		//********************** 

		public struct _dywapitchtracker {
			public double _prevPitch;
			public int _pitchConfidence;
		} 


		//**********************
		//       Utils
		//**********************

		// returns 1 if power of 2
		private int _power2p(int value) {
			if (value == 0) return 1;
			if (value == 2) return 1;
			if ((value & 0x1) != 0) return 0;
			return (_power2p(value >> 1));
		}

		// count number of bits
		private int _bitcount(int value) {
			if (value == 0) return 0;
			if (value == 1) return 1;
			if (value == 2) return 2;
			return _bitcount(value >> 1) + 1;
		}

		// closest power of 2 above or equal
		private int _ceil_power2(int value) {
			if (_power2p(value) != 0) return value;

			if (value == 1) return 2;
			int j, i = _bitcount(value);
			int res = 1;
			for (j = 0; j < i; j++) res <<= 1;
			return res;
		}

		// closest power of 2 below or equal
		private int _floor_power2(int value) {
			if (_power2p(value) != 0) return value;
			return _ceil_power2(value)/2;
		}

		// abs value
		private int _iabs(int x) {
			if (x >= 0) return x;
			return -x;
		}

		// 2 power
		private int _2power(int i) {
			int res = 1, j;
			for (j = 0; j < i; j++) res <<= 1;
			return res;
		}


		//******************************
		// the Wavelet algorithm itself
		//******************************

		private int dywapitch_neededsamplecount(int minFreq) {
			int nbSam = 3*44100/minFreq; // 1017. for 130 Hz
			nbSam = _ceil_power2(nbSam); // 1024
			return nbSam;
		}







		double _dywapitch_computeWaveletPitch(double[] samples, int startsample, int samplecount) {
			double pitchF = 0.0;

			int i, j;
			double si, si1;

			// must be a power of 2
			samplecount = _floor_power2(samplecount);

			double[] sam = new double[samplecount];
			Array.Copy (samples, startsample, sam, 0, samplecount);
			int curSamNb = samplecount;

			int[] distances = new int[samplecount];
			int[] mins = new int[samplecount];
			int[] maxs = new int[samplecount];
			int nbMins, nbMaxs;

			// algorithm parameters
			int maxFLWTlevels = 6;
			double maxF = 3000.0;
			int differenceLevelsN = 3;
			double maximaThresholdRatio = 0.75;

			double ampltitudeThreshold;  
			double theDC = 0.0;

			{ // compute ampltitudeThreshold and theDC
				//first compute the DC and maxAMplitude
				double maxValue = 0.0;
				double minValue = 0.0;
				for (i = 0; i < samplecount;i++) {
					si = sam[i];
					theDC = theDC + si;
					if (si > maxValue) maxValue = si;
					if (si < minValue) minValue = si;
				}
				theDC = theDC/samplecount;
				maxValue = maxValue - theDC;
				minValue = minValue - theDC;
				double amplitudeMax = (maxValue > -minValue ? maxValue : -minValue);

				ampltitudeThreshold = amplitudeMax*maximaThresholdRatio;
				//asLog("dywapitch theDC=%f ampltitudeThreshold=%f\n", theDC, ampltitudeThreshold);

			}

			// levels, start without downsampling..
			int curLevel = 0;
			double curModeDistance = -1.0;
			int delta;

			while(true) {

				// delta
				delta = (int) (44100.0/(_2power(curLevel)*maxF));
				//("dywapitch doing level=%ld delta=%ld\n", curLevel, delta);

				if (curSamNb < 2) goto cleanup;

				// compute the first maximums and minumums after zero-crossing
				// store if greater than the min threshold
				// and if at a greater distance than delta
				double dv, previousDV = -1000;
				nbMins = nbMaxs = 0;   
				int lastMinIndex = -1000000;
				int lastmaxIndex = -1000000;
				int findMax = 0;
				int findMin = 0;
				for (i = 2; i < curSamNb; i++) {
					si = sam[i] - theDC;
					si1 = sam[i-1] - theDC;

					if (si1 <= 0 && si > 0) findMax = 1;
					if (si1 >= 0 && si < 0) findMin = 1;

					// min or max ?
					dv = si - si1;

					if (previousDV > -1000) {

						if (findMin != 0 && previousDV < 0 && dv >= 0) { 
							// minimum
							if (Math.Abs(si) >= ampltitudeThreshold) {
								if (i > lastMinIndex + delta) {
									mins[nbMins++] = i;
									lastMinIndex = i;
									findMin = 0;
									//if DEBUGG then put "min ok"&&si
									//
								} else {
									//if DEBUGG then put "min too close to previous"&&(i - lastMinIndex)
									//
								}
							} else {
								// if DEBUGG then put "min "&abs(si)&" < thresh = "&ampltitudeThreshold
								//--
							}
						}

						if (findMax != 0 && previousDV > 0 && dv <= 0) {
							// maximum
							if (Math.Abs(si) >= ampltitudeThreshold) {
								if (i > lastmaxIndex + delta) {
									maxs[nbMaxs++] = i;
									lastmaxIndex = i;
									findMax = 0;
								} else {
									//if DEBUGG then put "max too close to previous"&&(i - lastmaxIndex)
									//--
								}
							} else {
								//if DEBUGG then put "max "&abs(si)&" < thresh = "&ampltitudeThreshold
								//--
							}
						}
					}

					previousDV = dv;
				}

				if (nbMins == 0 && nbMaxs == 0) {
					// no best distance !
					//asLog("dywapitch no mins nor maxs, exiting\n");

					// if DEBUGG then put "no mins nor maxs, exiting"
					goto cleanup;
				}
				//if DEBUGG then put count(maxs)&&"maxs &"&&count(mins)&&"mins"

				// maxs = [5, 20, 100,...]
				// compute distances
				int d;

				// set 0 to samplecount elements in the distance array
				for (int z = 0; z < samplecount; z++) {
					distances[z] = 0;
				}
				//memset(distances, 0, samplecount*sizeof(int));
				for (i = 0 ; i < nbMins ; i++) {
					for (j = 1; j < differenceLevelsN; j++) {
						if (i+j < nbMins) {
							d = _iabs(mins[i] - mins[i+j]);
							//asLog("dywapitch i=%ld j=%ld d=%ld\n", i, j, d);
							distances[d] = distances[d] + 1;
						}
					}
				}
				for (i = 0 ; i < nbMaxs ; i++) {
					for (j = 1; j < differenceLevelsN; j++) {
						if (i+j < nbMaxs) {
							d = _iabs(maxs[i] - maxs[i+j]);
							//asLog("dywapitch i=%ld j=%ld d=%ld\n", i, j, d);
							distances[d] = distances[d] + 1;
						}
					}
				}

				// find best summed distance
				int bestDistance = -1;
				int bestValue = -1;
				for (i = 0; i< curSamNb; i++) {
					int summed = 0;
					for (j = -delta ; j <= delta ; j++) {
						if (i+j >=0 && i+j < curSamNb)
							summed += distances[i+j];
					}
					//asLog("dywapitch i=%ld summed=%ld bestDistance=%ld\n", i, summed, bestDistance);
					if (summed == bestValue) {
						if (i == 2*bestDistance)
							bestDistance = i;

					} else if (summed > bestValue) {
						bestValue = summed;
						bestDistance = i;
					}
				}
				//asLog("dywapitch bestDistance=%ld\n", bestDistance);

				// averaging
				double distAvg = 0.0;
				double nbDists = 0;
				for (j = -delta ; j <= delta ; j++) {
					if (bestDistance+j >=0 && bestDistance+j < samplecount) {
						int nbDist = distances[bestDistance+j];
						if (nbDist > 0) {
							nbDists += nbDist;
							distAvg += (bestDistance+j)*nbDist;
						}
					}
				}
				// this is our mode distance !
				distAvg /= nbDists;
				//asLog("dywapitch distAvg=%f\n", distAvg);

				// continue the levels ?
				if (curModeDistance > -1.0) {
					double similarity = Math.Abs(distAvg*2 - curModeDistance);
					if (similarity <= 2*delta) {
						//if DEBUGG then put "similarity="&similarity&&"delta="&delta&&"ok"
						//asLog("dywapitch similarity=%f OK !\n", similarity);
						// two consecutive similar mode distances : ok !
						pitchF = 44100.0/(_2power(curLevel-1)*curModeDistance);
						goto cleanup;
					}
					//if DEBUGG then put "similarity="&similarity&&"delta="&delta&&"not"
				}

				// not similar, continue next level
				curModeDistance = distAvg;

				curLevel = curLevel + 1;
				if (curLevel >= maxFLWTlevels) {
					// put "max levels reached, exiting"
					//asLog("dywapitch max levels reached, exiting\n");
					goto cleanup;
				}

				// downsample
				if (curSamNb < 2) {
					//asLog("dywapitch not enough samples, exiting\n");
					goto cleanup;
				}
				for (i = 0; i < curSamNb/2; i++) {
					sam[i] = (sam[2*i] + sam[2*i + 1])/2.0;
				}
				curSamNb /= 2;
			}

			///
			cleanup:
			//free(distances);
			//free(mins);
			//free(maxs);
			//free(sam);

			return pitchF;
		}


		// ***********************************
		// the dynamic postprocess
		// ***********************************

		/***
It states: 
 - a pitch cannot change much all of a sudden (20%) (impossible humanly,
 so if such a situation happens, consider that it is a mistake and drop it. 
 - a pitch cannot double or be divided by 2 all of a sudden : it is an
 algorithm side-effect : divide it or double it by 2. 
 - a lonely voiced pitch cannot happen, nor can a sudden drop in the middle
 of a voiced segment. Smooth the plot. 
***/

		double _dywapitch_dynamicprocess(_dywapitchtracker pitchtracker, double pitch) {

			// equivalence
			if (pitch == 0.0) pitch = -1.0;

			//
			double estimatedPitch = -1;
			double acceptedError = 0.2f;
			int maxConfidence = 5;

			if (pitch != -1) {
				// I have a pitch here

				if (pitchtracker._prevPitch == -1) {
					// no previous
					estimatedPitch = pitch;
					pitchtracker._prevPitch = pitch;
					pitchtracker._pitchConfidence = 1;

				} else if (Math.Abs(pitchtracker._prevPitch - pitch)/pitch < acceptedError) {
					// similar : remember and increment pitch
					pitchtracker._prevPitch = pitch;
					estimatedPitch = pitch;
					pitchtracker._pitchConfidence = Math.Min(maxConfidence, pitchtracker._pitchConfidence + 1); // maximum 3

				} else if ((pitchtracker._pitchConfidence >= maxConfidence-2) && Math.Abs(pitchtracker._prevPitch - 2.0*pitch)/(2.0*pitch) < acceptedError) {
					// close to half the last pitch, which is trusted
					estimatedPitch = 2.0*pitch;
					pitchtracker._prevPitch = estimatedPitch;

				} else if ((pitchtracker._pitchConfidence >= maxConfidence-2) && Math.Abs(pitchtracker._prevPitch - 0.5*pitch)/(0.5*pitch) < acceptedError) {
					// close to twice the last pitch, which is trusted
					estimatedPitch = 0.5*pitch;
					pitchtracker._prevPitch = estimatedPitch;

				} else {
					// nothing like this : very different value
					if (pitchtracker._pitchConfidence >= 1) {
						// previous trusted : keep previous
						estimatedPitch = pitchtracker._prevPitch;
						pitchtracker._pitchConfidence = Math.Max(0, pitchtracker._pitchConfidence - 1);
					} else {
						// previous not trusted : take current
						estimatedPitch = pitch;
						pitchtracker._prevPitch = pitch;
						pitchtracker._pitchConfidence = 1;
					}
				}

			} else {
				// no pitch now
				if (pitchtracker._prevPitch != -1) {
					// was pitch before
					if (pitchtracker._pitchConfidence >= 1) {
						// continue previous
						estimatedPitch = pitchtracker._prevPitch;
						pitchtracker._pitchConfidence = Math.Max(0, pitchtracker._pitchConfidence - 1);
					} else {
						pitchtracker._prevPitch = -1;
						estimatedPitch = -1.0;
						pitchtracker._pitchConfidence = 0;
					}
				}
			}

			// put "_pitchConfidence="&pitchtracker->_pitchConfidence
			if (pitchtracker._pitchConfidence >= 1) {
				// ok
				pitch = estimatedPitch;
			} else {
				pitch = -1;
			}

			// equivalence
			if (pitch == -1) pitch = 0.0;

			return pitch;
		}


		// ************************************
		// the API main entry points
		// ************************************

		public void dywapitch_inittracking(_dywapitchtracker pitchtracker) {
			pitchtracker._prevPitch = -1.0;
			pitchtracker._pitchConfidence = -1;
		}

		public double dywapitch_computepitch(_dywapitchtracker pitchtracker, double[] samples, int startsample, int samplecount) {
			double raw_pitch = _dywapitch_computeWaveletPitch(samples, startsample, samplecount);
			return _dywapitch_dynamicprocess(pitchtracker, raw_pitch);
		}
	}
}


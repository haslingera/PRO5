﻿using UnityEngine;
using System.Collections;

public class GlassDestroyer : MonoBehaviour {

	private StationaryMovement movement;
	private float totalScreamTime;
	private float startTime;
	private bool screaming = false;
	private bool stopscream = false;
	private float endTime;

	void Start () {
		movement = GetComponent<StationaryMovement> ();
	}

	void FixedUpdate () {
		if ((GetComponent<InputAnalyser>().MicLoudness+60) > 0f) {
			transform.position = new Vector3 (Mathf.Sin (Time.time*30)*(Time.time - startTime)/5f, transform.position.y,transform.position.z);
		}
		
		if (GetComponent<InputAnalyser>().MicLoudness > 35f) {
			endTime = Time.time;
		}


		if (!screaming && GetComponent<InputAnalyser>().MicLoudness > 35f) {
			screaming = true;
			startTime = Time.time;
		} else if(GetComponent<InputAnalyser>().MicLoudness < 35f) {
			if (!stopscream && Time.time - startTime > 2.0f) {
				stopscream = true;
				endTime = Time.time;
			}
		}

		if (Time.time - endTime >= 1f) {
			screaming = false;
			startTime = Time.time;
		}

		Debug.Log (Time.time - startTime);
		if (Time.time - startTime >= 5f) {
			Destroy (gameObject);
		}
	}
}

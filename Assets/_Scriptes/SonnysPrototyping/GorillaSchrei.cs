using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GorillaSchrei : MonoBehaviour {

	public Material [] materials;
	public float randomClamp;

	private float greenTime;
	//private float randomTime;
	private float startTime;
	private bool result;
	private float[] rythm = {0.3f,0.7f, 0.3f,0.3f,0.7f, 0.3f};
	private int count;

	void Start () {
		newTimer ();
		count = 0;
	}

	void FixedUpdate () {
		float passedTime = Time.time - startTime;
		if (passedTime >= rythm [count]) {
			GetComponent<Renderer> ().material = materials [1];
			if (!result && AudioAnalyzer.Instance.getMicLoudness () > 30.0f) {
				result = true;
				greenTime = passedTime;
			} else if (result) {
				GetComponent<Renderer> ().material = materials [2];
				if (passedTime - greenTime > 0.3f) {
					newTimer ();
				}
			} else if (!result && passedTime - rythm [count] < 1.3f && passedTime - rythm [count] > 1.0f) {
				GetComponent<Renderer> ().material = materials [3];
			} else if (passedTime - rythm [count] > 1.3f) {
			}
		}
	}

	void newTimer() {
		//randomTime = Random.value * randomClamp;
		startTime = Time.time;
		GetComponent<Renderer>().material = materials[0];
		result = false;
		count++;
		if (count >= rythm.Length) {
			count = 0;
		}
	}
}

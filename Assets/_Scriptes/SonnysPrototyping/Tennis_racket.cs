using UnityEngine;
using System.Collections;

public class Tennis_racket : MonoBehaviour {

	private Quaternion target;

	// Use this for initialization
	void Start () {
		target = Quaternion.Euler (0, 15, 300);
	}
	
	// Update is called once per frame
	void Update () {
		if (AudioAnalyzer.Instance.getMicLoudness() > 10) {
			transform.rotation = Quaternion.Lerp (transform.rotation, target, 0.02F);
		}
	}
}

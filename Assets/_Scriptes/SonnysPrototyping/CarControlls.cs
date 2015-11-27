using UnityEngine;
using System.Collections;

public class CarControlls : MonoBehaviour {

	private Rigidbody rigibi;

	void Awake () {
		AudioAnalyzer.Instance.Init ();
		rigibi = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getMicLoudness () >= 35) {
			rigibi.AddForce (transform.forward * 35);
			
		} else {
			if (AudioAnalyzer.Instance.getPitch () >= 300) {
				transform.Rotate(new Vector3 (0,-1,0));
			} else if (AudioAnalyzer.Instance.getPitch () < 300 && AudioAnalyzer.Instance.getPitch () > 50 ) {
				transform.Rotate(new Vector3 (0,1,0));
			}
		}
	}
}

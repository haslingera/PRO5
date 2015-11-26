using UnityEngine;
using System.Collections;

public class CarControlls : MonoBehaviour {

	private Rigidbody rigibi;

	void Awake () {
		AudioAnalyzer.Instance.Init ();
		rigibi = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getPitch () >= 500) {
			transform.Rotate(new Vector3 (0,1,0));
		} else if (AudioAnalyzer.Instance.getPitch () < 500 && AudioAnalyzer.Instance.getPitch () > 100 ) {
			transform.Rotate(new Vector3 (0,-1,0));
		}

		if (AudioAnalyzer.Instance.getMicLoudness () >= 35) {
			rigibi.AddForce(new Vector3 (50,0,0));
		
		}
	}
}

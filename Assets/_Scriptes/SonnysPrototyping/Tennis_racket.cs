using UnityEngine;
using System.Collections;

public class Tennis_racket : MonoBehaviour {

	public Vector3 rota;
	private Quaternion target;
	private Quaternion start;
	private bool swinging;
	private float time;
	//Quaternion.Euler (0, 80, 300)

	// Use this for initialization
	void Start () {
		target = transform.rotation.eulerAngles * Quaternion.Euler(rota);
		start = transform.rotation;
		swinging = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!swinging && AudioAnalyzer.Instance.getMicLoudness() > 10) {
			swinging = true;
			time = Time.time;
		}

		if (swinging) {
			transform.rotation = Quaternion.Lerp (transform.rotation, target, 0.25F);
		} else {
			transform.rotation = Quaternion.Lerp (transform.rotation, start, 0.25F);
		}

		if (transform.rotation == target) {
			swinging = true;
		}
	}
}

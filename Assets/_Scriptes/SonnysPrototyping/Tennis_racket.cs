using UnityEngine;
using System.Collections;

public class Tennis_racket : MonoBehaviour {

	public float stayTime;
	public Vector3 rota;
	private Quaternion target;
	private Quaternion start;
	private bool swinging;
	private float time;
	BoxCollider boxi;
	//Quaternion.Euler (0, 80, 300)

	// Use this for initialization
	void Start () {
		start = transform.rotation;
		target = Quaternion.Euler (rota);
		swinging = false;
		boxi = GetComponent<BoxCollider> ();
		//boxi.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!swinging && AudioAnalyzer.Instance.getMicLoudness() > 10) {
			swinging = true;
			time = Time.time;
		}

		if (swinging) {
			transform.rotation = Quaternion.Lerp (transform.rotation, target, 0.70F);
		} else if ((Time.time - time) > stayTime){
			transform.rotation = Quaternion.Lerp (transform.rotation, start, 0.25F);
		}

		if (transform.rotation == target) {
			boxi.enabled = true;
		}

		if (transform.rotation == target && (Time.time - time) > stayTime) {
			swinging = false;
			//boxi.enabled = false;
		}
	}
}

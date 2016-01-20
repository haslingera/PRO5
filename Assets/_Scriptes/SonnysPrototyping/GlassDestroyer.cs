using UnityEngine;
using System.Collections;

public class GlassDestroyer : MonoBehaviour {

	private StationaryMovement movement;
	private float totalScreamTime;
	private float startTime;
	private bool screaming = false;
	private bool stopscream = false;
	private float endTime;

	public GameObject broken;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
	}

	void Start () {
		movement = GetComponent<StationaryMovement> ();
	}

	void Update () {
		if ((AudioAnalyzer.Instance.getMicLoudness()+60) > 0f) {
			transform.position = new Vector3 (Mathf.Sin (Time.time*20)*(Time.time - startTime)/5f, transform.position.y,transform.position.z);
		}
		
		if (AudioAnalyzer.Instance.getMicLoudness() > 35f) {
			endTime = Time.time;
		}


		if (!screaming && AudioAnalyzer.Instance.getMicLoudness() > 35f) {
			screaming = true;
			startTime = Time.time;
		} else if(AudioAnalyzer.Instance.getMicLoudness() < 35f) {
			if (!stopscream && Time.time - startTime > 2.0f) {
				stopscream = true;
				endTime = Time.time;
			}
		}

		if (Time.time - endTime >= 1f) {
			screaming = false;
			startTime = Time.time;
		}

		//Debug.Log (Time.time - startTime);
		if (Time.time - startTime >= 5f) {
			GameObject clone;
			clone = Instantiate(broken, transform.position, broken.transform.rotation) as GameObject; 
			Destroy (gameObject);
		}
	}
}

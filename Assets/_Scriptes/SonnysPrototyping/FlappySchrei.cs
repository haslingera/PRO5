using UnityEngine;
using System.Collections;

public class FlappySchrei : MonoBehaviour {

	private StationaryMovement movement;

	public float maxSpeed;
	public float acceleration;
	public float gameSpeed = 1.0f;

	private Rigidbody rigbi;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
		gameSpeed = GameObject.Find("LevelLogic").GetComponent<LevelLogic>().numberOfBeats/8.0f;
	}

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		rigbi = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getMicLoudness () > 30f) {
			rigbi.AddForce (Vector3.up * 300 * gameSpeed);
		} else {
			rigbi.AddForce(Physics.gravity * rigbi.mass * gameSpeed);
		}
	}
}

using UnityEngine;
using System.Collections;

public class FlappySchrei : MonoBehaviour {

	private StationaryMovement movement;

	public float maxSpeed;
	public float acceleration;
	public float gameSpeed = 1.0f;
	private bool levelDidStart;

	private Rigidbody rigbi;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
		gameSpeed = GameLogic.Instance.getLevelSpeed();
		this.GetComponent<Rigidbody> ().useGravity = false;
	}

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		rigbi = GetComponent<Rigidbody> ();
		this.levelDidStart = false;
	}

	// register for broadcast event "OnLevelReadyToStart"
	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
	}

	// unregister for broadcast event "OnLevelReadyToStart"
	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
	}

	// receives broadcast event "OnLevelReadyToStart" from GameLogic
	private void levelReadyToStart() {
		this.levelDidStart = true;
		this.GetComponent<Rigidbody> ().useGravity = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.levelDidStart) {
			if (AudioAnalyzer.Instance.getMicLoudness () > 30f) {
				rigbi.velocity = Vector3.zero;
				rigbi.AddForce (Vector3.up * 4000 * gameSpeed);
			} else {
				rigbi.AddForce (Physics.gravity * rigbi.mass * 3 * (gameSpeed*gameSpeed));
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Collider>().CompareTag ("Obstacle")) {
			Destroy (gameObject);
			GameLogic.Instance.didFailLevel ();
		}
	}
}

using UnityEngine;
using System.Collections;

public class PingPongBall : MonoBehaviour {

	private StationaryMovement movement;
	private bool allowed1 = false;
	private bool allowed2 = false;

	void Start () {
		movement = GetComponent<StationaryMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (allowed1 && AudioAnalyzer.Instance.getMicLoudness() > 0.1f) {
			movement.constantSpeedX += 0.1f;
			movement.revertMovement();
			allowed1 = false;
		} else if (allowed2 && AudioAnalyzer.Instance.getMicLoudness() > 0.1f) {
			movement.constantSpeedX += 0.1f;
			movement.revertMovement();
			allowed2 = false;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("TennisPlayer1")) {
			allowed1 = true;
		} else if (other.CompareTag ("TennisPlayer2")) {
			allowed2 = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("TennisPlayer1")) {
			allowed1 = false;
		} else if (other.CompareTag ("TennisPlayer2")) {
			allowed2 = false;
		}
	}

}

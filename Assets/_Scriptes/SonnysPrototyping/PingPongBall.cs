using UnityEngine;
using System.Collections;

public class PingPongBall : MonoBehaviour {

	private StationaryMovement movement;
	private bool allowed1 = false;
	private bool allowed2 = false;

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		movement.constantSpeedX *= GameLogic.Instance.getLevelSpeed ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag ("TennisPlayer1") || other.gameObject.CompareTag ("TennisPlayer2")) {
			movement.revertMovement ();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("LevelLost")) {
			GameLogic.Instance.didFailLevel ();
		}
	}
}
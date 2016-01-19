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
		if (transform.position.y < 0.0f) {
			GameLogic.Instance.didFailLevel ();
		}
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag ("TennisPlayer1") || other.gameObject.CompareTag ("TennisPlayer2")) {
			movement.revertMovement ();
			Debug.Log ("now");
		}
	}
}
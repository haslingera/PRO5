using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour {

	private bool sawing = false;
	private StationaryMovement movement;
	public Vector3 pointA;
	public Vector3 pointB;

	private bool didStart = false;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
	}

	void Start() {
		movement = GetComponent<StationaryMovement> ();
	}

	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelStartEvent;
	}

	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelStartEvent;
	}

	void levelStartEvent() {
		this.didStart = true;
	}


	void FixedUpdate(){
		// only allow voice control when game officially started
		if (this.didStart) {
			if (!sawing && AudioAnalyzer.Instance.getPitch () > 10) {
				StartCoroutine (sawDaThing ());
			}
		}
	}

	IEnumerator sawDaThing() {	
		if (transform.position.x != pointB.x && AudioAnalyzer.Instance.getPitch () > 200) {
			sawing = true;
			movement.moveToPoint(new Vector3 (pointB.x, transform.position.y-0.14f, transform.position.z), 0f, 1f/GameLogic.Instance.getLevelSpeed());
			yield return new WaitForSeconds (1.0f/GameLogic.Instance.getLevelSpeed());
		} else if (transform.position.x != pointA.x && AudioAnalyzer.Instance.getPitch () < 200 && AudioAnalyzer.Instance.getPitch () > 10) {
			sawing = true;
			movement.moveToPoint(new Vector3 (pointA.x, transform.position.y-0.14f, transform.position.z), 0f, 1f/GameLogic.Instance.getLevelSpeed());
			yield return new WaitForSeconds (1.0f/GameLogic.Instance.getLevelSpeed());
		}
		sawing = false;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag ("Tree")) {
			other.gameObject.GetComponent<Rigidbody>().useGravity = true;
			other.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
			GameLogic.Instance.didFinishLevel ();
		}
	}

}

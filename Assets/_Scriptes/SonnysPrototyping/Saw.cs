using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour {

	private bool sawing = false;
	private StationaryMovement movement;
	public Vector3 pointA;
	public Vector3 pointB;


	void Awake() {
		AudioAnalyzer.Instance.Init ();
	}

	void Start() {
		movement = GetComponent<StationaryMovement> ();
	}

	void FixedUpdate(){
		Debug.Log (AudioAnalyzer.Instance.getPitch ());
		//Debug.Log (AudioAnalyzer.Instance.getPitch ());
		if (!sawing && AudioAnalyzer.Instance.getPitch() > 10) {
			StartCoroutine(sawDaThing());
		}
	}

	IEnumerator sawDaThing() {	
		if (transform.position.x != pointB.x && AudioAnalyzer.Instance.getPitch () > 200) {
			sawing = true;
			movement.moveToPoint(new Vector3 (pointB.x, transform.position.y-0.2f, transform.position.z), 0f, 1f);
			yield return new WaitForSeconds (1.0f);
		} else if (transform.position.x != pointA.x && AudioAnalyzer.Instance.getPitch () < 200 && AudioAnalyzer.Instance.getPitch () > 10) {
			sawing = true;
			movement.moveToPoint(new Vector3 (pointA.x, transform.position.y-0.2f, transform.position.z), 0f, 1f);
			yield return new WaitForSeconds (1.0f);
		}
		sawing = false;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag ("Tree")) {
			other.gameObject.GetComponent<Rigidbody>().useGravity = true;
			other.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
		}
	}

}

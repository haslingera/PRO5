using UnityEngine;
using System.Collections;

public class SchreckSchreiPlayer : MonoBehaviour {

	private StationaryMovement movement;
	private Player_Animation animationMouth;

	private bool moved = true;
	private RaycastHit hit;
	public Material [] materials;



	void Start () {
		movement = GetComponent<StationaryMovement> ();
		animationMouth = GetComponent<Player_Animation> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getMicLoudness() > 30f) {
			animationMouth.talkDirtyToMe = true;
			if (moved) {
				movement.moveToPoint(movement.moveTo, 0.0f, 0.1f);
				moved = false;
			}
		}
		if (transform.position == movement.moveTo) {
			
			if (Physics.Raycast(transform.position + new Vector3 (0.0f, 1.0f, 0.0f), new Vector3 (1.0f, 0.0f, 0.0f), out hit, 15.0f)) {
				//Debug.DrawLine (transform.position + new Vector3 (0.0f, 0.5f, 0.0f), hit.transform.position, Color.red);
				if (hit.collider.CompareTag("pupil") && hit.distance < 2.5f) {
					StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement>();
					pupil.stopMovement();

					GameLogic.Instance.didFinishLevel ();
				} else if (hit.collider.CompareTag("pupil")) {
					StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement>();
					pupil.stopMovement();
					GameLogic.Instance.didFailLevel ();
				}

			if (hit.collider.CompareTag("police")) {
					StationaryMovement police = hit.collider.gameObject.GetComponent<StationaryMovement>();
					police.stopMovement();
					GameLogic.Instance.didFailLevel ();
				}
			}
		}

	}
}

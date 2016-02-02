using UnityEngine;
using System.Collections;

public class SchreckSchreiPlayer : MonoBehaviour {

	private StationaryMovement movement;
	private Player_Animation animationMouth;
    private SkinnedMeshRenderer talk;

	private bool moved = true;
	private RaycastHit hit;
	public Material [] materials;
	private bool levelDidStart;

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		animationMouth = GetComponent<Player_Animation> ();
        talk = GetComponent<SkinnedMeshRenderer>();
		levelDidStart = false;
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
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.levelDidStart) {
		if (AudioAnalyzer.Instance.getMicLoudness() > 30f) {
			animationMouth.talkDirtyToMe = true;
			if (moved) {
				movement.moveToPoint(movement.moveTo, 0.0f, 0.1f);
				moved = false;
			}
		}
			if (transform.position == movement.moveTo) {
			
				if (Physics.Raycast (transform.position + new Vector3 (0.0f, 1.0f, 0.0f), new Vector3 (1.0f, 0.0f, 0.0f), out hit, 15.0f)) {
					//Debug.DrawLine (transform.position + new Vector3 (0.0f, 0.5f, 0.0f), hit.transform.position, Color.red);
					if (hit.collider.CompareTag ("pupil") && hit.distance < 2.5f) {
						StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement> ();
						pupil.stopMovement ();
						GameLogic.Instance.didFinishLevel ();
						Player_Animation ani = hit.collider.gameObject.GetComponent<Player_Animation> ();
						ani.talkDirtyToMe = true;
                        talk.SetBlendShapeWeight(0,0.0f);

						GameLogic.Instance.didFinishLevel ();
					} else if (hit.collider.CompareTag ("pupil")) {
						StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement> ();
						pupil.stopMovement ();
						GameLogic.Instance.didFailLevel ();
                        talk.SetBlendShapeWeight(0, 0.0f);
                    }

					if (hit.collider.CompareTag ("police")) {
						StationaryMovement police = hit.collider.gameObject.GetComponent<StationaryMovement> ();
						police.stopMovement ();
						GameLogic.Instance.didFailLevel ();
                        talk.SetBlendShapeWeight(0, 0.0f);
                    }
				}
			}
		}
	}
}

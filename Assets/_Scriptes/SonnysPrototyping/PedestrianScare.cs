using UnityEngine;
using System.Collections;

public class PedestrianScare : MonoBehaviour {

	public GameObject[] spawningObject;
	public Vector3 position;
	private int rand;
	private float gameSpeed = 1f;
	private bool levelDidStart;
	private bool spawned = false;


	void Start () {
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

	void Update() {
		if (levelDidStart) {
			if (!spawned) {
				gameSpeed = 5.0f/(GameLogic.Instance.getLevelSpeed());
				rand = Random.Range (0, 2);
				GameObject clone;
				clone = Instantiate(spawningObject[rand], position, spawningObject[rand].transform.rotation) as GameObject; 
				clone.GetComponent<StationaryMovement> ().time = gameSpeed;
				spawned = true;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("police")) {
			GameLogic.Instance.didFinishLevel();
		}

		if (other.CompareTag ("pupil")) {
			GameLogic.Instance.didFailLevel ();
		}
	}
}

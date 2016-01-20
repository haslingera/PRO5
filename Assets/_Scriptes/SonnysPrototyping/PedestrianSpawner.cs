using UnityEngine;
using System.Collections;

public class PedestrianSpawner : MonoBehaviour {

	public GameObject[] spawningObject;
	private int rand;
	private float gameSpeed = 1f;

	// Use this for initialization
	void Start () {
		gameSpeed = GameObject.Find ("LevelLogic").GetComponent<LevelLogic> ().numberOfBeats / 8.0f;
		rand = Random.Range (0, 1);
		Debug.Log (rand);
		GameObject clone;
		clone = Instantiate(spawningObject[0], transform.position, transform.rotation) as GameObject; 
		clone.GetComponent<StationaryMovement> ().constantSpeedX *= gameSpeed;
	}
}

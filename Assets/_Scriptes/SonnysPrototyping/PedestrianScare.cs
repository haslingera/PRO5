using UnityEngine;
using System.Collections;

public class PedestrianScare : MonoBehaviour {

	public GameObject[] spawningObject;
	public Vector3 position;
	private int rand;
	private float gameSpeed = 1f;

	void Start () {
		gameSpeed = 5f/(GameObject.Find ("LevelLogic").GetComponent<LevelLogic> ().numberOfBeats / 8.0f);
		rand = Random.Range (0, 2);
		GameObject clone;
		clone = Instantiate(spawningObject[rand], position, spawningObject[rand].transform.rotation) as GameObject; 
		clone.GetComponent<StationaryMovement> ().time = gameSpeed;
	}
}

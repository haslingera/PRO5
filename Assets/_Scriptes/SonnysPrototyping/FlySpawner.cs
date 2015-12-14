using UnityEngine;
using System.Collections;

public class FlySpawner : MonoBehaviour {

	public GameObject spawningObject;
	public float minTime = 1.0f;
	public float maxTime = 5.0f;
	public bool randomSpawn = false;
	public float rangeY;

	private bool isSpawning = false;
	private float gameSpeed = 1f;

	void Start () {
		gameSpeed = GameObject.Find("LevelLogic").GetComponent<LevelLogic>().numberOfBeats/8.0f;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!isSpawning) {
			isSpawning = true;
			StartCoroutine(SpawnObject(Random.Range(minTime/gameSpeed, maxTime/gameSpeed)));
		}
	}

	IEnumerator SpawnObject(float seconds)
	{	
		GameObject clone;
		yield return new WaitForSeconds(seconds);
		clone = Instantiate(spawningObject, transform.position + new Vector3(0,Random.Range (-rangeY, rangeY),0), spawningObject.transform.rotation) as GameObject; 
		clone.GetComponent<StationaryMovement> ().constantSpeedX *= gameSpeed;
		isSpawning = false;
	}

}
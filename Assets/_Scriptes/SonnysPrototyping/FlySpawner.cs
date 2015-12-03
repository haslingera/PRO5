using UnityEngine;
using System.Collections;

public class FlySpawner : MonoBehaviour {

	public GameObject spawningObject;
	private bool isSpawning = false;
	public float minTime = 1.0f;
	public float maxTime = 5.0f;
	public bool randomSpawn = false;
	public float rangeY;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isSpawning) {
			isSpawning = true;
			StartCoroutine(SpawnObject(Random.Range(minTime, maxTime)));
		}
	}

	IEnumerator SpawnObject(float seconds)
	{		
		yield return new WaitForSeconds(seconds);
		Instantiate(spawningObject, transform.position + new Vector3(0,Random.Range (-rangeY, rangeY),0), spawningObject.transform.rotation); 
		isSpawning = false;
	}

}
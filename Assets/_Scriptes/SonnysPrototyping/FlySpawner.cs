using UnityEngine;
using System.Collections;

public class FlySpawner : MonoBehaviour {

	public GameObject fly;
	private bool isSpawning = false;
	public float minTime = 1.0f;
	public float maxTime = 5.0f;
	
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
		Instantiate(fly, transform.position, transform.rotation); 
		isSpawning = false;
	}

}
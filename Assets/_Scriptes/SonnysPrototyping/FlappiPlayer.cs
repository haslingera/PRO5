using UnityEngine;
using System.Collections;

public class FlappiPlayer : MonoBehaviour {
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Collider>().CompareTag ("Obstacle")) {
			DestroyObject(gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class FlyDestroyer : MonoBehaviour {

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerStay(Collider other) {
		if (other.GetComponent<Collider>().CompareTag("Fly")) {
			DestroyObject (gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("meh");
		if (other.GetComponent<Collider>().CompareTag ("Fly")) {
			DestroyObject(gameObject);
		}
	}
}

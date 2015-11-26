using UnityEngine;
using System.Collections;

public class FlappiPlayer : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<Collider>().CompareTag ("Obstacle")) {
			DestroyObject(gameObject);
		}
	}
}

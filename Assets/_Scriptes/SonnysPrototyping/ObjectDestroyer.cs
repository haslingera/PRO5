using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour {

	void OnTriggerExit(Collider other) {
		DestroyObject (other.gameObject);
	}
}

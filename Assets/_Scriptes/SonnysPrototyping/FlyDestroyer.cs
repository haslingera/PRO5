using UnityEngine;
using System.Collections;

public class FlyDestroyer : MonoBehaviour {

	public string collidingTag;
	public float minPitch;
	public float maxPitch;

	void Start() {
		AudioAnalyzer.Instance.Init ();
	}

	void OnTriggerStay(Collider other) {
		//Debug.Log (AudioAnalyzer.Instance.getPitch ());
		if (AudioAnalyzer.Instance.getPitch() > minPitch && AudioAnalyzer.Instance.getPitch() < maxPitch) {
			if (other.GetComponent<Collider>().CompareTag(collidingTag)) {
				DestroyObject (other.gameObject);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class FlappySchrei : MonoBehaviour {

	private StationaryMovement movement;

	public float maxSpeed;
	public float acceleration;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
	}

	void Start () {
		movement = GetComponent<StationaryMovement> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getMicLoudness() > 0.01f) {
			if (movement.constantSpeedZ <= maxSpeed) {
				movement.constantSpeedZ += acceleration;
			}
		} else {
			if (movement.constantSpeedZ >= -maxSpeed) {
				movement.constantSpeedZ -= acceleration;
			}
		}
	}
}

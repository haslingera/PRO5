using UnityEngine;
using System.Collections;

public class SchreckSchreiPlayer : MonoBehaviour {

	private StationaryMovement movement;

	private bool moved = true;
	private RaycastHit hit;
	public Material [] materials;


	void Start () {
		movement = GetComponent<StationaryMovement> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GetComponent<InputAnalyser>().MicLoudness > 30f) {
			if (moved) {
				movement.moveToPoint(new Vector3 (-5.7f, 1.0f,-2.5f), 0.0f, 0.1f);
				moved = false;
			}
		}

		if (transform.position == new Vector3 (-5.7f, 1.0f, -2.5f)) {
			if (Physics.Raycast(transform.position, new Vector3 (1.0f, 0.0f, 0.0f), out hit, 10.0f)) {
				if (hit.collider.CompareTag("pupil") && hit.distance < 2.5f) {
					StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement>();
					pupil.stopMovement();
					Renderer mesh = hit.collider.gameObject.GetComponent<Renderer> ();
					mesh.material = materials[1];
				} else if (hit.collider.CompareTag("pupil")) {
					StationaryMovement pupil = hit.collider.gameObject.GetComponent<StationaryMovement>();
					pupil.stopMovement();
					Renderer mesh = hit.collider.gameObject.GetComponent<Renderer> ();
					mesh.material = materials[2];

				}
			}
		}

	}
}

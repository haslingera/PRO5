using UnityEngine;
using System.Collections;

public class Runner_Final : MonoBehaviour {

	private Rigidbody rigbi;
	private InputAnalyser sound;
	private float startTimer;
	private float EndTimer;
	private bool already = false;
	private Vector3 scale;
	
	void Start () {
		rigbi = GetComponent<Rigidbody> ();
		scale = this.transform.localScale;
	}
	

	void FixedUpdate () {


		if (AudioAnalyzer.Instance.getPitch () < 300f && AudioAnalyzer.Instance.getPitch () > 0) {
			duck ();
		}

		if (AudioAnalyzer.Instance.getPitch () > 500f) {
			jump ();
		}

		if (Time.time - startTimer > 1f) {
			already = false;
			transform.localScale = scale;
		}

		if (!already) {
			startTimer = Time.time;
		}

	}
	
	private void jump() {
		if (!already) {
			//transform.Translate (Vector3.up * 100 * Time.deltaTime, Space.World);
			rigbi.velocity += new Vector3(0f,5f,0f);
			already = true;
			startTimer = Time.time;
		}
	}

	private void duck() {
		if (!already) {
			Vector3 temp = scale;
			temp.y = 0.5f;
			transform.localScale = temp;
			transform.position -= new Vector3 (0f, 0.4f, 0f);
			already = true;
			startTimer = Time.time;
		}
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}
}

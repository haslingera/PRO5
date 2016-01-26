using UnityEngine;
using System.Collections;

public class Tennis_racket : MonoBehaviour {

	public float stayTime;
	public Vector3 rota;
	private Quaternion target;
	private Quaternion start;
	private bool swinging;
	private float time;
	private bool timeTaking;
	private float swingForwardAnimationTime;
	private float swingBackAnimationTime;
	private float animationStartTime;
	//BoxCollider boxi;
	//Quaternion.Euler (0, 80, 300)

	// Use this for initialization
	void Start () {
		start = transform.rotation;
		target = Quaternion.Euler (rota);
		swinging = false;
		timeTaking = true;

		this.swingForwardAnimationTime = 0.70f / GameLogic.Instance.getLevelSpeed ();
		this.swingBackAnimationTime = 0.25f / GameLogic.Instance.getLevelSpeed ();

		this.stayTime = this.stayTime / GameLogic.Instance.getLevelSpeed ();
		//boxi = GetComponent<BoxCollider> ();
		//boxi.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!swinging && AudioAnalyzer.Instance.getMicLoudness() > 30 && timeTaking) {
			swinging = true;
			time = Time.time;
			timeTaking = false;
			this.animationStartTime = Time.time;
		}

		if (swinging) {
			transform.rotation = Quaternion.Lerp (transform.rotation, target, (Time.time - this.animationStartTime) / this.swingForwardAnimationTime);//0.70F);
		} else if ((Time.time - time) > stayTime){
			transform.rotation = Quaternion.Lerp (transform.rotation, start, (Time.time - this.animationStartTime) / this.swingBackAnimationTime);//0.25F);
		}

		/*if (transform.rotation == target) {
			boxi.enabled = true;
		}*/

		if (transform.rotation == target && (Time.time - time) > (stayTime) && swinging == true) {
			swinging = false;
			this.animationStartTime = Time.time;
		}

		if (transform.rotation == start) {
			timeTaking = true;
		}
	}
}

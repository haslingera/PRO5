using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public int lives = 2;
	int count = 1;
	public bool rotate = false;
	float oldAngle = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if (rotate) {

			var targetPos = GameObject.Find ("Enemy").transform.position;
			targetPos.y = this.transform.position.y; //set targetPos y equal to mine, so I only look at my own plane
			var targetDir = Quaternion.LookRotation(targetPos - this.transform.position);
			this.transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 6*Time.deltaTime);
		}

	}

	float getDB(){

		float db;
		float endDB = 0;
		db = AudioAnalyzer.Instance.getMicLoudness();
		endDB = (Mathf.InverseLerp(-70, 40, db))*100;
		//Debug.Log(endDB);
		if (endDB < 40)
			return 0;
		return endDB;
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

}

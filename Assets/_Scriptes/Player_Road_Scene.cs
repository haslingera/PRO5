using UnityEngine;
using System.Collections;

public class Player_Road_Scene : MonoBehaviour {

	float db;
	Vector3 start;
	Vector3 end;
	bool move = false;
	Vector3 newPos;

	// Use this for initialization
	void Start () {

		start = this.transform.position;
		end = new Vector3 (18f, start.y,start.z);
		newPos = start;
	
	}
	
	// Update is called once per frame
	void Update () {

		db = AudioAnalyzer.Instance.getMicLoudness();

		if (!move) {
			if (db > 15f) {
				newPos.x += 0.2f;
				this.transform.position = newPos;
				move = true;
			}
		} else {
			if (db >15f) {
				move = false;
			}
		}

		if(this.transform.position.x > end.x){
			GameLogic.Instance.didFinishLevel();
		}
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

	public void  resetPlayer(){

		//this.transform.position = start;
		//newPos = start;
		GameLogic.Instance.didFailLevel ();

	}
}

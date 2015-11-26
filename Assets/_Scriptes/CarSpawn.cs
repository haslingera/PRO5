using UnityEngine;
using System.Collections;

public class CarSpawn : MonoBehaviour {
	
	GameObject player;
	float speed;
	Vector3 start;
	Vector3 end;
	bool onTheMove = false;

	// Use this for initialization
	void Start () {

		start = this.gameObject.transform.position;

		player = GameObject.Find ("Player");

		end = new Vector3 (this.gameObject.transform.position.x,this.gameObject.transform.position.y,-16f);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!onTheMove) {

			speed = Random.Range (1, 3);
			iTween.MoveTo (this.gameObject, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
			onTheMove = true;
		}

		if(this.gameObject.transform.position.z == -16){

			spawnCar();

		}
	}

	void spawnCar(){
		string temp = this.gameObject.name;
		GameObject clone = Instantiate (this.gameObject, this.start, transform.rotation) as GameObject;
		Destroy (this.gameObject);
		clone.name = temp;

	}
}

using UnityEngine;
using System.Collections;

public class CarSpawn : MonoBehaviour {
	
	GameObject player;
	float speed;
	Vector3 start;
	Vector3 end;
	bool onTheMove = true;
	GameObject clone;
	string name;

	// Use this for initialization
	void Start () {
	
		start = this.gameObject.transform.position;
		name = this.gameObject.name;
		player = GameObject.Find ("Player");
		end = new Vector3 (this.gameObject.transform.position.x,this.gameObject.transform.position.y,-16f);
		iTween.Stop (this.gameObject);
		iTween.Init (this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {

		if (!moves()) {
			speed = Random.Range (1, 3);
			Debug.Log (gameObject.name);
			iTween.MoveTo (this.gameObject, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
			this.onTheMove = false;
		}

		/*if((int)this.gameObject.transform.position.z == Random.Range(-4,2)){
			spawnCar();
		}*/
		if(this.gameObject.transform.position.z == -16){
			spawnCar();
			destroyCar();
		}
	}

	void spawnCar(){

		clone = Instantiate (this.gameObject, this.start, transform.rotation) as GameObject;
		//iTween.Stop(this.gameObject);
		//clone.GetComponent<CarSpawn>().setOnMove(false);
		//iTween.MoveTo (clone, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
		clone.name = name;
	}

	void destroyCar(){

		Destroy (this.gameObject);
		onTheMove = true;

	}

	void setOnMove(bool x){
		this.onTheMove = x;
	}

	bool moves(){
		if (this.gameObject.transform.position == start)
			return false;
		else
			return true;
	}

}

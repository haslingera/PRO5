using UnityEngine;
using System.Collections;

public class CarSpawn : MonoBehaviour {
	
	GameObject player;
	float speed;
	Vector3 start;
	Vector3 end;
	bool onTheMove = false;
	GameObject clone;
	string name;

	// Use this for initialization
	void Start () {

		iTween.Stop (this.gameObject);
		start = this.gameObject.transform.position;
		name = gameObject.name;
		player = GameObject.Find ("Player");
		end = new Vector3 (this.gameObject.transform.position.x,this.gameObject.transform.position.y,-16f);
		iTween.Init (this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {


		if (!this.moves () && !onTheMove) {
			this.speed = Random.Range (1, 3);
			iTween.MoveTo (this.gameObject, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
			this.onTheMove = true;
		}

		else if((int)this.gameObject.transform.position.z == Random.Range(-12,-4)){
			spawnCar();
		}
		else if(this.gameObject.transform.position.z == -16){
			//spawnCar();
			destroyCar();
		}
	}

	void spawnCar(){

		/*if(GameObject.Find ("Counter").GetComponent<counterCar> ().getCars (1) <=2 || 
		   GameObject.Find ("Counter").GetComponent<counterCar> ().getCars (2) <=2 ||
		   GameObject.Find ("Counter").GetComponent<counterCar> ().getCars(3) <=2){
			clone = Instantiate (Resources.Load("Car"+Random.Range(1,4)), this.start, transform.rotation) as GameObject;
			//iTween.Stop(this.gameObject);
			//clone.GetComponent<CarSpawn>().setOnMove(true);
			//iTween.MoveTo (clone, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
			clone.name = name;
		}		
		*/

		clone = Instantiate (Resources.Load("Car"+Random.Range(1,4)), this.start, transform.rotation) as GameObject;
		clone.name = name;
	}

	void destroyCar(){

		/*if (this.gameObject.name == "Clone1")
			GameObject.Find ("Counter").GetComponent<counterCar> ().subC1 ();
		if (this.gameObject.name == "Clone2")
			GameObject.Find ("Counter").GetComponent<counterCar> ().subC2 ();
		if (this.gameObject.name == "Clone3")
			GameObject.Find ("Counter").GetComponent<counterCar> ().subC3 ();
	*/
		Destroy (this.gameObject);
		if (clone != null) {
			clone.name = name;
			clone.GetComponent<CarSpawn> ().setOnMove (true);
		} else {
			spawnCar();
		}
		this.onTheMove = false;
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

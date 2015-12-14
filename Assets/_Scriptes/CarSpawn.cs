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
	float worldSpeed;

	// Use this for initialization
	void Start () {

		iTween.Stop (this.gameObject);
		start = this.gameObject.transform.position;
		name = gameObject.name;
		player = GameObject.Find ("Player");
		end = new Vector3 (this.gameObject.transform.position.x,this.gameObject.transform.position.y,-16f);
		iTween.Init (this.gameObject);
		worldSpeed = GameLogic.Instance.getLevelSpeed();

	}
	
	// Update is called once per frame
	void Update () {

        if (!this.moves() && !onTheMove) {
			levelSpeed();
			iTween.MoveTo (this.gameObject, iTween.Hash ("z", -16, "easetype", "linear", "time", speed));
            this.onTheMove = true;
        }

		else if((int)this.gameObject.transform.position.z == Random.Range(-12,-4)){
			spawnCar();
		}
		else if(this.gameObject.transform.position.z == -16){
			destroyCar();
		}
	}

	void spawnCar(){

		float dist = Vector3.Distance(this.gameObject.transform.position,this.start);

		if (dist > 50) {
			clone = Instantiate (Resources.Load ("Car" + Random.Range (1, 4)), this.start, transform.rotation) as GameObject;
			clone.name = name;
		}

	}

	void destroyCar(){

		if (clone != null) {
			clone.name = name;
			clone.GetComponent<CarSpawn> ().setOnMove (true);
		} else {
			spawnCar();
		}
		this.onTheMove = false;
		Destroy (this.gameObject);
	}

	void setOnMove(bool x){
		this.onTheMove = x;
	}

	bool moves(){
        Debug.Log(this.transform.position + " + " + start);
        if (this.transform.position == start)
			return false;
		else
			return true;
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Player")
		{
            StartCoroutine(GameObject.Find("Player").GetComponent<Player_Road_Scene>().resetPlayer());
		}
	}

	void levelSpeed(){

		//Debug.Log (worldSpeed);
		float temp = worldSpeed - 1;

		if(temp < 1)
			this.speed = Random.Range (2-temp, 7-temp);
		else
			this.speed = Random.Range (1, 6-temp);
	}

}

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
    bool levelDidStart = false;
    bool stop = false;

    // Use this for initialization
    void Start () {

		iTween.Stop (this.gameObject);
		start = this.gameObject.transform.position;
		name = gameObject.name;
		player = GameObject.Find ("Player");
		end = new Vector3 (this.gameObject.transform.position.x,this.gameObject.transform.position.y,-16f);
		iTween.Init (this.gameObject);
		worldSpeed = GameLogic.Instance.getLevelSpeed();
		levelDidStart = GameLogic.Instance.getLevelIsReadyToStart ();
        //Debug.Log(levelDidStart);
	}

    void OnEnable()
    {
        GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
    }

    // Unregister Broadcast "OnLevelReadyToStart" event
    void OnDisable()
    {
        GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
    }

    // receives OnLevelReadyToStart events
    private void levelReadyToStart()
    {
        this.levelDidStart = true;
    }

    // Update is called once per frame
    void Update () {

        if (levelDidStart)
        {
            //if level still runs
            if (!stop)
            {
                //if car moves 
                if (!this.moves() && !onTheMove)
                {
                    levelSpeed();
                    //Debug.Log ("car speed: " + speed);
                    iTween.MoveTo(this.gameObject, iTween.Hash("z", -16, "easetype", "linear", "time", speed));
                    this.onTheMove = true;
                }

                else if ((int)this.gameObject.transform.position.z == Random.Range(-12, -4))
                {
                    spawnCar();
                }
                else if (this.gameObject.transform.position.z == -16)
                {
                    destroyCar();
                }
            }
        }
	}

    //spawns a new car when called
	void spawnCar(){

		float dist = Vector3.Distance(this.gameObject.transform.position,this.start);

		if (dist > 48) {
            //Debug.Log("Hallo");
			clone = Instantiate (Resources.Load ("Car" + Random.Range (1, 4)), this.start, transform.rotation) as GameObject;
			clone.name = name;
		}

	}

    //destroys car when called
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

    //when car moves reutrns true
	bool moves(){
        //Debug.Log(this.transform.position + " + " + start);
        if (this.transform.position == start)
			return false;
		else
			return true;
	}

    //collision methode for player
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Player" && !stop)
		{
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            StartCoroutine(player.GetComponent<Player_Road_Scene>().resetPlayer());
            stopMovement();
            player.GetComponent<Player_Road_Scene>().setStop(true);
            //Debug.Log("Stop");
		}
	}

    //sets speed for the cars
	void levelSpeed(){

		float temp = worldSpeed - 1;
        //Debug.Log (worldSpeed);

        this.speed = Random.Range (Mathf.Max(2-(temp*1.5f), 1), 5-(temp * 2));
        //Debug.Log(speed);

	}

    //stops every movement
    void stopMovement() {
        iTween.Stop(this.gameObject);
        //iTween.MoveTo(this.gameObject, iTween.Hash("z", transform.position.z-3, "easetype", "linear", "time", speed));
        this.stop = true;
    }
}

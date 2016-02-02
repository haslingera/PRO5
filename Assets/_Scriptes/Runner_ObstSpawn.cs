using UnityEngine;
using System.Collections;

public class Runner_ObstSpawn : MonoBehaviour {

	private float[] height = new float[2];
	private int end = -70;
    private int spawnNew;
	private Vector3 start = new Vector3(-29f,0.13f,-7.4f);
    GameObject obstacle;
	GameObject clone;
	private bool spawned = false;
	private float defaultSpeed = -0.11f;
    GameObject player;
    bool stop = false;
    StationaryMovement stationaryMovement;

    // Use this for initialization
    void Start () {
		height[0] = 0.13f;
		height[1] = 2.1f;
        spawnNew = Random.Range(-40,-50);
        player = GameObject.Find("Player");
        stationaryMovement = this.GetComponent<StationaryMovement>();
        setSpeed();
	}
	
	// Update is called once per frame
	void Update () {

        isGoing();

		if ((int)transform.position.x == end) {
			destroyObst();
		}

        if (this.stop)
        {
            if ((int)transform.position.x == spawnNew && !spawned)
            {
                start.y = height[Random.Range(0, 2)];
                if (start.y == 2.1f)
                {
                    start.y = 4.264082e-17f;
                    clone = Instantiate(Resources.Load("Obstacle_big"), start, this.transform.rotation) as GameObject;
                    //clone.transform.localScale.Set(this.transform.localScale.x, 3.6f, this.transform.localScale.z);
                }
                else {
                    clone = Instantiate(Resources.Load("Obstacle"), start, this.transform.rotation) as GameObject;
                }
                clone.name = this.name;
                spawned = true;
            }
        }
	}

    //destroy the acutal gameobject
	private void destroyObst(){
		Destroy (this.gameObject);
	}

    //sets speed for the level
	public void setSpeed(){
		// set speed for obstacles based on level speed
		float levelSpeed = GameLogic.Instance.getLevelSpeed ();
		//stationaryMovement = GameObject.Find ("Obstacle").GetComponent<StationaryMovement> ();
		stationaryMovement.constantSpeedX = this.defaultSpeed * levelSpeed;
	}

    private void setSpeedStop()
    {
        stationaryMovement.stopMovement();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            player.GetComponent<Runner_Final>().endGame();
            setSpeedStop();
            this.stop = false;
        }
    }

    //looks if the level is still running
    private void isGoing()
    {
        //Debug.Log(GameLogic.Instance.getIsLevelActive());
        this.stop = GameLogic.Instance.getIsLevelActive();

        if(player.GetComponent<Runner_Final>().getStop())
        {
            setSpeedStop();
        }

    }

}

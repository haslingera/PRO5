using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {

    public bool randomSpawn = false;
    Vector3 playerPosition;
    Vector3 startPosition;
    Vector3 position;
    public float speed = 1;
	float distance = 0;
	float count;
	bool visible = true;
	public GameObject enem;
	public Collider objColl;
	private Camera cam;
	private Plane[] planes;
	private Vector3[] spawnPoint;

    // Use this for initialization
    void Start () {

		spawnPoint = new Vector3[5];
		spawnPoint [0] = new Vector3 (21.9f,3f,24f);
		spawnPoint [1] = new Vector3 (21.9f,3f,-24f);
		spawnPoint [2] = new Vector3 (-15f,3f,-24f);
		spawnPoint [3] = new Vector3 (18f,3f,1f);
		spawnPoint [4] = new Vector3 (-14f,3f,29f);

		position = spawnPoint [Random.Range(0,spawnPoint.Length)];
        startPosition = transform.position;
		playerPosition = GameObject.Find("Player").transform.position;
		enem = GameObject.Find ("Enemy");

		cam = Camera.main;
		planes = GeometryUtility.CalculateFrustumPlanes(cam);
		objColl = GetComponent<Collider>();

        if (randomSpawn)
        {
            transform.position = position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float tempDB = InputAnalyser.LevelMax();
		distance = Vector3.Distance (playerPosition, transform.position);

		if (GeometryUtility.TestPlanesAABB (planes, objColl.bounds))
			visible = true;
		else {
			visible = false;
			count++;
		}

		Debug.Log ("Attack "+count);

		if (count > 400) {
			attack();
			count = 0;
		} else {
			if (tempDB > -5) {
				moveAway();
			} else {
				transform.position = Vector3.MoveTowards (transform.position, playerPosition, speed * Time.deltaTime);
			}
		}
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            Destroy(GameObject.Find("Player"));
        }
    }

    void moveAway()
    {
		Vector3 player = GameObject.Find ("Player").transform.position - transform.position; 
		player = player.normalized;
			
		Vector3 temp = +1 * player;
		Vector3 goal = position;
		goal += temp;
		goal.y = 3;

		transform.position = Vector3.MoveTowards(transform.position, position+goal , speed * Time.deltaTime);
    }

	void attack(){

		position = spawnPoint [Random.Range(0,spawnPoint.Length)];
		transform.position = Vector3.MoveTowards(transform.position, position , 40 * Time.deltaTime);
		startPosition = transform.position;

		speed++;
	}
}



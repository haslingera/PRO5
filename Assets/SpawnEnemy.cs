using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {

    public bool randomSpawn = false;
    Vector3 playerPosition;
    Vector3 startPosition;
    Vector3 position;
    public float speed = 1;
	float distance = 0;
	float count = 0;

    // Use this for initialization
    void Start () {

        position = new Vector3(Random.Range(-20.0F, 20.0F), 3, Random.Range(-16F, 16F));
        startPosition = transform.position;
		playerPosition = GameObject.Find("Player").transform.position;

        if (randomSpawn)
        {
            transform.position = position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float tempDB = InputAnalyser.LevelMax();
		distance = Vector3.Distance (playerPosition, transform.position);

		if (count == 3) {
			attack();
			Debug.Log ("Attack");
		} else {
			if (tempDB > -5) {
				Debug.Log ("ha"+count);
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
		Debug.Log ("temp" + goal);
		goal += temp;

		transform.position = Vector3.MoveTowards(transform.position, position+goal , speed * Time.deltaTime);
    }

	void attack(){
		GameObject enemy = GameObject.Find ("Enemy");
		//iTween.MoveTo (enemy, new Vector3(Random.Range (enemy - 4, enemy + 4), 3, Random.Range (enemy + 4, enemy - 4)), 2);
	}
}

using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {

    public bool randomSpawn = false;
    Vector3 playerPosition;
    public float speed = 1;

    // Use this for initialization
    void Start () {

        Vector3 position = new Vector3(Random.Range(-20.0F, 20.0F), 3, Random.Range(-16F, 16F));
        playerPosition = GameObject.Find("Player").transform.position;

        if (randomSpawn)
        {
            transform.position = position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float tempDB = InputAnalyser.LevelMax();

        if(tempDB > -5)
        {

        }
        else if ()
        {

        }

        else {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        }

        
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            Destroy(GameObject.Find("Player"));
        }
    }
}

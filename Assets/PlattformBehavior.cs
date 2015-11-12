using UnityEngine;
using System.Collections;

public class PlattformBehavior : MonoBehaviour {

    Vector3 startPosition;
    public float speed = 3;
    bool onObject;
    float tempDB;

    // Use this for initialization
    void Start () {

        startPosition = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

        tempDB = InputAnalyser.LevelMax();
        if (onObject)
        {
            moveTower();
        }
        else
        {
            //moveRandomTower();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            onObject = true;
        }
        else
        {
            onObject = false;
        }
    }

    void moveTower()
    {
        if (startPosition.y > -6)
        {
            if (transform.position.y == GameObject.Find("Plattform2").transform.position.y)
            {
                moveCharakter();
            }
            else
            {
                if (tempDB < 5)
                {
                    startPosition.y += 1;
                    transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
                }
                else
                {
                    startPosition.y -= 1;
                    transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
                }
            }
        }
    }

    void moveRandomTower()
    {
        Vector3 temp = new Vector3(transform.position.x,Random.Range(-6F, 8F), transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, temp, 4 * Time.deltaTime);
    }

    void moveCharakter()
    {
        Vector3 tempPlayer = GameObject.Find("Player").transform.position;
        Vector3 tempNewPos = new Vector3(startPosition.x,tempPlayer.y,startPosition.z);
        GameObject.Find("Player").transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 2 * Time.deltaTime);
    }
}



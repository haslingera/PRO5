using UnityEngine;
using System.Collections;

public class PlattformBehavior : MonoBehaviour {

    Vector3 startPosition;
    public float speed = 3;
    bool onObject;
    float tempDB;
    Vector3 moveTo;
    GameObject next;

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
            moveRandomTower();
        }
    }

    void OnCollisionStay(Collision col)
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

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            onObject = false;
        }
    }

    void moveTower()
    {
        if (startPosition.y > -6 && startPosition.y < 8)
        {
            if ((int)transform.position.y == (int)GameObject.Find(getNext()).transform.position.y)
            {
                moveCharakter();
            }
            else
            {
                if (tempDB > -10)
                {
                    moveDown();
                }
                else
                {
                    moveUp();
                }
            }
        }
        else if(startPosition.y < -6)
        {
            moveUp();
        }
        else if(startPosition.y > 8)
        {
            moveDown();
        }
            
    }

    void moveRandomTower()
    {
        moveTo = new Vector3(transform.position.x,Random.Range(-6F, 8F), transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, moveTo, 4 * Time.deltaTime);
    }

    void moveCharakter()
    {
        Vector3 tempPlayer = GameObject.Find("Player").transform.position;
        Vector3 tempNewPos = new Vector3(GameObject.Find(getNext()).transform.position.x,tempPlayer.y, GameObject.Find(getNext()).transform.position.z);
        GameObject.Find("Player").transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 2 * Time.deltaTime);
    }

    void moveUp()
    {
        startPosition.y += 1;
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
    }

    void moveDown()
    {
        startPosition.y -= 1;
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
    }

    string getNext()
    {
        string sillyMeme = name;

        char temp;

        temp = sillyMeme[9];
        int bar = temp - '0';
        bar++;

        return "Plattform" + bar;
    }
}




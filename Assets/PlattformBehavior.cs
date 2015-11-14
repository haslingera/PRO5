using UnityEngine;
using System.Collections;

public class PlattformBehavior : MonoBehaviour {

    Vector3 startPosition;
    public float speed = 3;
    bool onObject;
    double tempDB;
    Vector3 moveTo;
    GameObject next;
    int counter = 0;

    // Use this for initialization
    void Start () {

        startPosition = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        tempDB = GameObject.Find("Audio Source").GetComponent<AudioAnalyzer>().getDecibel();
        Debug.Log(tempDB);

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
                if (tempDB > -10d)
                {
                    moveDown();
                }
                else if (tempDB < -60d)
                {
                    moveUp();
                }
            }
        }
        else if(startPosition.y == -6)
        {
            moveUp();
        }
        else if(startPosition.y == 8)
        {
            moveDown();
        }
            
    }

    void moveRandomTower()
    {
        if (counter == 0 || counter == 50) {
            counter = 0;
            moveTo = new Vector3(transform.position.x, Random.Range(-6F, 8F), transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, moveTo, 20 * Time.deltaTime);
            counter++;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTo, 20 * Time.deltaTime);
            counter++;
        }
    }

    //moves the charakter
    void moveCharakter()
    {
        Vector3 tempPlayer = GameObject.Find("Player").transform.position;
        Vector3 tempNewPos = new Vector3(GameObject.Find(getNext()).transform.position.x,tempPlayer.y, GameObject.Find(getNext()).transform.position.z);
        GameObject.Find("Player").transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 2 * Time.deltaTime);
    }

    //Moves tower up
    void moveUp()
    {
        startPosition.y += 1;
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
    }

    //moves tower down
    void moveDown()
    {
        startPosition.y -= 1;
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
    }

    //get the next tower name
    string getNext()
    {
        string sillyMeme = name;

        char temp;

        temp = sillyMeme[9];
        int bar = temp - '0';
        bar++;

        if(bar == 11)
        {
            return "Plattform10";
        }

        return "Plattform" + bar;
    }
}




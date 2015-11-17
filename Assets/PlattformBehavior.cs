using UnityEngine;
using System.Collections;

public class PlattformBehavior : MonoBehaviour {

    Vector3 startPosition;
    public float speed = 3;
    bool onObject;
    double DB;
    double frequ;
    Vector3 moveTo;
    GameObject next;
    int counter = 0;
    bool choosen = false;
    bool visited = false;

    // Use this for initialization
    void Start() {

        startPosition = transform.position;

    }

    // Update is called once per frame
    void Update() {

        DB = GameObject.Find("Audio Source").GetComponent<AudioAnalyzer>().getDecibel();
        frequ = GameObject.Find("Audio Source").GetComponent<AudioAnalyzer>().getFrequency();
        //Debug.Log(frequ);
        isVisited();

        if (onPlayer())
        {
            moveTower();
        }
        else if (!onPlayer() && visited)
        {
            Debug.Log("Stop");
            Stop();
        }
        else
        {
            if (!onPlayer()) {
                moveRandomTower();
            }
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


   bool onPlayer()
    {
        Vector3 tower = transform.position;
        Vector3 player = GameObject.Find("Player").transform.position;

        if(player.x >= tower.x-1 && player.x <= tower.x + 1)
        {
            //Debug.Log("On");
            return true;
        }
        else
        {
            //Debug.Log("Off");
            return false;
        }
    }

    void moveTower()
    {
            if ((int)transform.position.y == (int)GameObject.Find(getNext()).transform.position.y)
            {
                //Debug.Log("Hallo");
                choosen = true;
                moveCharakter();
            }
            else
            {
                if (frequ >= 400 && frequ <= 700)
                {
                    //Debug.Log("UP");
                    moveUp();
                }
                else if (frequ >= 0 && frequ <= 400)
                {
                    //Debug.Log("DOWN");
                    moveDown();
                }
            

        }
        /*else if (startPosition.y <= -6)
        {
            moveUp();
        }
        else if (startPosition.y >= 8)
        {
            moveDown();
        }*/

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
        freeze(GameObject.Find(getNext()));
        Vector3 tempPlayer = GameObject.Find("Player").transform.position;
        Vector3 tempNewPos = new Vector3(GameObject.Find(getNext()).transform.position.x, tempPlayer.y, GameObject.Find(getNext()).transform.position.z);
        GameObject.Find("Player").transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 5 * Time.deltaTime);
    }

    //Moves tower up
    void moveUp()
    {
        if (startPosition.y < 8)
        {
            startPosition.y += 1;
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
        }
    }

    //moves tower down
    void moveDown()
    {
        if (startPosition.y > -6)
        {
            startPosition.y -= 1;
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
        }
    }

    //get the next tower name
    string getNext()
    {
        string sillyMeme = name;

        char temp;

        temp = sillyMeme[9];
        int bar = temp - '0';
        bar++;

        if (bar == 11)
        {
            return "Plattform10";
        }


        return "Plattform" + bar;
    }

    private void freeze(GameObject tower)
    {
        //Debug.Log("Fahren");
        if (choosen) {
            float y = transform.position.y;
            tower.transform.position = new Vector3(tower.transform.position.x, y, tower.transform.position.z);
        }
  
    }

    private void Stop()
    {
        if (transform.position.y != 8) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 8F, transform.position.z), speed * Time.deltaTime);
        } 
    }

    private void isVisited()
    {
        Vector3 tower = transform.position;
        Vector3 player = GameObject.Find("Player").transform.position;

        if (player.x > tower.x + 1.5)
        {
            Debug.Log(name +" Visited");
            visited = true;
        }
        else
        {
            visited = false;
        }
    }
}




using UnityEngine;
using System.Collections;

public class PlattformBehavior : MonoBehaviour {

    Vector3 startPosition;
    public float speed = 3;
    bool onObject;
    float DB = 0;
    float frequ;
    Vector3 moveTo;
    GameObject next;
    int counter = 0;
    int first = 1;
    bool choosen = false;
    bool visited = false;
	GameObject Player;

    // Use this for initialization
    void Start() {

        startPosition = transform.position;
		Player = GameObject.Find ("Player");
		AudioAnalyzer.Instance.Init ();

    }

    // Update is called once per frame
    void Update() {

		DB = AudioAnalyzer.Instance.getPitch();//GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().MicLoudness;
		//frequ = GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().getPitch ();
		//Debug.Log (DB);

        isVisited();

		if (first == 1 && !GameObject.Find("Plattform1").GetComponent<PlattformBehavior>().onPlayer())
        {
            startGame();
        }
        else
        {
            first++;
            if (onPlayer())
            {
				//Debug.Log("Hallo");
                moveTower();
            }
			else if (!onPlayer() && visited)
            {
                Stop(8f);
            }
            else
            {
                if (!onPlayer() && !choosen)
                {
					//Debug.Log ("Random"+name);
                    //moveRandomTower();
                }
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
		Vector3 tow = this.transform.position;

		if(Player.transform.position.x == tow.x) //|| Player.transform.position.x >= tow.x-1 && Player.transform.position.x <= tow.x + 1)
        {
            //Debug.Log("On"+name);
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
		//Debug.Log("Hallo");
        if ((int)transform.position.y == (int)GameObject.Find (getNext ()).transform.position.y) {
			Vector3 temp = new Vector3(this.transform.position.x, GameObject.Find (getNext ()).transform.position.y,
			                           this.transform.position.z);
			transform.position = temp;
			GameObject.Find(getNext()).GetComponent<PlattformBehavior>().choosen = true;
			moveCharakter();

		} else if (transform.position.y > 3.10 && transform.position.y < 3.20 && this.name == "Plattform10") {
			endGame ();
		} else {
			if ((int)DB > 350) {
				//Debug.Log ("UP");
				moveUp ();
			}
			else if((int)DB == -1){
				//Do noting
			
			} else if ((int)DB < 350) {
				//Debug.Log ("DOWN");
				moveDown ();
			}
		}
            
    }

    void moveRandomTower()
    {
		if(!onPlayer()){
	        if (counter == 0 || counter == 50) {
	            counter = 0;
	            moveTo = new Vector3(transform.position.x, Random.Range(-6F, 8F), transform.position.z);
	            transform.position = Vector3.MoveTowards(transform.position, moveTo, 5 * Time.deltaTime);
	            counter++;
	        }
	        else
	        {
	            transform.position = Vector3.MoveTowards(transform.position, moveTo, 5 * Time.deltaTime);
	            counter++;
	        }
		}
    }

    //moves the charakter
	void  moveCharakter()
    {
        Vector3 tempPlayer = Player.transform.position;
        Vector3 tempNewPos = new Vector3(GameObject.Find(getNext()).transform.position.x, tempPlayer.y, GameObject.Find(getNext()).transform.position.z);

        freeze(GameObject.Find(getNext()));

		iTween.MoveTo(Player, tempNewPos, 2);

    }

    //Moves tower up
    void moveUp()
    {
        if (startPosition.y < 8)
        {
            startPosition.y += 0.1f;
			transform.position = startPosition;
            updatePlayer(1);
        }
    }

    //moves tower down
    void moveDown()
    {
        if (startPosition.y > -6)
        {
            startPosition.y -= 0.1f;
			transform.position = startPosition;
            updatePlayer(-1);
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
		if (GameObject.Find(getNext()).GetComponent<PlattformBehavior>().choosen) {
            float y = transform.position.y;
			//Debug.Log ("Freeze");
            tower.transform.position = Vector3.MoveTowards(tower.transform.position, tower.transform.position, speed * Time.deltaTime);
        }
  
    }

    private void Stop(float x)
    {
        if (this.transform.position.y != 8) {
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, x, transform.position.z), speed * Time.deltaTime);
        } 
    }

    private void isVisited()
    {
        Vector3 tower = this.transform.position;


        if (Player.transform.position.x > tower.x + 1.5)
        {
            //Debug.Log(name +" Visited");
            this.visited = true;
        }
        else
        {
            this.visited = false;
        }
    }

	private void  startGame()
    {
        Vector3 tempPlayer = Player.transform.position;
		Vector3 tempNewPos = new Vector3(GameObject.Find("Plattform1").transform.position.x, tempPlayer.y, GameObject.Find("Plattform1").transform.position.z);

        freeze(GameObject.Find("Plattform1"));
		Player.transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 1 * Time.deltaTime);
    }

    private void endGame()
    {
        Stop(this.transform.position.y);
        Vector3 tempPlayer = GameObject.Find("Player").transform.position;
        Vector3 tempNewPos = new Vector3(GameObject.Find("End").transform.position.x, tempPlayer.y, GameObject.Find("End").transform.position.z);
		iTween.MoveTo(Player, tempNewPos, 2);
        isVisited();
    }

    private void updatePlayer(int x)
    {
        Vector3 tower = this.transform.position;
        GameObject player = GameObject.Find("Player");
        Player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(tower.x, player.transform.position.y+x, tower.z), 10 *Time.deltaTime);
    }
}




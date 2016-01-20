using UnityEngine;
using System.Collections.Generic;

public class PlattformBehaviorAbsolute : MonoBehaviour {

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
	private Queue<int> pitchQueue;


    // Use this for initialization
    void Start() {

        startPosition = transform.position;
		Player = GameObject.Find ("Player");
		AudioAnalyzer.Instance.Init ();

		pitchQueue = new Queue<int> ();
		pitchQueue.Enqueue(350);
		pitchQueue.Enqueue(350);
		pitchQueue.Enqueue(350);
		pitchQueue.Enqueue(350);
		pitchQueue.Enqueue(350);
    }

    // Update is called once per frame
    void Update() {

		DB = AudioAnalyzer.Instance.getPitch();//GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().MicLoudness;
		//frequ = GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().getPitch ();
		Debug.Log (DB);

        isVisited();

        if (first == 1 && !GameObject.Find("Plattform1").GetComponent<PlattformBehaviorAbsolute>().onPlayer())
        {
            startGame();
        }
        else
        {
            first++;
            if (onPlayer())
            {
				Debug.Log("Hallo");
                moveTower();
            }
            else if (!onPlayer() && visited)
            {
                Stop(8f);
            }
            else
            {
                if (!onPlayer())
                {
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
        Vector3 tower = this.transform.position;

		if(Player.transform.position.x == tower.x || Player.transform.position.x >= tower.x-1 && Player.transform.position.x <= tower.x + 1)
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
		Debug.Log("y position: " + (int)transform.position.y);
        if (transform.position.y < GameObject.Find (getNext ()).transform.position.y + 0.1 &&
		    transform.position.y > GameObject.Find (getNext ()).transform.position.y - 0.1) {
			Vector3 temp = new Vector3(this.transform.position.x, GameObject.Find (getNext ()).transform.position.y,
			                           this.transform.position.z);
			transform.position = temp;
			this.choosen = true;
			moveCharakter();

		} else if (transform.position.y > 3.10 && transform.position.y < 3.20 && this.name == "Plattform10") {
			endGame ();
		} else {

			if ((int)DB != -1) {
				this.pitchQueue.Enqueue ((int)DB);

				int[] pitches = this.pitchQueue.ToArray ();
				int pitchSum = 0;
				foreach (int pitch in pitches) {
					pitchSum += pitch;
				}

				this.pitchQueue.Dequeue();

				int pitchAvg = pitchSum / pitches.Length;
				Debug.Log ("pitchAVG: " + pitchAvg);

				moveToPitch (pitchAvg);
			}
			/*
			if ((int)DB > 350) {
				//Debug.Log ("UP");
				moveUp ();
			}
			else if((int)DB == -1){
				//Do noting
			
			} else if ((int)DB < 350) {
				//Debug.Log ("DOWN");
				moveDown ();
			}*/
		}
            
    }

    void moveRandomTower()
    {
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

	void moveToPitch(float pitch) {
		// y position must be between [-6 | 8]
		// take frequency range from [160 | 500] 
		// means: f(160) = y(-6), f(500) = y(8)

		// bring pitch in range [0 | 340]
		pitch = pitch - 160;
		pitch = Mathf.Max (0, pitch);
		pitch = Mathf.Min (340, pitch);

		float yRange = 15;

		// map 330 frequencies to 14ys
		float y = yRange / 340.0f * pitch;
		startPosition.y = y - 7;
		transform.position = startPosition;

		Vector3 tower = this.transform.position;
		GameObject player = GameObject.Find("Player");
		Player.transform.position = new Vector3 (Player.transform.position.x, transform.position.y - ((transform.localScale.y) / 2.0f), Player.transform.position.z);
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




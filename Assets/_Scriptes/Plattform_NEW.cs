using UnityEngine;
using System.Collections;

public class Plattform_NEW : MonoBehaviour
{

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
    bool levelDidStart = false;
    GameObject[] platforms;

    // Use this for initialization
    void Start()
    {
        int a = 1;
        startPosition = transform.position;
        Player = GameObject.Find("Player");
        AudioAnalyzer.Instance.Init();

        platforms = new GameObject[10];

        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i] = GameObject.Find("Plattform" + a.ToString());
            //Debug.Log(platforms[i]);
            a++;
        }
    }

    void OnEnable()
    {
        GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
    }

    // Unregister Broadcast "OnLevelReadyToStart" event
    void OnDisable()
    {
        GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
    }

    // receives OnLevelReadyToStart events
    private void levelReadyToStart()
    {
        this.levelDidStart = true;
    }

    void Update()
    {

        if (levelDidStart)
        {
            DB = AudioAnalyzer.Instance.getPitch();//GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().MicLoudness;
                                                   //frequ = GameObject.Find ("Audio Source").GetComponent<InputAnalyser> ().getPitch ();
                                                  //Debug.Log (DB);

            if (first == 1 && counter == 0)
            {
                startGame();
            }
            else
            {
                Debug.Log("Ja herinnen");
                moveTower();
            }
        }

    }

    private void startGame()
    {
        Vector3 tempPlayer = Player.transform.position;
        Vector3 tempNewPos = new Vector3(platforms[0].transform.position.x, tempPlayer.y, platforms[0].transform.position.z);

        Player.transform.position = Vector3.MoveTowards(tempPlayer, tempNewPos, 10 * Time.deltaTime);

        if(Player.transform.position.x == platforms[0].transform.position.x)
        {
            first++; 
        }
        
    }

    void moveTower()
    {
        //Debug.Log("Hallo");
        if ((int)transform.position.y == (int)platforms[counter+1].transform.position.y)
        {
            Vector3 temp = new Vector3(this.transform.position.x, platforms[counter + 1].transform.position.y,
                                       this.transform.position.z);
            transform.position = temp;
            moveCharakter();

        }
        else if (transform.position.y > 3.10 && transform.position.y < 3.20 && counter == 9)
        {
            //endGame();
        }
        else {
            if ((int)DB > 350)
            {
                //Debug.Log ("UP");
                moveUp();
            }
            else if ((int)DB == -1)
            {
                //Do noting

            }
            else if ((int)DB < 350)
            {
                //Debug.Log ("DOWN");
                moveDown();
            }
        }
    }

    void moveUp()
    {
        if (platforms[counter].transform.position.y < 8)
        {
            Vector3 temp = new Vector3(platforms[counter].transform.position.x, platforms[counter].transform.position.y + 0.1f, platforms[counter].transform.position.z);
            platforms[counter].transform.position = temp;
        }
    }

    //moves tower down
    void moveDown()
    {
        if (platforms[counter].transform.position.y > -6)
        {
            Vector3 temp = new Vector3(platforms[counter].transform.position.x, platforms[counter].transform.position.y - 0.1f, platforms[counter].transform.position.z);
            platforms[counter].transform.position = temp;
        }
    }

    void moveCharakter()
    {
        Vector3 tempPlayer = Player.transform.position;
        Vector3 tempNewPos = new Vector3(platforms[counter+1].transform.position.x, tempPlayer.y, platforms[counter + 1].transform.position.z);

        //freeze(platforms[counter+1]);

        iTween.MoveTo(Player, tempNewPos, 2);

    }

}

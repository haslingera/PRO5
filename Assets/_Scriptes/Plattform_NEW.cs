using UnityEngine;
using System.Collections;

public class Plattform_NEW : MonoBehaviour
{

    Vector3 startPosition;
    float DB = 0;
    float frequ;
    int counter = 0;
    int first = 1;
    bool onTheMove = false;
    public bool moveThemRandom = true;
    GameObject Player;
    bool levelDidStart = false;
    GameObject[] platforms;
    Vector3[] moveRandom;
    float speed = 0;
    float randspeed = 0;

    // Use this for initialization
    void Start()
    {
        int a = 1;
        startPosition = transform.position;
        Player = GameObject.Find("Player");
        AudioAnalyzer.Instance.Init();

        platforms = new GameObject[10];
        moveRandom = new Vector3[10];

        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i] = GameObject.Find("Plattform" + a.ToString());
            a++;
        }

        for (int x = 1; x < platforms.Length-1; x++)
        {
            Vector3 temp = new Vector3(platforms[x].transform.position.x, Random.Range(-7F, 8F), platforms[x].transform.position.z);
            platforms[x].transform.position = temp;
        }

        setSpeed();

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
                                                   //Debug.Log (DB)
            if (first == 1 && counter == 0)
            {
                startGame();
            }
            else
            {
                updateCounter();
                moveTower();
                if (counter > 0 && moveThemRandom)
                {
                    for (int i = 0; i < counter; i++)
                    {
                        moveRandomTower(platforms[i], i);
                    }
                    for (int x = 8; x > counter+1; x--)
                    {
                        moveRandomTower(platforms[x], x);
                    }
                }
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

        if (counter == 9)
        {
            if (platforms[counter].transform.position.y > 3.10 && platforms[counter].transform.position.y < 3.20 && counter == 9)
            {
                endGame();
            }
            else
            {
                    if ((int)DB > 350)
                    {
                        moveUp();
                    }
                    else if ((int)DB == -1)
                    {
                        //Do noting

                    }
                    else if ((int)DB < 350)
                    {
                        moveDown();
                    }
            }
        }
        else {
            moveShit();
        }
    }

    private void moveUp()
    {
        if (platforms[counter].transform.position.y < 8)
        {
            Vector3 temp = new Vector3(platforms[counter].transform.position.x,
                                       platforms[counter].transform.position.y + speed, platforms[counter].transform.position.z);
            platforms[counter].transform.position = temp;
            updatePlayer(0.1f);
        }
    }
    
    private void moveDown()
    {
        if (platforms[counter].transform.position.y > -6)
        {
            Vector3 temp = new Vector3(platforms[counter].transform.position.x, 
                                       platforms[counter].transform.position.y - speed, platforms[counter].transform.position.z);
            platforms[counter].transform.position = temp;
            updatePlayer(-0.5f);
        }
    }

    void moveCharakter()
    {
        if (!onTheMove)
        {
            Vector3 tempPlayer = Player.transform.position;
            Vector3 tempNewPos = new Vector3(platforms[counter + 1].transform.position.x, tempPlayer.y, platforms[counter + 1].transform.position.z);

            iTween.MoveTo(Player, tempNewPos, 1);
            onTheMove = true;
        }
        if (isThere())
        {
            onTheMove = false;
            if(counter < 9)
                counter++;
        }

    }

    private bool isThere()
    {
        return Player.transform.position.x == platforms[counter+1].transform.position.x;
    }

    private void updatePlayer(float x)
    {
        Vector3 tower = platforms[counter].transform.position;
        Player.transform.position = Vector3.MoveTowards(Player.transform.position, new Vector3
                                                        (tower.x, Player.transform.position.y + x, tower.z), 10 * Time.deltaTime);
    }

    private void moveShit()
    {

        if (platforms[counter].transform.position.y < platforms[counter + 1].transform.position.y+0.3 &&
            platforms[counter].transform.position.y > platforms[counter + 1].transform.position.y - 0.3)
        {
            Vector3 temp = new Vector3(platforms[counter].transform.position.x, platforms[counter + 1].transform.position.y,
                                       platforms[counter].transform.position.z);
            platforms[counter].transform.position = temp;
            moveCharakter();

        }
        else {
            if (!onTheMove)
            {
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

    }

    private void endGame()
    {
        Vector3 tempEnd = GameObject.Find("End").transform.position;
        Vector3 tempNewPos = new Vector3(tempEnd.x, Player.transform.position.y,tempEnd.z);
        iTween.MoveTo(Player, tempNewPos, 2);

        if(Player.transform.position.x >= 7.5)
        {
            GameLogic.Instance.didFinishLevel();
        }

    }

    private void moveRandomTower(GameObject tower, int i)
    {
            if (tower.transform.position.y == moveRandom[i].y || moveRandom[i].x == 0.0)
            {
                moveRandom[i] = new Vector3(tower.transform.position.x, Random.Range(-6F, 8F), tower.transform.position.z);
                tower.transform.position = Vector3.MoveTowards(tower.transform.position, moveRandom[i], randspeed * Time.deltaTime);
            }
            else
            {
                tower.transform.position = Vector3.MoveTowards(tower.transform.position, moveRandom[i], randspeed * Time.deltaTime);
            }
        }

    private void updateCounter()
    {
        for(int i = 0; i < platforms.Length; i++)
        {
            if(Player.transform.position.x == platforms[i].transform.position.x && !onTheMove)
            {
                counter = i;
                //Debug.Log(counter);
            }
        }
    }

    private void setSpeed()
    {
        float temp = GameLogic.Instance.getLevelSpeed();

        if(temp == 1.0)
        {
            speed = 0.1f;
            randspeed = 4;
        }

        else if (temp > 1.0 && temp < 2.0)
        {
            speed = 0.15f;
            randspeed = 6;
        }

        else if (temp == 2.0)
        {
            speed = 0.2f;
            randspeed = 8;
        }

    }

}



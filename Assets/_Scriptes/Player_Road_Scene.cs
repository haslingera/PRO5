using UnityEngine;
using System.Collections;

public class Player_Road_Scene : MonoBehaviour {

	float db;
	Vector3 start;
	Vector3 end;
	bool move = false;
	Vector3 newPos;
    bool levelDidStart = false;
	private float levelSpeed;
    bool stop = false;

    // Use this for initialization
    void Start () {
		start = this.transform.position;
		end = new Vector3 (22.8f, start.y,start.z);
		newPos = start;
		this.levelSpeed = GameLogic.Instance.getLevelSpeed ()+1.0f;
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

    // Update is called once per frame
    void Update () {

        if (levelDidStart)
        {
            if (!stop)
            {
                db = AudioAnalyzer.Instance.getMicLoudness();

                if (!move)
                {
                    if (db > 15f)
                    {
                        newPos.x += (0.3f);
                        //this.transform.position = newPos;
                        transform.position = Vector3.MoveTowards(transform.position, newPos, 10*levelSpeed * Time.deltaTime);
                        move = true;
                    }
                }
                else {
                    if (db > 15f)
                    {
                        move = false;
                    }
                }

                if (this.transform.position.x >= end.x)
                {
                    GameLogic.Instance.didFinishLevel();
                    stop = true;
                }
            }
        }
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

    public IEnumerator resetPlayer(){

        yield return new WaitForSeconds(1);
        GameLogic.Instance.didFailLevel ();

	}

    public void setStop(bool temp)
    {
        this.stop = temp;
    }
}

using UnityEngine;
using System.Collections;

public class Player_TodSzene : MonoBehaviour {


	//public int lives = 2;
	public bool  rotate = false;
    bool levelDidStart = false;
    GameObject enem;
    // Use this for initialization
    void Start () {
		this.levelDidStart = GameLogic.Instance.getLevelIsReadyToStart ();

        enem = Instantiate(Resources.Load("Enemy" + Random.Range(1, 3)), new Vector3(21.9f, 3f, -18f), transform.rotation) as GameObject;
        enem.name = "Enemy";
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
            if (rotate)
            {
                var targetPos = GameObject.Find("Enemy").transform.position;
                targetPos.y = this.transform.position.y; //set targetPos y equal to mine, so I only look at my own plane
                var targetDir = Quaternion.LookRotation(targetPos - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 6 * Time.deltaTime);
            }
        }
	}

	float getDB(){

		float db;
		float endDB = 0;
		db = AudioAnalyzer.Instance.getMicLoudness();
		endDB = (Mathf.InverseLerp(-70, 40, db))*100;
		//Debug.Log(endDB);
		if (endDB < 40)
			return 0;
		return endDB;
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

}

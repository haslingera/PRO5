using UnityEngine;
using System.Collections;

public class Runner_Final : MonoBehaviour {

	private Rigidbody rigbi;
	private InputAnalyser sound;
	private float startTimer;
	private float EndTimer;
	private bool already = false;
	private Vector3 scale;
	private Vector3 start;
    GameObject obstacle;
	private bool onStart = true;
	private bool levelDidStart;
    bool stop = false;
    int frequ;

	void Start () {
		rigbi = GetComponent<Rigidbody> ();
		scale = this.transform.localScale;
		start = new Vector3 (-44.51059f, 0.274f, -3.86591f);
		this.levelDidStart = false;

        Vector3 start2 = new Vector3(-29f, 0.13f, -7.4f);

        start2.y = Random.Range(0, 2);
        if (start2.y == 1)
        {
            start2.y = 4.264082e-17f;
            obstacle = Instantiate(Resources.Load("Obstacle_big"), start2, this.transform.rotation) as GameObject;
            obstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
            //clone.transform.localScale.Set(this.transform.localScale.x, 3.6f, this.transform.localScale.z);
        }
        else {
            obstacle = Instantiate(Resources.Load("Obstacle"), start2, this.transform.rotation) as GameObject;
            obstacle.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        obstacle.name = "Obstacle";

    }

	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
	}

	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
	}

	// receive events from OnLevelReadyToStart
	private void levelReadyToStart() {
		this.levelDidStart = true;
	}
	

	void FixedUpdate () {
        if (stop)
        {
            this.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0.0f);
        }

        if (this.levelDidStart)
        {
            if (onStart)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, start, 0.3f);
                if (start.x == this.transform.position.x)
                {
                    onStart = false;
                }
            }

            if (!stop)
            {
                frequ = (int)AudioAnalyzer.Instance.getPitch();
                Debug.Log(frequ);

                if (!onStart) {
                    if(frequ == -1)
                    {
                        //Do nothing
                    }

                    if (frequ < 350f && frequ > 0)
                    {
                        duck();
                        //Debug.Log("duck");
                    }

                    if (frequ > 400f)
                    {
                        jump();
                        //Debug.Log("jump");
                    }

                    if (Time.time - startTimer > 1.5f)
                    {
                        already = false;
                        transform.localScale = scale;
                    }

                    if (!already)
                    {
                        startTimer = Time.time;
                    }
                }
                /*if (!onStart && (int)start.x != (int)this.transform.position.x) {
				    StartCoroutine (endGame ());
			    }
                if (this.transform.eulerAngles.x > 50 & this.transform.eulerAngles.x < 100)
                {
                    //Debug.Log (this.transform.eulerAngles.x);
                }*/
            }
        }
	}
	
    //jump method 
	private void jump() {
		if (!already) {
			//transform.Translate (Vector3.up * 100 * Time.deltaTime, Space.World);
			rigbi.velocity += new Vector3(0f,6f,0f);
			already = true;
			startTimer = Time.time;
		}
	}

    //duck method
	private void duck() {
		if (!already) {
			Vector3 temp = scale;
			temp.y = 0.5f;
			transform.localScale = temp;
			transform.position -= new Vector3 (0f, 0.4f, 0f);
			already = true;
			startTimer = Time.time;
		}
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

    //ends the game and sets stop boolean
	public IEnumerator endGame(){
        stop = true;
        yield return new WaitForSeconds(1);
		GameLogic.Instance.didFailLevel ();
	}

}

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
	private bool onStart = true;
	private bool levelDidStart;
    bool stop = false;

	void Start () {
		rigbi = GetComponent<Rigidbody> ();
		scale = this.transform.localScale;
		start = new Vector3 (-44.51059f, -0.03087401f,-3.86591f);
		this.levelDidStart = false; // TODO: set to false when level is ready for distribution
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

        if (this.levelDidStart)
        {

            if (!stop)
            {
                if (onStart)
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, start, 0.1f);
                    if (start.x == this.transform.position.x)
                    {
                        onStart = false;
                    }
                }


                if (AudioAnalyzer.Instance.getPitch() < 300f && AudioAnalyzer.Instance.getPitch() > 0)
                {
                    duck();
                }

                if (AudioAnalyzer.Instance.getPitch() > 500f)
                {
                    jump();
                }

                if (Time.time - startTimer > 1f)
                {
                    already = false;
                    transform.localScale = scale;
                }

                if (!already)
                {
                    startTimer = Time.time;
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
	
	private void jump() {
		if (!already) {
			//transform.Translate (Vector3.up * 100 * Time.deltaTime, Space.World);
			rigbi.velocity += new Vector3(0f,6f,0f);
			already = true;
			startTimer = Time.time;
		}
	}

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

	public IEnumerator endGame(){
        stop = true;
		yield return new WaitForSeconds(1);
		GameLogic.Instance.didFailLevel ();
	}

}

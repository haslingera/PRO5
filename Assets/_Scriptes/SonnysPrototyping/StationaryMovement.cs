using UnityEngine;
using System.Collections;

public class StationaryMovement : MonoBehaviour {

	public float constantSpeedX;
	public float constantSpeedY;
	public float constantSpeedZ;


	public bool movementToPoint = false;
	public Vector3 moveTo;
	public float speed;
	public bool timeMovement;
	public float time;

	private Vector3 moveFrom;
	private Rigidbody rb;
	private float startTime;
	private float journeyLength;
	private float direction = 1;

	private bool levelDidStart;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		this.levelDidStart = GameLogic.Instance.getLevelIsReadyToStart();//GameLogic.Instance.getLevelIsReadyToStart (); // for testing set this variable to true

		GameLogic.Instance.OnShowLevelInstructions += showLevelInstructions;
		moveToPoint (transform.position, moveTo, this.speed, this.time);
	}

	public void showLevelInstructions() {
		
	}

	// Register Broadcast "OnLevelReadyToStart" event
	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
		//GameLogic.Instance.OnShowLevelInstructions += showLevelInstructions;
		GameLogic.Instance.OnHideLevelInstructions += hideLevelInstructions;

		this.speed = this.speed * GameLogic.Instance.getLevelSpeed ();
		this.constantSpeedX *= GameLogic.Instance.getLevelSpeed ();
		this.constantSpeedY *= GameLogic.Instance.getLevelSpeed ();
		this.constantSpeedZ *= GameLogic.Instance.getLevelSpeed ();
	} 

	// Unregister Broadcast "OnLevelReadyToStart" event
	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
		//GameLogic.Instance.OnShowLevelInstructions -= showLevelInstructions;
		GameLogic.Instance.OnHideLevelInstructions -= hideLevelInstructions;
	}

	// receives OnLevelReadyToStart events
	private void levelReadyToStart() {
		this.levelDidStart = true;
	}

	/*private void showLevelInstructions() {
		Debug.Log ("show Level Instructions");
	}*/

	private void hideLevelInstructions() {
		Debug.Log ("hide Level Instructions");
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (this.levelDidStart) {
			if (!movementToPoint) {			
				transform.position += new Vector3 (constantSpeedX, constantSpeedY, constantSpeedZ) * direction;
			} else if (journeyLength > 0) {
				float distCovered = 0;
				if (timeMovement && time > 0) {
					distCovered = (Time.time - startTime) * (journeyLength / time);
					float fracJourney = distCovered / journeyLength;
					transform.position = Vector3.Lerp (moveFrom, moveTo, fracJourney);
				} else if (speed > 0) {
					distCovered = (Time.time - startTime) * speed;
					float fracJourney = distCovered / journeyLength;
					transform.position = Vector3.Lerp (moveFrom, moveTo, fracJourney);
				}
			}
		}
	}

	public void revertMovement(){
		direction *= -1.0f;
	}

	public void moveToPoint(Vector3 pointA, Vector3 pointB, float speed, float time) {
		moveFrom = pointA;
		moveTo = pointB;
		this.speed = speed;
		this.time = time;
		journeyLength = Vector3.Distance (pointA, pointB);
		startTime = Time.time;
	}

	public void moveToPoint(Vector3 pointB, float speed, float time) {
		moveFrom = transform.position;
		moveTo = pointB;
		this.speed = speed;
		this.time = time;
		journeyLength = Vector3.Distance (moveFrom, pointB);
		startTime = Time.time;
	}

	public void stopMovement() {
		constantSpeedX = 0;
		constantSpeedY = 0;
		constantSpeedZ = 0;
		moveToPoint (transform.position, 0, 0);
	}
}

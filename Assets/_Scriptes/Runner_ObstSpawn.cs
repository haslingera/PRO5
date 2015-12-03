using UnityEngine;
using System.Collections;

public class Runner_ObstSpawn : MonoBehaviour {

	private float[] height = new float[2];
	private int end = -52;
	private int spawnNew = -45;
	private Vector3 start = new Vector3(-29f,0.13f,-7.4f);
	GameObject clone;
	private bool spawned = false;

	// Use this for initialization
	void Start () {
		height[0] = 0.13f;
		height[1] = 2.1f;
		setSpeed ();

	}
	
	// Update is called once per frame
	void Update () {

		if ((int)transform.position.x == end) {
			destroyObst();
		}

		if ((int)transform.position.x == spawnNew && !spawned) {
			start.y = height[Random.Range(0,2)];
			clone = Instantiate (Resources.Load ("Obstacle"), start, transform.rotation) as GameObject;
			clone.name = this.name;
			spawned = true;
		}
	
	}

	void destroyObst(){
		Destroy (this.gameObject);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Player")
		{
			//endGame or subtract Life
		}
	}

	void setSpeed(){

		GameObject.Find ("Obstacle").GetComponent<StationaryMovement> ().constantSpeedX = -0.2f;

	}

}

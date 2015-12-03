using UnityEngine;
using System.Collections;

public class Runner_ObstSpawn : MonoBehaviour {

	private float[] height = new float[2];
	private float end = -52;

	// Use this for initialization
	void Start () {
		height[0] = 0.13f;
		height[1] = 2.1f;

	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.x == end) {
			destroyObst();
		}
	
	}

	void destroyObst(){

	}


}

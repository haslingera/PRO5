using UnityEngine;
using System.Collections;

public class CapPropeller : MonoBehaviour {

    public float rot = 0.1f;
    float rotChange = 0.0f;
    Vector3 startL;
    Vector3 startR;
    GameObject left;
    GameObject right;

    // Use this for initialization
    void Start () {

        left = GameObject.Find("left_part");
        right = GameObject.Find("right_part");

        startL = left.transform.position;
        startR = right.transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        //this.transform.rotation = Quaternion.EulerAngles(0.0f, rotChange, 0.0f);
        left.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
        right.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
        rotChange += rot;

        left.transform.position = startL;

        right.transform.position = startR;
    }
}

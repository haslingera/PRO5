using UnityEngine;
using System.Collections;

public class CapPropeller : MonoBehaviour {

    public float rot = 0.1f;
    float rotChange = 0.0f;
    Vector3 startL;
    Vector3 startR;
    GameObject middle;
    GameObject propeller;

    // Use this for initialization
    void Start () {

        middle = GameObject.Find("middle_part");

        propeller = GameObject.Find("propeller");

    }
	
	// Update is called once per frame
	void Update () {



        //this.transform.rotation = Quaternion.EulerAngles(0.0f, rotChange, 0.0f);
        transform.RotateAround(middle.transform.position, Vector3.up, 80 * Time.deltaTime);
        rotChange += rot;
        /*
        left.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
        right.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
        

        //Vector3 temp = middle.transform.position;
        //temp.y = 3.176f;
        //temp.z = middle.transform.position.z - 0.5f;

        //left.transform.position = middle.transform.position;

        //right.transform.position = middle.transform.position;*/
    }
}

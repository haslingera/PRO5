using UnityEngine;
using System.Collections;

public class ObsatcleBehaviour : MonoBehaviour {

    GameObject obstacle;
    public Renderer rend;
    int count = 0;

    // Use this for initialization
    void Start () {
        obstacle = GameObject.Find("Obstacle");
        rend = GetComponent<Renderer>();
        rend.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        /* Find out whether current second is odd or even
        bool oddeven = Mathf.FloorToInt(Time.time) % 2 == 0;

        // Enable renderer accordingly
        rend.enabled = oddeven;*/

        if(count < 50)
        {
            count += 1;
        }
        else
        {
            count = 0;
            rend.enabled = false;
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Torus")
        {
            rend.enabled = true;
        }
    } 

}

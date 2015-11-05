using UnityEngine;
using System.Collections;

public class ObsatcleBehaviour : MonoBehaviour {

    GameObject obstacle;
    public Renderer rend;
    int count = 0;
    ButtonOnClick red;
    ButtonOnClick yellow;
    ButtonOnClick cyan;

    // Use this for initialization
    void Start () {
        obstacle = GameObject.Find("Obstacle");
        rend = GetComponent<Renderer>();
        rend.material.color = Color.white;
        //rend.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        cyan = GameObject.Find("CYAN").GetComponent<ButtonOnClick>();
        red = GameObject.Find("RED").GetComponent<ButtonOnClick>();
        yellow = GameObject.Find("YELLOW").GetComponent<ButtonOnClick>();

        /* Find out whether current second is odd or even
        bool oddeven = Mathf.FloorToInt(Time.time) % 2 == 0;

        // Enable renderer accordingly
        rend.enabled = oddeven;*/

        if (count < 50)
        {
            count += 1;
        }
        else
        {
            count = 0;
            rend.material.color = Color.white;
        }

    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "Torus")
        {

            if (red.getColor() == Color.red)
            {
                if (gameObject.name == "Obstacle_red")
                {
                    rend.material.color = Color.red;
                }
            }

            else if (yellow.getColor() == Color.yellow)
            {
                if (gameObject.name == "Obstacle_yellow")
                {
                    rend.material.color = Color.yellow;
                }
            }

            else if (cyan.getColor() == Color.cyan)
            {
                if (gameObject.name == "Obstacle_cyan")
                {
                    rend.material.color = Color.cyan;
                }

            }
        }
    }


}



 using UnityEngine;
 using System.Collections;

public class PointAndClick : MonoBehaviour
{

    Vector3 targetPosition;
    public float speed = 0.1f;
    private Vector3 target;
    float y;
    Ray ray;
    RaycastHit hit;
    public Vector3 startPosition = new Vector3(-0.2f, 1.51f, -1.4f);
    public Quaternion rotation;
    public bool onPlayer = false;
    int count = 0;

    void Start()
    {
        targetPosition = transform.position;
        y = targetPosition.y;
        rotation.x = 0.0f;
        rotation.y = 0.0f;
        rotation.z = 0.0f;
    }
    void Update()
    {
        if (onPlayer)
        {
            DoTheDance();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            movePlayer();
            onPlayer = false;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Wall")
        {
            Debug.Log("Collision");
            targetPosition = GameObject.Find("Player").transform.position;
            //Destroys Object on Collision and spawns a new one
            /*GameObject.Destroy(GameObject.Find("Player"));
            GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Player"), startPosition, rotation);
            gameObject.name = "Player";*/
        }

        if (collision.gameObject.name == "Obstacle")
        {
            Debug.Log("Collision");
            targetPosition = GameObject.Find("Player").transform.position;
            //Destroys Object on Collision and spawns a new one
            /*GameObject.Destroy(GameObject.Find("Player"));
            GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Player"), startPosition, rotation);
            gameObject.name = "Player";*/

        }
    }

    void OnMouseDown()
    {
        onPlayer = true;
    }

    void movePlayer()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
            targetPosition.y = y;
        }
    }

    public void DoTheDance()
    {
        if (count < 50)
        {
            count = count +1;
        }
        else
        {
            onPlayer = false; // will make the update method pick up 
            count = 0;
        }

        }

}


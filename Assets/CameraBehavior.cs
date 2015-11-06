using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    PointAndClick Follower;
    Vector3 startPositon;
    Quaternion temp;
    Vector3 Abstand;
    Vector3 newPosition;

	// Use this for initialization
	void Start () {

        Follower = GameObject.Find("Player").GetComponent<PointAndClick>();
        startPositon = transform.position;
        temp.x = transform.rotation.x;
        temp.y = 0.0f;
        temp.z = 0.0f;
        temp.w = 0.9f;

        Abstand.x = transform.position.x;
        Abstand.y = transform.position.y;
        Abstand.z = transform.position.z;

    }
	
	// Update is called once per frame
	void Update () {

        transform.rotation = temp;

        newPosition.x = Abstand.x + Follower.transform.position.x;
        newPosition.y = Abstand.y;
        newPosition.z = Abstand.z + Follower.transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position,newPosition, Follower.speed * Time.deltaTime);
	
	}
}

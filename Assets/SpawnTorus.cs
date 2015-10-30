using UnityEngine;
using System.Collections;

public class SpawnTorus : MonoBehaviour {

    Vector3 targetPosition;
    PointAndClick playerScript;
    public Vector3 startScale = new Vector3(0.3314262f, 0.3314262f, 0.3314262f);

    void Update()
    {
        GameObject thePlayer = GameObject.Find("Player");
        GameObject torus = GameObject.Find("Torus");
        playerScript = thePlayer.GetComponent<PointAndClick>();
        targetPosition = GameObject.Find("Player").transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, playerScript.speed);

        if (playerScript.onPlayer)
        {
            torus.transform.localScale += new Vector3(0.1F, 0, 0.1F);
        }
        else
        {
            torus.transform.localScale = startScale;
        }

    }

    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

}

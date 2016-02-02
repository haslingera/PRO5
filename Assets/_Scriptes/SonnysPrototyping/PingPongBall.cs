using UnityEngine;
using System.Collections;

public class PingPongBall : MonoBehaviour {

	private StationaryMovement movement;
	private bool allowed1 = false;
	private bool allowed2 = false;
    private SkinnedMeshRenderer scream;

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		movement.constantSpeedX *= GameLogic.Instance.getLevelSpeed ();
        scream = GameObject.Find("jogger").GetComponent<SkinnedMeshRenderer>();
	}

    void Update()
    {
        if (!GameLogic.Instance.getIsLevelActive())
        {
            scream.SetBlendShapeWeight(0, 0.0f);
        }
    }
	
	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag ("TennisPlayer1") || other.gameObject.CompareTag ("TennisPlayer2")) {
			movement.revertMovement ();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("LevelLost")) {
			GameLogic.Instance.didFailLevel ();
            scream.SetBlendShapeWeight(0,0.0f);
		}

		if (other.gameObject.CompareTag ("TennisPlayer1") || other.gameObject.CompareTag ("TennisPlayer2")) {
			movement.revertMovement ();
		}
	}
}
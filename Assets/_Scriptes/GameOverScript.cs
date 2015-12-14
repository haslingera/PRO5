using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onPlayAgain() {
		GameLogic.Instance.startNewSinglePlayerGame ();
	}
}

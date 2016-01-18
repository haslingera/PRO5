using UnityEngine;
using System.Collections;

public class LevelLogic : MonoBehaviour {

	public int numberOfBeats = 8;
	public bool isSurviveLevel = false;

	void Awake() {
		GameLogic.Instance.setLevelNumberOfBeats (this.numberOfBeats);
		GameLogic.Instance.setIsSurviveLevel (this.isSurviveLevel);
		//GameLogic.Instance.startNewDemoGame (this.numberOfBeats);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

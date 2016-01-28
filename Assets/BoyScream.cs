using UnityEngine;
using System.Collections;

public class BoyScream : MonoBehaviour {

	private bool levelDidStart;

	void Start () {
		levelDidStart = false;
	}

	// register for broadcast event "OnLevelReadyToStart"
	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
	}

	// unregister for broadcast event "OnLevelReadyToStart"
	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
	}

	// receives broadcast event "OnLevelReadyToStart" from GameLogic
	private void levelReadyToStart() {
		this.levelDidStart = true;
	}

	void Update() {
		if (levelDidStart) {
			Debug.Log (AudioAnalyzer.Instance.getPitch ());
			if (AudioAnalyzer.Instance.getPitch () > 200) {
				GetComponent<Player_Animation> ().talkDirtyToMe = true;
			} else {
				GetComponent<Player_Animation> ().talkDirtyToMe = false;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class GirlScream : MonoBehaviour {

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
			if (AudioAnalyzer.Instance.getPitch () < 200 && AudioAnalyzer.Instance.getPitch() > 0) {
				GetComponent<Player_Animation> ().talkDirtyToMe = true;
			} else {
				GetComponent<Player_Animation> ().talkDirtyToMe = false;
			}
		}
	}
}

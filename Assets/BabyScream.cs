using UnityEngine;
using System.Collections;

public class BabyScream : MonoBehaviour {

	private bool levelDidStart;

	private bool babyscream;

	void Start () {
		levelDidStart = false;
		babyscream = false;
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
			if (AudioAnalyzer.Instance.getMicLoudness() > 30) {
				GameLogic.Instance.didFailLevel ();
				babyscream = true;
			}

			if (babyscream) {
				SkinnedMeshRenderer babyMeshRen = GetComponent<SkinnedMeshRenderer> ();
				babyMeshRen.SetBlendShapeWeight (0, (Mathf.Sin (Time.time*20)*50)+50);
			}
		}
	}
}
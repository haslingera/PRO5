using UnityEngine;
using System.Collections;

public class BabyScream : MonoBehaviour {

	private bool levelDidStart;
	private bool babyscream;
	private SkinnedMeshRenderer babyMeshRen;

	void Start () {
		babyMeshRen = GetComponent<SkinnedMeshRenderer> ();
		levelDidStart = false;
		babyscream = false;
		GameObject.Find("eyes_default").transform.localScale = new Vector3(1.0f, 0.001f, 1.0f);;
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
				babyMeshRen.SetBlendShapeWeight (0, (Mathf.Sin (Time.time*20)*50)+50);
				GetComponent<Player_Animation> ().blinkSpeed = 0.09f;
			}
		}
	}
}
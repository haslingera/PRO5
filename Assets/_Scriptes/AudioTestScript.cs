using UnityEngine;
using System.Collections;

public class AudioTestScript : MonoBehaviour {


	private float time = 0.0f;

	// Use this for initialization
	void Start () {
		AudioAnalyzer.Instance.Init ();
		time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float pitch = AudioAnalyzer.Instance.getPitch ();
		Debug.Log ("pitch: " + pitch);

		time += Time.deltaTime;

		if (time > 5.0f) GameLogic.Instance.loadNextLevel();

	}
}

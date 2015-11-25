using UnityEngine;
using System.Collections;

public class AudioTestScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
		AudioAnalyzer.Instance.Init ();
	}
	
	// Update is called once per frame
	void Update () {
		float pitch = AudioAnalyzer.Instance.getPitch ();
		Debug.Log ("pitch: " + pitch);
	}
}

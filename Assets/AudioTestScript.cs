using UnityEngine;
using System.Collections;

public class AudioTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Decibel: " + AudioAnalyzer.Instance.getDecibel ());
	}
}

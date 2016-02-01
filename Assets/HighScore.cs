using UnityEngine;
using System.Collections;

public class HighScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<UnityEngine.UI.Text> ().text = PlayerPrefs.GetInt ("highscore").ToString();
	}
}

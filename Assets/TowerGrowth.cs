using UnityEngine;
using System.Collections;

public class TowerGrowth : MonoBehaviour {

    float level;
    InputAnalyser AudioInput;

    // Use this for initialization
    void Start () {

        GameObject Audio = GameObject.Find("Audio Source");
        AudioInput = Audio.GetComponent<InputAnalyser>();

    }
	
	// Update is called once per frame
	void Update () {

        level = AudioInput.LevelMax();
        Debug.Log(level);

        transform.position = new Vector3(0.8461666f, level*10, -17.3f);
	
	}
}

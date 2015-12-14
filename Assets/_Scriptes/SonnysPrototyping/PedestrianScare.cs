using UnityEngine;
using System.Collections;

public class PedestrianScare : MonoBehaviour {
	
	void Start () {
		GameObject.Find ("Pedestrian").GetComponent<StationaryMovement> ().time = 5f/(GameObject.Find ("LevelLogic").GetComponent<LevelLogic> ().numberOfBeats/8f);
	}
}

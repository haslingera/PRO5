using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (startNau ());
		//GameLogic.Instance.startNewDemoGame (30);
		//UIBehaviour.Instance.CameraStartRotation (new Vector3(-90f, -90f, 0f)).CameraStartSize (0.01f).CameraStartSpeed(2.0f).LevelStart ();

	}
	
	IEnumerator startNau() {
		//GameLogic.Instance.startNewDemoGame (30);
		UIBehaviour.Instance.CameraStartRotation (new Vector3(-90f, -90f, 0f)).CameraStartSize (0.01f).CameraStartSpeed(2.0f).LevelStart ();
		yield return new WaitForSeconds (10.5f);
		UIBehaviour.Instance.CameraEndMove (true).CameraEndRotation (new Vector3 (0f, 0f, 180f)).CameraEndSize (0.01f).CameraEndSpeed (2.0f).LevelEnd ();
		yield return new WaitForSeconds (3.5f);
		Application.LoadLevel ("Andi's Szene");
	}

}

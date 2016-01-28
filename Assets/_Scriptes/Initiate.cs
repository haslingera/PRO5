using UnityEngine;
using System.Collections;

public class Initiate : MonoBehaviour {

	bool first = true;
	
	// Use this for initialization
	void Start () {
		if (Application.loadedLevelName.Equals ("Road_Scene")) {
			UIBehaviour.Instance.CameraStartRotation (new Vector3 (50f, -90f, 0f)).TimeBand(true).CameraStartSize (0.01f).LevelStart ();
		} else if (Application.loadedLevelName.Equals ("TreeSawing")) {
			UIBehaviour.Instance.CameraStartRotation (new Vector3 (-90f, 90f, 0f)).TimeBand(true).CameraStartSize (0.01f).LevelStart ();
		} else if (Application.loadedLevelName.Equals ("Plattformen-Szene")) {
			UIBehaviour.Instance.CameraStartRotation (new Vector3 (-90f, 90f, 0f)).TimeBand(true).CameraStartSize (0.01f).LevelStart ();
		} else if (Application.loadedLevelName.Equals ("FlappyScream")) {
			UIBehaviour.Instance.CameraStartRotation (new Vector3 (90f, 0f, 0f)).TimeBand(true).CameraStartSize (0.01f).LevelStart ();
		}else {
			UIBehaviour.Instance.CameraStartRotation (new Vector3 (-90f, 90f, 0f)).TimeBand(true).CameraStartSize (0.01f).LevelStart ();
		}

		if (UIBehaviour.Instance.initateScript) {
			UIBehaviour.Instance.initateScript = false;
			GameLogic.Instance.OnShowLevelInstructions += UIBehaviour.Instance.ShowInstruction;
			GameLogic.Instance.OnHideLevelInstructions += UIBehaviour.Instance.HideInstruction;
			GameLogic.Instance.OnShowLives += UIBehaviour.Instance.ShowLives;
			GameLogic.Instance.OnShowTransitionToNextLevel += TransitionToNextLevel;
		}

	}

	void TransitionToNextLevel() {
		if (Application.loadedLevelName.Equals ("Road_Scene")) {
			UIBehaviour.Instance.CameraEndRotation (new Vector3 (0f, 0f, 0f)).CameraEndSize (0.01f).LevelEnd ();
		} else if (Application.loadedLevelName.Equals ("TreeSawing")) {
			UIBehaviour.Instance.CameraEndRotation (new Vector3 (-90f, 90f, 0f)).CameraEndSize (0.01f).LevelEnd ();
		} else if (Application.loadedLevelName.Equals ("Plattformen-Szene")) {
			UIBehaviour.Instance.CameraEndRotation (new Vector3 (-90f, 90f, 0f)).CameraStartSize (0.01f).LevelEnd ();
		} else if (Application.loadedLevelName.Equals ("FlappyScream")) {
			UIBehaviour.Instance.CameraEndRotation (new Vector3 (90f, 0f, 0f)).CameraStartSize (0.01f).LevelEnd ();
		} else {
			UIBehaviour.Instance.CameraEndRotation (new Vector3 (0f, 0f, 0f)).CameraEndSize (0.01f).LevelEnd ();
		}
	}

}

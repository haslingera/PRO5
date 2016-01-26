using UnityEngine;
using System.Collections;

public class Initiate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (Application.loadedLevelName.Equals("Road_Scene")) {
			UIBehaviour.Instance.CameraStartRotation (new Vector3(0f, 0f, 0f)).CameraStartSize (0.01f).CameraStartSpeed(3.0f).LevelStart ();
		}

		GameLogic.Instance.OnShowLevelInstructions += UIBehaviour.Instance.ShowInstruction;
		GameLogic.Instance.OnHideLevelInstructions += UIBehaviour.Instance.HideInstruction;
		GameLogic.Instance.OnShowLives += UIBehaviour.Instance.ShowLives;
		GameLogic.Instance.OnShowTransitionToNextLevel += TransitionToNextLevel;
	}

	void TransitionToNextLevel() {
		UIBehaviour.Instance.CameraEndMove (true).CameraEndRotation (new Vector3 (0f, 0f, 180f)).CameraEndSize (0.01f).CameraEndSpeed (3.0f).LevelEnd ();
	}

}

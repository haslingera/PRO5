using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {
	void Start () {
		GameLogic.Instance.loadFirstLevelOnHold ();
	}
}

using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIBehaviour.Instance.LevelStart ();
		UIBehaviour.Instance.LevelEnd (true, true, true);
		StartCoroutine (startNau ());
	}
	
	IEnumerator startNau() {
		UIBehaviour.Instance.LevelStart ();
		yield return new WaitForSeconds (2.5f);
		UIBehaviour.Instance.LevelEnd (true, true, true);
		yield return new WaitForSeconds (2.5f);
		StartCoroutine(startNau ());
	}

}

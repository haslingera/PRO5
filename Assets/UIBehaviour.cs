using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Hashtable ht;

	// Use this for initialization
	void Start () {
		StartCoroutine(ChangeText());
	}

	IEnumerator ChangeText() {

		GameObject.Find ("startText").transform.localScale = new Vector3(0, 0, 0);
		GameObject.Find ("startText").GetComponent<Text> ().enabled = true;

		ht = new Hashtable();
		ht.Add("x",1.0);
		ht.Add("y",1.0);
		ht.Add("7",1.0);
		ht.Add("time",0.5);
		ht.Add("easetype",iTween.EaseType.easeOutCubic);

		iTween.ScaleTo (GameObject.Find ("startText"),ht);

		yield return new WaitForSeconds(1.5f);

		Text startText = GameObject.Find ("startText").GetComponent<Text> ();

		startText.text = "go!";

		yield return new WaitForSeconds(1);

		GameObject.Find ("startText").GetComponent<Text> ().enabled = false;

	}

}

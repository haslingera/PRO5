using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Hashtable ht;
	Hashtable ht2;

	// Use this for initialization
	void Start () {

		ht = new Hashtable();
		ht.Add("x",GameObject.Find("Player").transform.position.x);
		ht.Add("y",GameObject.Find("Player").transform.position.y);
		ht.Add("z",GameObject.Find("Player").transform.position.z);
		ht.Add("time",2);

		Camera camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();

		ht2 = new Hashtable();
		ht2.Add("from",camera.orthographicSize);
		ht2.Add("to",0.1);
		ht2.Add ("time", 2);
		ht2.Add ("onupdatetarget", this.gameObject);
		ht2.Add ("onupdate","changeCameraSize");

		iTween.MoveTo(GameObject.Find ("Main Camera"), ht);
		iTween.ValueTo (GameObject.Find ("Main Camera"), ht2);
		iTween.RotateTo(GameObject.Find ("Main Camera"),iTween.Hash("y",90,"easetype",iTween.EaseType.easeInOutSine,"time",2));

		//StartCoroutine(ChangeText());
	}

	void changeCameraSize(float value) {
		GameObject.Find ("Main Camera").GetComponent<Camera>().orthographicSize = value;
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

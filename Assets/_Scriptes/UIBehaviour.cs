using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Vector3 originalCameraPosition;
	Vector3 originalCameraRotation;
	float originalCameraSize;
	bool orange = true;

	// Use this for initialization
	void Start () {
		Camera camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		originalCameraPosition = camera.transform.position;
		originalCameraRotation = camera.transform.eulerAngles;
		originalCameraSize = camera.orthographicSize;
		AdjustCamera (camera);
		CameraEnter (camera);

	}

	void changeCameraSize(float value) {
		ChangeOrthographicCameraSize (GameObject.Find ("Main Camera").GetComponent<Camera>(), value);
		Debug.Log (value);
	}

	void CameraExit(Camera camera, bool move, float moveX, float moveY, float moveZ, bool rotate, float rotateX, float rotateY, float rotateZ, bool zoom, float zoomFactor, float seconds) {

		if (move) {
			iTween.MoveTo(GameObject.Find ("Main Camera"), iTween.Hash("x", moveX, "y", moveY, "z", moveZ, "time", seconds));
		}

		if (rotate) {
			iTween.RotateTo(GameObject.Find ("Main Camera"),iTween.Hash("x", rotateX, "y", rotateY, "z", rotateZ, "easetype",iTween.EaseType.easeInOutSine,"time",seconds));
		}

		if (zoom) {
			iTween.ValueTo (GameObject.Find ("Main Camera"), iTween.Hash("from",originalCameraSize, "to",0.1, "time", seconds, "onupdatetarget", this.gameObject, "onupdate","changeCameraSize"));
		}

		StartCoroutine (StartCameraEnter());

	}

	IEnumerator StartCameraEnter() {
		yield return new WaitForSeconds(2.5f);

		if (orange) {
			GameObject.Find ("Main Camera").GetComponent<Camera> ().backgroundColor = new Color (0f / 255f, 210f / 255f, 201f / 255f, 255f / 255f);
			GameObject.Find ("Player").GetComponent<Renderer> ().material.SetColor ("_Color", new Color (255f / 255f, 152f / 255f, 0f / 255f, 255f / 255f));
			orange = false;
		} else {
			GameObject.Find ("Main Camera").GetComponent<Camera> ().backgroundColor = new Color (255f / 255f, 152f / 255f, 0f / 255f, 255f / 255f);
			GameObject.Find ("Player").GetComponent<Renderer> ().material.SetColor ("_Color", new Color (0f / 255f, 210f / 255f, 201f / 255f, 255f / 255f));
			orange = true;
		}

		CameraEnter (GameObject.Find ("Main Camera").GetComponent<Camera> ());
	}

	void CameraEnter(Camera camera) {

		camera.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
		camera.transform.position = originalCameraPosition;
		ChangeOrthographicCameraSize (camera, originalCameraSize);

		iTween.MoveTo(GameObject.Find ("Main Camera"), iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "time", 2f));

		iTween.RotateTo(GameObject.Find ("Main Camera"),iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time",2f));

		StartCoroutine (StartCameraExit());

	}

	IEnumerator StartCameraExit() {
		yield return new WaitForSeconds(2.5f);
		GameObject player = GameObject.Find ("Player");
		CameraExit (GameObject.Find ("Main Camera").GetComponent<Camera> (), true, player.transform.position.x, player.transform.position.y, player.transform.position.z, true, 90f, 90f, 0f, true, 0.1f, 2f);
	}

	IEnumerator ChangeText() {

		GameObject.Find ("startText").transform.localScale = new Vector3(0, 0, 0);
		GameObject.Find ("startText").GetComponent<Text> ().enabled = true;

		Hashtable ht = new Hashtable();
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

	void AdjustCamera(Camera camera) {

		float screenRatio = (float)Screen.height / (float)Screen.width;

		if (screenRatio >= 1.78f) {
			float newRatio = 1f / 1.78f;
			float newScale = screenRatio / newRatio;
			
			camera.projectionMatrix = Matrix4x4.Ortho(
				-camera.orthographicSize * 1.78f, camera.orthographicSize * 1.78f,
				-camera.orthographicSize * newScale, camera.orthographicSize * newScale,
				camera.nearClipPlane, camera.farClipPlane);
		}


	}

	void ChangeOrthographicCameraSize(Camera camera, float size) {

		float screenRatio = (float)Screen.height / (float)Screen.width;

		if (screenRatio >= 1.78f) {
			float newRatio = 1f / 1.78f;
			float newScale = screenRatio / newRatio;
			
			camera.projectionMatrix = Matrix4x4.Ortho (
				-size * 1.78f, size * 1.78f,
				-size * newScale, size * newScale,
				camera.nearClipPlane, camera.farClipPlane);
		} else {
			camera.orthographicSize = size;
		}

	}

}

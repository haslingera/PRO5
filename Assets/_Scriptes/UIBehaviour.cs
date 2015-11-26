using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Color backgroundColor;
	Color zoomToObjectColor;

	Camera camera;
	GameObject zoomToObject;

	Vector3 originalCameraPosition;
	Vector3 originalCameraRotation;
	float originalCameraSize;

	bool first = true;

	private static UIBehaviour instance = null;

	private UIBehaviour () {}
	
	public static UIBehaviour Instance {
		get {
			if (instance==null) {
				instance = new UIBehaviour();
			}
			return instance;
		}
	}

	public void LevelStart() {

		//Set Camera And Zoom To Object
		camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		if (GameObject.FindGameObjectsWithTag ("ZoomTo").Length != 0) {
			zoomToObject = GameObject.FindGameObjectsWithTag ("ZoomTo") [0];
		} else {
			zoomToObject = GameObject.Find ("Neutral");
		}
			
		//Set Original Camera;
		originalCameraPosition = camera.transform.position;
		originalCameraRotation = camera.transform.eulerAngles;
		originalCameraSize = camera.orthographicSize;

		//Set Camera To See All Things That Were Seen In 16:9
		AdjustCamera ();

		//Get's a new color based on the last player color
		SetBackgroundColorFromZoomToObject ();

		//Switch Colors
		Color tempColor = this.backgroundColor;
		backgroundColor = zoomToObjectColor;
		zoomToObjectColor = tempColor;

		GameObject.Find ("Main Camera").GetComponent<Camera> ().backgroundColor = backgroundColor;
		GameObject.Find ("Neutral").GetComponent<Renderer> ().material.SetColor ("_Color", zoomToObjectColor);

		//Set the camera upwards and to the original position  
		camera.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
		camera.transform.position = originalCameraPosition;
		ChangeOrthographicCameraSize (camera, originalCameraSize);

		//Start the camera entering process
		iTween.MoveTo(camera.gameObject, iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "time", 2f));
		iTween.RotateTo(camera.gameObject,iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time",2f));

	}

	void SetBackgroundColorFromZoomToObject() {
		if (instance.first) {
			first = false;
			zoomToObjectColor = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		}
		backgroundColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
	}

	public void LevelEnd(bool move, bool rotate, bool zoom) {

		if (move) {
			iTween.MoveTo(camera.gameObject, iTween.Hash("x", zoomToObject.transform.position.x, "y", zoomToObject.transform.position.y + zoomToObject.GetComponent<Renderer>().bounds.size.y + 1, "z", zoomToObject.transform.position.z, "time", 2f));
		}
		
		if (rotate) {
			iTween.RotateTo(camera.gameObject,iTween.Hash("x", 90f, "y", 90f, "z", 0f, "easetype",iTween.EaseType.easeInOutSine,"time",2f));
		}

		if (zoom) {
			iTween.ValueTo (camera.gameObject, iTween.Hash("from",originalCameraSize, "to",0.1, "time", 2f, "onupdatetarget", zoomToObject.gameObject, "onupdate","ChangeCameraSize"));
		}

	}

	void ChangeCameraSize(float value) {
		ChangeOrthographicCameraSize (GameObject.Find ("Main Camera").GetComponent<Camera>(), value);
		Debug.Log (value);
	}

	void AdjustCamera() {

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

}

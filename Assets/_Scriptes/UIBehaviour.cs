using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Color backgroundColor;
	Color zoomToObjectColor;

	Camera sceneCamera;
	GameObject zoomToObject;

	Vector3 originalCameraPosition;
	Vector3 originalCameraRotation;
	float originalCameraSize;

	Vector3 cameraStartRotation = new Vector3 (0f, 0f, 0f); //-90,90,0
	Vector3 cameraStartPosition = new Vector3 (0f, 0f, 0f);
	float cameraStartSize = 0.0f;
	float cameraStartSpeed = 0.0f;

	bool startSizeSet = false;
	bool startPosSet = false;
	bool startRotSet = false;
	bool startSpeedSet = false;

	Vector3 cameraEndRotation = new Vector3 (0f, 0f, 0f); //-90,90,0
	float cameraEndSize = 0.01f;
	float cameraEndSpeed = 0.0f;

	bool endSizeSet = false;
	bool endRotSet = false;
	bool endSpeedSet = false;
	bool endMoveSet = false;

	bool first = true;

	float TimeLineWidth = 3f;
	GameObject[] timeLines = new GameObject[4];
	float timeLineAnimationTime = 0.5f;

	bool timeBand = false;
	bool timeBandCreated = false;

	GameObject[] images = new GameObject[4];
	float redValue = 1.0f;

	private static UIBehaviour instance = null;

	private UIBehaviour () {}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	public static UIBehaviour Instance {
		get {
			if (instance == null) { instance = new GameObject ("UIBehaviour").AddComponent<UIBehaviour> (); }
			return instance;
		}
	}

	void Update() {
		if (timeBand) {
			if (timeBandCreated) {
				ChangeTimeBandSize(GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime());
			}
		}
	}

	public void LevelStart() {

		if (GameLogic.Instance.getShowMainMenu()) {
			GameObject.Find ("StartButton").GetComponent<Button>().onClick.AddListener(() => { OnButtonGameClicked();});
			StartScreen();
		} else {
			//Set Camera And Zoom To Object
			sceneCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
			zoomToObject = GameObject.FindGameObjectsWithTag ("ZoomTo") [0];
			
			//Set Camera To See All Things That Were Seen In 16:9
			AdjustCamera ();
			
			//Set Original Camera;
			originalCameraPosition = sceneCamera.transform.position;
			originalCameraRotation = sceneCamera.transform.eulerAngles;
			originalCameraSize = sceneCamera.orthographicSize;
			
			//Get's a new color based on the last player color
			SetBackgroundColorFromZoomToObject ();
			
			//Switch Colors
			Color tempColor = this.backgroundColor;
			backgroundColor = zoomToObjectColor;
			zoomToObjectColor = tempColor;
			
			GameObject.Find ("Main Camera").GetComponent<Camera> ().backgroundColor = backgroundColor;
			zoomToObject.GetComponent<Renderer> ().material.SetColor ("_Color", zoomToObjectColor);
			
			if (startSpeedSet) {
				
				//Set the camera upwards and to the original position  
				if (startRotSet) { sceneCamera.transform.eulerAngles = cameraStartRotation; } else { sceneCamera.transform.eulerAngles = originalCameraRotation; };
				if (startPosSet) { sceneCamera.transform.position = cameraStartPosition; } else { sceneCamera.transform.position = originalCameraPosition; };
				if (startSizeSet) { ChangeOrthographicCameraSize (cameraStartSize); } else { ChangeOrthographicCameraSize (originalCameraSize); };
				
				//Start the camera entering process
				iTween.MoveTo(sceneCamera.gameObject, iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "easetype",iTween.EaseType.easeInOutSine, "time", cameraStartSpeed));
				iTween.RotateTo(sceneCamera.gameObject,iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", cameraStartSpeed));
				iTween.ValueTo (sceneCamera.gameObject, iTween.Hash("from",sceneCamera.orthographicSize, "to",originalCameraSize, "time", cameraStartSpeed, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
				
			}
			
			if (timeBand) {
				StartCoroutine (StartTimeBand (cameraStartSpeed));
			}
			
			endSizeSet = false;
			endRotSet = false;
			endSpeedSet = false;
		}
		
	}

	IEnumerator StartTimeBand(float seconds) {
		yield return new WaitForSeconds (seconds);
		TimeBandStart ();
	}

	public void TimeBandStart() {

		images [0] = GameObject.Find ("CanvasTimeBar").transform.GetChild (0).gameObject as GameObject;
		images [1] = GameObject.Find ("CanvasTimeBar").transform.GetChild (1).gameObject as GameObject;
		images [2] = GameObject.Find ("CanvasTimeBar").transform.GetChild (2).gameObject as GameObject;
		images [3] = GameObject.Find ("CanvasTimeBar").transform.GetChild (3).gameObject as GameObject;

		images [0].GetComponent<Image>().enabled = true;
		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, 0);
		images [1].GetComponent<Image>().enabled = true;
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);
		images [2].GetComponent<Image>().enabled = true;
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, 0);
		images [3].GetComponent<Image>().enabled = true;
		images [3].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);

		iTween.ValueTo (sceneCamera.gameObject, iTween.Hash("from",0f, "to",1f, "time", timeLineAnimationTime, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));
	}

	void ChangeTimeBandSize (float value) {
		Camera cam = Camera.main;
		ChangeOrthographicCameraSize (originalCameraSize + (TimeLineWidth / 2f * value));

		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, 60 * value);
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 60 * value);
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, 60 * value);
		images [3].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, 60 * value);

		if ((int)(60 * value) == 60) {
			timeBandCreated = true;
		}

		if ((int)(60 * value) == 0 && timeBandCreated == true) {
			timeBandCreated = false;
		}

		if (timeBandCreated) {

			if (value > 0.5f) {
				redValue = value;
			}

			images [0].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / redValue, 1.0f * value / redValue);
			images [1].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / redValue, 1.0f * value / redValue);
			images [2].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / redValue, 1.0f * value / redValue);
			images [3].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / redValue, 1.0f * value / redValue);

		}

	}

	public void LevelEnd() {
		StartCoroutine (CoroutineEndLevel());
	}

	IEnumerator CoroutineEndLevel() {
		if (timeBand) {
			TimeBandEnd ();
			yield return new WaitForSeconds (timeLineAnimationTime);
		}
		
		EndAnimation ();
		
	}

	void TimeBandEnd() {
		timeBandCreated = false;
		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime(), "to",0f, "time", timeLineAnimationTime, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));
	}

	public void EndAnimation() {

		if (zoomToObject == null) {
			zoomToObject = GameObject.FindGameObjectsWithTag ("ZoomTo") [0];
		}

		if (endSpeedSet) {
			if (endRotSet) { 
				iTween.RotateTo(Camera.main.gameObject,iTween.Hash("x", cameraEndRotation.x, "y", cameraEndRotation.y, "z", cameraEndRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", cameraEndSpeed));
			}
			
			if (endSizeSet) {
				iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",Camera.main.orthographicSize, "to", cameraEndSize, "time", cameraEndSpeed, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
			}
			
			if (endMoveSet) {
				if ((int)cameraEndRotation.x == 90) {
					iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", zoomToObject.transform.position.x, "y", zoomToObject.transform.position.y + zoomToObject.GetComponent<Renderer>().bounds.size.y + 1, "z", zoomToObject.transform.position.z, "time", cameraEndSpeed));
				} else if ((int)cameraEndRotation.y == 90) {
					iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", zoomToObject.transform.position.x - zoomToObject.GetComponent<Renderer>().bounds.size.x - 1, "y", zoomToObject.transform.position.y + zoomToObject.GetComponent<Renderer>().bounds.size.y / 2f, "z", zoomToObject.transform.position.z, "time", cameraEndSpeed));
				} else if ((int)cameraEndRotation.y == -90) {
					iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", zoomToObject.transform.position.x + zoomToObject.GetComponent<Renderer>().bounds.size.x + 1, "y", zoomToObject.transform.position.y + zoomToObject.GetComponent<Renderer>().bounds.size.y / 2f, "z", zoomToObject.transform.position.z, "time", cameraEndSpeed));
				} else {
					iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", zoomToObject.transform.position.x, "y", zoomToObject.transform.position.y, "z", zoomToObject.transform.position.z - zoomToObject.GetComponent<Renderer>().bounds.size.z - 1, "time", cameraEndSpeed));
				}
			}
			
		}
		
		startSizeSet = false;
		startRotSet = false;
		startPosSet = false;
		startSpeedSet = false;
	}

	public void ShowLives() {
		StartCoroutine (ShowLivesNumerator((float)(3.0 / GameLogic.Instance.getLevelSpeed()), GameLogic.Instance.getNumberOfLives()));
	}

	IEnumerator ShowLivesNumerator(float time, int lives) {
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		
		Camera cameraNew = (Camera) Camera.Instantiate(Camera.main, new Vector3(0, 0, 0), Quaternion.FromToRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
		cameraNew.CopyFrom (Camera.main);
		cameraNew.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
		
		Camera cameraMain = GameObject.Find ("Main Camera").GetComponent<Camera> () as Camera;
		StartCoroutine(ScreenWipe.use.CrossFadePro (cameraNew, cameraMain, time));

		GameObject.Find ("livesOld").GetComponent<Text> ().text = lives + 1 + "";
		GameObject.Find ("livesNew").GetComponent<Text> ().text = lives + "";
		
		GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (0,Screen.height);
		
		GameObject.Find ("livesOld").GetComponent<Text> ().enabled = true;
		GameObject.Find ("livesNew").GetComponent<Text> ().enabled = true;

		float newTime = time + 0.2f;

		yield return new WaitForSeconds (newTime);

		iTween.ValueTo(GameObject.Find ("livesNew").gameObject, iTween.Hash(
			"from", GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition.y,
			"to", 0.0f,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutBack,
			"onupdatetarget", this.gameObject, 
			"onupdate", "MoveGuiElementNewLives"));

		iTween.ValueTo(GameObject.Find ("livesOld").gameObject, iTween.Hash(
			"from", GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition.y,
			"to", -Screen.height,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutBack,
			"onupdatetarget", this.gameObject, 
			"onupdate", "MoveGuiElementOldLives"));

	}

	public void MoveGuiElementNewLives(float yPosition){
		GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (0f,yPosition);
	}

	public void MoveGuiElementOldLives(float yPosition){
		GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition = new Vector2 (0f,yPosition);
	}

	public void StartScreen () {
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;

		GameObject.Find ("StartButton").GetComponent<Image> ().enabled = true;
		GameObject.Find ("StartBackground").GetComponent<Image> ().enabled = true;
		GameObject.Find ("StartForeground").GetComponent<Image> ().enabled = true;

		int xPosition = (int)(GameObject.Find ("StartForeground").GetComponent<RectTransform> ().localPosition.x + Random.Range (-Screen.height / 20, Screen.height / 20));
		int yPosition = (int)(GameObject.Find ("StartForeground").GetComponent<RectTransform> ().localPosition.y + Random.Range (-Screen.height / 20, Screen.height / 20));

		GameObject.Find ("StartForeground").GetComponent<RectTransform> ().localPosition = new Vector2 (xPosition,yPosition);

	}

	public void OnButtonGameClicked() {
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
		
		Camera cameraNew = (Camera) Camera.Instantiate(Camera.main, new Vector3(0, 0, 0), Quaternion.FromToRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
		cameraNew.CopyFrom (Camera.main);
		cameraNew.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		
		Camera cameraMain = GameObject.Find ("Main Camera").GetComponent<Camera> () as Camera;
		StartCoroutine(ScreenWipe.use.CrossFadePro (cameraNew, cameraMain, 1.0f));

		GameObject.Find ("StartButton").GetComponent<Image> ().enabled = false;
		GameObject.Find ("StartBackground").GetComponent<Image> ().enabled = false;
		GameObject.Find ("StartForeground").GetComponent<Image> ().enabled = false;

		GameLogic.Instance.startNewSinglePlayerGame ();
	}

	public void ShowInstruction () {
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = true;
	}

	public void HideInstruction () {
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = false;
	}

	//--------------------------------------
	//------------HELPER-METHODS------------
	//--------------------------------------

	public UIBehaviour CameraEndSpeed(float speed) {
		cameraEndSpeed = speed;
		endSpeedSet = true;
		return this;
	}

	public UIBehaviour CameraEndSize(float size) {
		cameraEndSize = size;
		endSizeSet = true;
		return this;
	}

	public UIBehaviour CameraEndRotation(Vector3 vec) {
		cameraEndRotation = vec;
		endRotSet = true;
		return this;
	}

	public UIBehaviour CameraEndMove(bool move) {
		endMoveSet = move;
		return this;
	}

	public UIBehaviour CameraStartRotation(Vector3 vec) {
		cameraStartRotation = vec;
		startRotSet = true;
		return this;
	}
	
	public UIBehaviour TimeBand(bool time) {
		timeBand = time;
		return this;
	}
	
	public UIBehaviour CameraStartSize(float size) {
		cameraStartSize = size;
		startSizeSet = true;
		return this;
	}
	
	public UIBehaviour CameraStartPosition(Vector3 vec) {
		cameraStartPosition = vec;
		startPosSet = true;
		return this;
	}
	
	public UIBehaviour CameraStartSpeed(float speed) {
		cameraStartSpeed = speed;
		startSpeedSet = true;
		return this;
	}
	
	void ChangeCameraSize(float value) {
		ChangeOrthographicCameraSize (value);
	}

	void SetBackgroundColorFromZoomToObject() {
		if (first) {
			first = false;
			zoomToObjectColor = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		}
		backgroundColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
	}

	void AdjustCamera() {

		float screenRatio = (float)Screen.height / (float)Screen.width;

		if (screenRatio <= 1.78f) {
			float newRatio = 1f / 1.78f;
			float newScale = screenRatio / newRatio;

			Camera.main.projectionMatrix = Matrix4x4.Ortho(
				-Camera.main.orthographicSize * 1.78f, Camera.main.orthographicSize * 1.78f,
				-Camera.main.orthographicSize * newScale, Camera.main.orthographicSize * newScale,
				Camera.main.nearClipPlane, Camera.main.farClipPlane);
		}

	}

	void ChangeOrthographicCameraSize(float size) {

		float screenRatio = (float)Screen.height / (float)Screen.width;

		if (screenRatio <= 1.78f) {
			float newRatio = 1f / 1.78f;
			float newScale = screenRatio / newRatio;
			
			Camera.main.projectionMatrix = Matrix4x4.Ortho (
				-size * 1.78f, size * 1.78f,
				-size * newScale, size * newScale,
				Camera.main.nearClipPlane, Camera.main.farClipPlane);
		} else {
			Camera.main.orthographicSize = size;
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

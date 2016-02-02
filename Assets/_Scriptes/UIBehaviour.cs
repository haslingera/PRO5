using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Color backgroundColor;
	Color zoomToObjectColor;

	GameObject zoomToObject;

	Vector3 originalCameraPosition;
	Vector3 originalCameraRotation;
	float originalCameraSize;

	Vector3 cameraStartRotation = new Vector3 (0f, 0f, 0f);
	Vector3 cameraStartPosition = new Vector3 (0f, 0f, 0f);
	float cameraStartSize = 0.0f;

	Vector3 cameraEndRotation = new Vector3 (0f, 0f, 0f);
	Vector3 cameraEndPosition = new Vector3 (0f, 0f, 0f);
	float cameraEndSize = 0.01f;

	bool startSizeSet = false;
	bool startPosSet = false;
	bool startRotSet = false;

	bool endSizeSet = false;
	bool endPosSet = false;
	bool endRotSet = false;

	bool first = true;
	
	bool timeBand = false;
	bool timeBandCreated = false;

	GameObject[] images = new GameObject[4];
	float redValue = 1.0f;

	float lastTimeValue = 0.0f;

	float cameraStartSpeed = 3.0f;
	float cameraEndSpeed = 2.0f;
	float showLivesSpeed = 3.0f;

	public bool initateScript = true;
	bool imageEffects = true;
	bool mainMenu = false;

	bool timeBandScale = false;
	bool scaleTimeBand = false;

	float timeBandWidth = 0.0f;

	bool timeBandEnded = false;

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
		if (scaleTimeBand) {
			ChangeTimeBandSize(GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime());
		}
	}

	public void LevelStart() {

		//Set Camera And Zoom To Object
		if (GameObject.FindGameObjectsWithTag ("ZoomTo").Length != 0) {
			zoomToObject = GameObject.FindGameObjectsWithTag ("ZoomTo") [0];
		}

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = true;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;
			
		//Set Camera To See All Things That Were Seen In 16:9
		AdjustCamera ();
			
		//Set Original Camera;
		originalCameraPosition = Camera.main.transform.position;
		originalCameraRotation = Camera.main.transform.eulerAngles;
		originalCameraSize = Camera.main.orthographicSize;
			
		//Get's a new color based on the last player color
		SetBackgroundColorFromZoomToObject ();
			
		//Switch Colors
		Color tempColor = this.backgroundColor;
		backgroundColor = zoomToObjectColor;
		zoomToObjectColor = tempColor;
			
		GameObject.Find("BackgroundCamera").GetComponent<Camera> ().backgroundColor = backgroundColor;

		if (zoomToObject != null) {
			zoomToObject.GetComponent<Renderer> ().materials[0].SetColor ("_Color", zoomToObjectColor);
		}

		timeBandWidth = GameObject.Find ("timeBarLeft").transform.gameObject.GetComponent<RectTransform> ().sizeDelta.y;

		timeBandEnded = false;
		
		if (GameLogic.Instance.getShowMainMenu ()) {

			timeBandScale = true;
			TimeBandStart (); 
			StartScreen ();

		} else {
			//Set the camera upwards and to the original position  
			if (startRotSet) { Camera.main.transform.eulerAngles = cameraStartRotation; } else { Camera.main.transform.eulerAngles = originalCameraRotation; };
			if (startPosSet) { Camera.main.transform.position = cameraStartPosition; } else { Camera.main.transform.position = originalCameraPosition; };
			if (startSizeSet) { ChangeOrthographicCameraSize (cameraStartSize); } else { ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height)); };
			
			//Start the camera entering process
			iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "easetype",iTween.EaseType.easeInOutSine, "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.RotateTo(Camera.main.gameObject,iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",Camera.main.orthographicSize, "to", originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height), "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
	
			TimeBandStart();

			endSizeSet = false;
			endRotSet = false;
			
		}
		
	}

	public void TimeBandStart() {

		images [0] = GameObject.Find ("timeBarLeft").transform.gameObject as GameObject;
		images [1] = GameObject.Find ("timeBarTop").transform.gameObject as GameObject;
		images [2] = GameObject.Find ("timeBarRight").transform.gameObject as GameObject;
		images [3] = GameObject.Find ("timeBarBottom").transform.gameObject as GameObject;

		images [0].GetComponent<Image>().enabled = true;
		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, 0);
		images [1].GetComponent<Image>().enabled = true;
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);
		images [2].GetComponent<Image>().enabled = true;
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, 0);
		images [3].GetComponent<Image>().enabled = true;
		images [3].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);

		ChangeTimeBandSize (1.0f);

		scaleTimeBand = true;

	}
	
	void ChangeTimeBandSize (float value) {

		float ratio = Screen.width / Screen.height;

		Color c = GameObject.Find ("BlackOverlay").GetComponent<Image> ().color;
		c.a = value;
		GameObject.Find ("BlackOverlay").GetComponent<Image> ().color = c;
		
		if (imageEffects) {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().blurSize = 3.0f * value;
			GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = true;
			//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().saturation = 1.0f * value;
		} else {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
			GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = false;
			//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;
		}

		ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height * value));

		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, timeBandWidth * value);
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, timeBandWidth * value);
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, timeBandWidth * value);
		images [3].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, timeBandWidth * value);

			if (GameLogic.Instance.getIsSurviveLevel()) {
				images [0].GetComponent<Image> ().color = new Color (1.0f * value, 1.0f, 1.0f * value);
				images [1].GetComponent<Image> ().color = new Color (1.0f * value, 1.0f, 1.0f * value);
				images [2].GetComponent<Image> ().color = new Color (1.0f * value, 1.0f, 1.0f * value);
				images [3].GetComponent<Image> ().color = new Color (1.0f * value, 1.0f, 1.0f * value);
			} else {
				images [0].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value, 1.0f * value);
				images [1].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value, 1.0f * value);
				images [2].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value, 1.0f * value);
				images [3].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value, 1.0f * value);
			}

		lastTimeValue = value;

	}

	public void LevelEnd() {
		StartCoroutine (CoroutineEndLevel());
	}

	IEnumerator CoroutineEndLevel() {

		if (!timeBandEnded) {
			scaleTimeBand = false;
			imageEffects = true;
			TimeBandEnd (cameraEndSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);
			yield return new WaitForSeconds (cameraEndSpeed / GameLogic.Instance.getLevelSpeed () / 6.0f);
		}
		
		EndAnimation (cameraEndSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f);
		
	}

	void TimeBandEnd(float time) {

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from", GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime(), "to",1f, "time", time, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));
	}

	public void EndAnimation(float time) {

		if (!endRotSet) {cameraEndRotation = Camera.main.transform.eulerAngles; }
		if (!endPosSet) {cameraEndPosition = Camera.main.transform.position; }
		if (!startSizeSet) {cameraEndSize = Camera.main.orthographicSize; }

		//Start the camera process
		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", cameraEndPosition.x, "y", cameraEndPosition.y, "z", cameraEndPosition.z, "easetype",iTween.EaseType.easeInOutSine, "time", time));
		iTween.RotateTo(Camera.main.gameObject,iTween.Hash("x", cameraEndRotation.x, "y", cameraEndRotation.y, "z", cameraEndRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", time));
		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",Camera.main.orthographicSize, "to",cameraEndSize, "time", time, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",0 ,"to", 1, "time", time, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "LerpBackgroundColors"));
		
		endSizeSet = false;
		endRotSet = false;
		endPosSet = false;
	}

	public void LerpBackgroundColors(float value){

		float differenceValueR = zoomToObjectColor.r - backgroundColor.r;
		float differenceValueG = zoomToObjectColor.g - backgroundColor.g;
		float differenceValueB = zoomToObjectColor.b - backgroundColor.b;

		Color differenceColor = new Color (backgroundColor.r + differenceValueR * value, backgroundColor.g + differenceValueG * value, backgroundColor.b + differenceValueB * value);
		GameObject.Find("BackgroundCamera").GetComponent<Camera> ().backgroundColor = differenceColor;

	}

	public void ShowLives() {
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		scaleTimeBand = false;
		imageEffects = true;
		timeBandEnded = true;
		TimeBandEnd(showLivesSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);

		StartCoroutine (ShowLivesNumerator(showLivesSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, GameLogic.Instance.getNumberOfLives()));
	}

	IEnumerator ShowLivesNumerator(float time, int lives) {

		GameObject.Find ("livesOld").GetComponent<Text> ().text = lives + 1 + "";
		GameObject.Find ("livesOld").GetComponent<Text> ().color = backgroundColor;
		GameObject.Find ("livesOld").GetComponent<Text> ().enabled = true;

		float tryAgainYPosition = GameObject.Find ("tryAgain").GetComponent<RectTransform> ().localPosition.y;
		float gameOverYPosition = GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition.y;

		if (lives != 0) {
			GameObject.Find ("livesNew").GetComponent<Text> ().text = lives + "";
			GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
			GameObject.Find ("livesNew").GetComponent<Text> ().enabled = true;
			GameObject.Find ("livesNew").GetComponent<Text> ().color = backgroundColor;
		} else {
			GameObject.Find ("gameOver").GetComponent<Image>().enabled = true;
			GameObject.Find ("tryAgain").GetComponent<Text>().enabled = true;
			GameObject.Find ("tryAgain").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("tryAgain").GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
			GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
		}
		
		GameObject.Find ("lives").GetComponent<Image> ().enabled = true;

		float newTime = time / 3.0f + 0.2f;

		yield return new WaitForSeconds (newTime);

		if (lives != 0) {
			iTween.ValueTo (GameObject.Find ("livesNew").gameObject, iTween.Hash (
				"from", GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition.y,
				"to", GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition.y,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInOutBack,
				"onupdatetarget", this.gameObject, 
				"onupdate", "MoveGuiElementNewLives"));
		} else {

			iTween.ValueTo (GameObject.Find ("tryAgain").gameObject, iTween.Hash (
				"from", GameObject.Find ("tryAgain").GetComponent<RectTransform> ().localPosition.y,
				"to", tryAgainYPosition,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInOutBack,
				"onupdatetarget", this.gameObject, 
				"onupdate", "MoveGuiElementTryAgain"));

			iTween.ValueTo (GameObject.Find ("livesNew").gameObject, iTween.Hash (
				"from", GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition.y,
				"to", gameOverYPosition,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInOutBack,
				"onupdatetarget", this.gameObject, 
				"onupdate", "MoveGuiElementGameOver"));

			iTween.ValueTo (GameObject.Find ("lives").gameObject, iTween.Hash (
				"from", GameObject.Find ("lives").GetComponent<RectTransform> ().localPosition.y,
				"to", -Screen.height * 2f,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInOutBack,
				"onupdatetarget", this.gameObject, 
				"onupdate", "MoveGuiElementLives"));

			GameObject.Find ("StartButton").GetComponent<Button>().onClick.AddListener(() => { OnRestartButtonGameClicked();});

		}

		iTween.ValueTo(GameObject.Find ("livesOld").gameObject, iTween.Hash(
			"from", GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition.y,
			"to", -Screen.height * 2f,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutBack,
			"onupdatetarget", this.gameObject, 
			"onupdate", "MoveGuiElementOldLives"));

		yield return new WaitForSeconds (time *  2.0f / 3.0f);

		if (lives != 0) {
			GameObject.Find ("livesOld").GetComponent<Text> ().enabled = false;
			GameObject.Find ("livesNew").GetComponent<Text> ().enabled = false;
			GameObject.Find ("lives").GetComponent<Image> ().enabled = false;
		}

	}

	public void MoveGuiElementLives(float yPosition){
		GameObject.Find ("lives").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("lives").GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void MoveGuiElementNewLives(float yPosition){
		GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void MoveGuiElementGameOver(float yPosition){
		GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void MoveGuiElementTryAgain(float yPosition){
		GameObject.Find ("tryAgain").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("gameOver").GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void MoveGuiElementOldLives(float yPosition){
		GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition = new Vector2 (GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void StartScreen () {

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = true;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		GameObject.Find ("StartText").GetComponent<Text> ().enabled = true;
		GameObject.Find ("Logo").GetComponent<Image> ().enabled = true;
		GameObject.Find ("StartButton").GetComponent<Button>().onClick.AddListener(() => { OnButtonGameClicked();});

	}

	public void OnButtonGameClicked() {

		GameObject.Find ("StartButton").GetComponent<Button> ().onClick.RemoveAllListeners ();

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
		GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = false;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;

		imageEffects = false;

		GameObject.Find ("StartText").GetComponent<Text> ().enabled = false;
		GameObject.Find ("Logo").GetComponent<Image> ().enabled = false;
		GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = false;
		
		GameLogic.Instance.startNewSinglePlayerGame ();
	}

	public void OnRestartButtonGameClicked() {
		GameObject.Find ("StartButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameLogic.Instance.restart ();
	}

	public void ShowInstruction () {
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = true;
	}

	public void HideInstruction () {
		imageEffects = false;
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = false;
	}

	//--------------------------------------
	//------------HELPER-METHODS------------
	//--------------------------------------

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

	public UIBehaviour CameraEndPosition(Vector3 vec) {
		endPosSet = true;
		cameraEndPosition = vec;
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

	public void AdjustCamera() {

		float screenRatio =  (float)Screen.width / (float)Screen.height;
		float desiredRatio = 16.0f / 9.0f;

		if (screenRatio < desiredRatio) {
			float newScale = desiredRatio / screenRatio;
			
			Camera.main.projectionMatrix = Matrix4x4.Ortho(
				-Camera.main.orthographicSize * 1.78f, Camera.main.orthographicSize * 1.78f,
				-Camera.main.orthographicSize * newScale, Camera.main.orthographicSize * newScale,
				Camera.main.nearClipPlane, Camera.main.farClipPlane);
		}

	}

	void ChangeOrthographicCameraSize(float size) {

		float screenRatio = (float)Screen.width / (float)Screen.height;
		float desiredRatio = 16.0f / 9.0f;

		if (screenRatio < desiredRatio) {
			float newScale = desiredRatio / screenRatio;
			
			Camera.main.projectionMatrix = Matrix4x4.Ortho (
				-size * 1.78f, size * 1.78f,
				-size * newScale, size * newScale,
				Camera.main.nearClipPlane, Camera.main.farClipPlane);
		} else {
			Camera.main.orthographicSize = size;
		}

	}

}

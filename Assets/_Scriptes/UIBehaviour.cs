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
				if ((int)GameLogic.Instance.getRemainingLevelTime() > 0) {
					imageEffects = false;
					ChangeTimeBandSize(GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime());
					imageEffects = true;
				}
			}
		}
	}

	public void LevelStart() {

		if (GameLogic.Instance.getShowMainMenu()) {
		//if (true) {
			StartScreen();
		} else {

			//Set Camera And Zoom To Object
			if (GameObject.FindGameObjectsWithTag ("ZoomTo").Length != 0) {
				zoomToObject = GameObject.FindGameObjectsWithTag ("ZoomTo") [0];
			}

			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;
			
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
			
			Camera.main.GetComponent<Camera> ().backgroundColor = backgroundColor;

			if (zoomToObject != null) {
				zoomToObject.GetComponent<Renderer> ().materials[0].SetColor ("_Color", zoomToObjectColor);
			}

			//Set the camera upwards and to the original position  
			if (startRotSet) { Camera.main.transform.eulerAngles = cameraStartRotation; } else { Camera.main.transform.eulerAngles = originalCameraRotation; };
			if (startPosSet) { Camera.main.transform.position = cameraStartPosition; } else { Camera.main.transform.position = originalCameraPosition; };
			if (startSizeSet) { ChangeOrthographicCameraSize (cameraStartSize); } else { ChangeOrthographicCameraSize (originalCameraSize); };
				
				//Start the camera entering process
			iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "easetype",iTween.EaseType.easeInOutSine, "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.RotateTo(Camera.main.gameObject,iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",Camera.main.orthographicSize, "to",originalCameraSize, "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
			
			if (timeBand) {
				StartCoroutine (StartTimeBand (cameraStartSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f, cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			}
			
			endSizeSet = false;
			endRotSet = false;
		}
		
	}

	IEnumerator StartTimeBand(float seconds, float wait) {
		yield return new WaitForSeconds (wait);
		TimeBandStart (seconds);
	}

	public void TimeBandStart(float seconds) {

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

		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",0f, "to",1f, "time", seconds, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));

	}
	
	void ChangeTimeBandSize (float value) {

		float ratio = Screen.width / Screen.height;

		if (imageEffects) {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().blurSize = 3.0f * (1.0f - value);
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().saturation = 1.0f * value;
		} else {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
			//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;
		}

		ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize * 60 / Screen.height * value));

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

		lastTimeValue = value;

	}

	public void LevelEnd() {
		StartCoroutine (CoroutineEndLevel());
	}

	IEnumerator CoroutineEndLevel() {
		if (timeBandCreated) {
			TimeBandEnd (cameraEndSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);
			yield return new WaitForSeconds (cameraEndSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);
		}
		
		EndAnimation (cameraEndSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f);
		
	}

	void TimeBandEnd(float time) {
		if (timeBandCreated) {
			timeBandCreated = false;

			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

			iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from", GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime(), "to",0f, "time", time, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));
			
		}
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
		Camera.main.GetComponent<Camera> ().backgroundColor = differenceColor;

	}

	public void ShowLives() {
		if (timeBandCreated) {

			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

			TimeBandEnd(showLivesSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);
		}
		StartCoroutine (ShowLivesNumerator(showLivesSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, GameLogic.Instance.getNumberOfLives()));
	}

	IEnumerator ShowLivesNumerator(float time, int lives) {

		GameObject.Find ("livesOld").GetComponent<Text> ().text = lives + 1 + "";
		GameObject.Find ("livesNew").GetComponent<Text> ().text = lives + "";
		
		GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (0,Screen.height * 2f);
		
		GameObject.Find ("livesOld").GetComponent<Text> ().enabled = true;
		GameObject.Find ("livesNew").GetComponent<Text> ().enabled = true;

		float newTime = time / 3.0f + 0.2f;

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
			"to", -Screen.height * 2f,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutBack,
			"onupdatetarget", this.gameObject, 
			"onupdate", "MoveGuiElementOldLives"));

		yield return new WaitForSeconds (time *  2.0f / 3.0f);

		GameObject.Find ("livesOld").GetComponent<Text> ().enabled = false;
		GameObject.Find ("livesNew").GetComponent<Text> ().enabled = false;

	}

	public void MoveGuiElementNewLives(float yPosition){
		GameObject.Find ("livesNew").GetComponent<RectTransform> ().localPosition = new Vector2 (0f,yPosition);
	}

	public void MoveGuiElementOldLives(float yPosition){
		GameObject.Find ("livesOld").GetComponent<RectTransform> ().localPosition = new Vector2 (0f,yPosition);
	}

	public void StartScreen () {

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		GameObject.Find ("StartButton").GetComponent<Image> ().enabled = true;
		GameObject.Find ("Logo").GetComponent<Image> ().enabled = true;

		GameObject.Find ("StartButton").GetComponent<Button>().onClick.AddListener(() => { OnButtonGameClicked();});

	}

	public void OnButtonGameClicked() {

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;

		GameObject.Find ("StartButton").GetComponent<Image> ().enabled = false;
		GameObject.Find ("Logo").GetComponent<Image> ().enabled = false;

		GameLogic.Instance.startNewSinglePlayerGame ();
	}

	public void ChangeSaturation(float value) {

	}

	public void ShowInstruction () {
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = true;
	}

	public void HideInstruction () {
		GameObject.Find ("InstructionText").GetComponent<Text> ().enabled = false;
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

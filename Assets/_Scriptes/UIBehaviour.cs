using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {

	Color backgroundColor;
	Color zoomToObjectColor;

	GameObject[] zoomToObjects;

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

	bool timeBandCreated = false;

	Image[] images = new Image[4];

	float cameraStartSpeed = 3.0f;
	float cameraEndSpeed = 2.0f;
	float showLivesSpeed = 3.0f;

	public bool initateScript = true;
	bool imageEffects = true;
	bool mainMenu = false;

	bool scaleTimeBand = false;

	float timeBandWidth = 0.0f;

	bool timeBandEnded = false;

	bool scoreIsShown = false;

	float scoreNewY = 0.0f;
	float scoreImageY = 0.0f;

	GameObject blackOverlay;
	GameObject scoreImage;
	GameObject scoreNew;
	GameObject backgroundCamera;
	GameObject livesOld;
	GameObject lives;
	GameObject livesNew;
	GameObject tryAgain;
	GameObject gameOver;
	GameObject highscore;
	GameObject startButton;
	GameObject score;
	GameObject scoreOld;
	GameObject logo;
	GameObject instructionText;
	GameObject instructionImageScreen;
	GameObject tweenObject;

	float screenRatio;
	float tryAgainYPosition;
	float gameOverYPosition;
	float highscoreYPosition;
	float livesYPosition;
	float scoreOldY;
	float originalInstructionImageScreamY;
	float originalInstructionTextY;
	float firstValue;
	float livesOldYPosition;
	float livesNewYPosition;

	int screenshotCounter = 1;

	RectTransform [] imageRects = new RectTransform[4];

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

		if (Input.GetKeyDown (KeyCode.S)) {
			Debug.Log ("Here");
			Application.CaptureScreenshot(Application.dataPath + "/Screenshot" + screenshotCounter + ".png", 4);
			Debug.Log (Application.dataPath + "Screenshot" + screenshotCounter + ".png");
			screenshotCounter++;
		}


	}

	public void LevelStart() {

		blackOverlay = GameObject.Find ("BlackOverlay");

		images [0] = GameObject.Find ("timeBarLeft").transform.gameObject.GetComponent<Image>();
		imageRects [0] = images [0].GetComponent<RectTransform> ();
		images [1] = GameObject.Find ("timeBarTop").transform.gameObject.GetComponent<Image>();
		imageRects [1] = images [1].GetComponent<RectTransform> ();
		images [2] = GameObject.Find ("timeBarRight").transform.gameObject.GetComponent<Image>();
		imageRects [2] = images [2].GetComponent<RectTransform> ();
		images [3] = GameObject.Find ("timeBarBottom").transform.gameObject.GetComponent<Image>();
		imageRects [3] = images [3].GetComponent<RectTransform> ();

		scoreImage = GameObject.Find ("scoreImage");
		scoreNew = GameObject.Find ("scoreNew");

		livesNew = GameObject.Find ("livesNew");
		livesOld = GameObject.Find ("livesOld");
		lives = GameObject.Find ("lives");
		
		tryAgain = GameObject.Find ("tryAgain");
		gameOver = GameObject.Find ("gameOver");
		highscore = GameObject.Find ("highscore");
		score = GameObject.Find ("score");
		scoreOld = GameObject.Find ("scoreOld");
		instructionText = GameObject.Find ("InstructionText");
		instructionImageScreen = GameObject.Find ("InstructionImageScream");

		startButton = GameObject.Find ("StartButton");

		logo = GameObject.Find ("Logo");

		backgroundCamera = GameObject.Find ("BackgroundCamera");

		firstValue = GameLogic.Instance.getRemainingLevelTime () / GameLogic.Instance.getLevelTime ();

		//Set Camera And Zoom To Object
		if (GameObject.FindGameObjectsWithTag ("ZoomTo").Length != 0) {
			zoomToObjects = GameObject.FindGameObjectsWithTag ("ZoomTo");
		}

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		blackOverlay.GetComponent<Image> ().enabled = true;
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;
			
		//Set Camera To See All Things That Were Seen In 16:9
		AdjustCamera ();
			
		//Set Original Camera;
		originalCameraPosition = Camera.main.transform.position;
		originalCameraRotation = Camera.main.transform.eulerAngles;
		originalCameraSize = Camera.main.orthographicSize;

		scoreOldY = scoreOld.GetComponent<RectTransform> ().localPosition.y;
		scoreNewY = scoreNew.GetComponent<RectTransform> ().localPosition.y;
			
		//Get's a new color based on the last player color
		SetBackgroundColorFromZoomToObject ();

		Color tempColor = this.backgroundColor;
		backgroundColor = zoomToObjectColor;
		zoomToObjectColor = tempColor;
			
		backgroundCamera.GetComponent<Camera> ().backgroundColor = backgroundColor;

		scaleTimeBand = true;

		if (zoomToObjects != null) {
			for (int i = 0; i < zoomToObjects.Length; i++) {
				zoomToObjects[i].GetComponent<Renderer> ().materials[0].SetColor ("_Color", zoomToObjectColor);
			}
		}

		timeBandWidth = images [0].transform.gameObject.GetComponent<RectTransform> ().sizeDelta.y;

		tweenObject = new GameObject ();
		Instantiate (tweenObject);

		timeBandEnded = false;
		
		if (GameLogic.Instance.getShowMainMenu ()) {
			TimeBandStart (); 
			StartScreen ();
		} else {

			if (scoreIsShown) {
				scoreImage.GetComponent<Image> ().enabled = true;
				scoreNew.GetComponent<Text> ().enabled = true;
				scoreNew.GetComponent<Text> ().text = GameLogic.Instance.getNumberOfLevelsCompleted() + "";
			}

			//Set the camera upwards and to the original position  
			if (startRotSet) { Camera.main.transform.eulerAngles = cameraStartRotation; } else { Camera.main.transform.eulerAngles = originalCameraRotation; };
			if (startPosSet) { Camera.main.transform.position = cameraStartPosition; } else { Camera.main.transform.position = originalCameraPosition; };
			if (startSizeSet) { ChangeOrthographicCameraSize (cameraStartSize); } else { ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height)); };
			
			//Start the camera entering process
			LeanTween.move(Camera.main.gameObject, new Vector3(originalCameraPosition.x,originalCameraPosition.y,originalCameraPosition.z), cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f).setEase(LeanTweenType.easeInOutSine);
			LeanTween.rotate (Camera.main.gameObject, new Vector3(originalCameraRotation.x, originalCameraRotation.y, originalCameraRotation.z), cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f).setEase(LeanTweenType.easeInOutSine);
			LeanTween.value (tweenObject, ChangeCameraSize, Camera.main.orthographicSize, originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height), cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f).setEase(LeanTweenType.easeInOutSine);

			imageEffects = true;
			TimeBandStart();

			endSizeSet = false;
			endRotSet = false;
			
		}
	}

	public void TimeBandStart() {

		images [0].enabled = true;
		imageRects [0].sizeDelta = new Vector2(Screen.height, 0);
		images [1].enabled = true;
		imageRects [1].sizeDelta = new Vector2(Screen.width, 0);
		images [2].enabled = true;
		imageRects [2].sizeDelta = new Vector2 (Screen.height, 0);
		images [3].enabled = true;
		imageRects [3].sizeDelta = new Vector2(Screen.width, 0);

		ChangeTimeBandSize (GameLogic.Instance.getRemainingLevelTime () / GameLogic.Instance.getLevelTime ());

	}
	
	void ChangeTimeBandSize (float value) {

		Color overlay = new Color ();
		overlay.a = value;

		blackOverlay.GetComponent<Image> ().color = overlay;
		
		if (imageEffects) {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().blurSize = 3.0f * value;
			if (blackOverlay.GetComponent<Image> ().enabled == false) blackOverlay.GetComponent<Image> ().enabled = true;
			//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().saturation = 1.0f * value;
		} else {
			if (Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled == true) Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
			if (blackOverlay.GetComponent<Image> ().enabled == true) blackOverlay.GetComponent<Image> ().enabled = false;
			if (Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled == true) Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;
		}

		ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height * value));

		imageRects [0] .sizeDelta = new Vector2(Screen.height, timeBandWidth * value / firstValue);
		imageRects [1] .sizeDelta = new Vector2(Screen.width, timeBandWidth * value / firstValue);
		imageRects [2] .sizeDelta = new Vector2 (Screen.height, timeBandWidth * value / firstValue);
		imageRects [3] .sizeDelta = new Vector2 (Screen.width, timeBandWidth * value / firstValue);

		Color current = Color.white;

		if (value < 0.9f) {
			if (GameLogic.Instance.getIsSurviveLevel ()) {
				current =ColorFromHSL (0.33, 0.39, 1.0 - (0.6 *  (1.0 - value / 0.9)));
				//new Color (1.0f * value / 0.9f, 1.0f, 1.0f * value / 0.9f);
			} else {
				current = ColorFromHSL (1.0, 0.6, 1.0 - (0.6 *  (1.0 - value / 0.9)));
			}
		}

		images [0].color = current;
		images [1].color = current;
		images [2].color = current;
		images [3].color = current;

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
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;
			
		LeanTween.value (tweenObject, ChangeTimeBandSize, GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime(), firstValue, time).setEase(LeanTweenType.easeInOutSine);
	}

	public void EndAnimation(float time) {

		if (!endRotSet) {cameraEndRotation = Camera.main.transform.eulerAngles; }
		if (!endPosSet) {cameraEndPosition = Camera.main.transform.position; }
		if (!startSizeSet) {cameraEndSize = Camera.main.orthographicSize; }

		//Start the camera process
		LeanTween.move (Camera.main.gameObject, new Vector3(cameraEndPosition.x, cameraEndPosition.y, cameraEndPosition.z), time).setEase(LeanTweenType.easeInOutSine);
		LeanTween.rotate (Camera.main.gameObject, new Vector3(cameraEndRotation.x, cameraEndRotation.y, cameraEndRotation.z), time).setEase(LeanTweenType.easeInOutSine);
		LeanTween.value (tweenObject, ChangeCameraSize, Camera.main.orthographicSize, cameraEndSize, time).setEase(LeanTweenType.easeInOutSine);
		LeanTween.value (tweenObject, LerpBackgroundColors, 0.0f, 1.0f, time).setEase(LeanTweenType.easeInOutSine);

		endSizeSet = false;
		endRotSet = false;
		endPosSet = false;
	}

	public void LerpBackgroundColors(float value){

		float differenceValueR = zoomToObjectColor.r - backgroundColor.r;
		float differenceValueG = zoomToObjectColor.g - backgroundColor.g;
		float differenceValueB = zoomToObjectColor.b - backgroundColor.b;

		Color differenceColor = new Color (backgroundColor.r + differenceValueR * value, backgroundColor.g + differenceValueG * value, backgroundColor.b + differenceValueB * value);
		backgroundCamera.GetComponent<Camera> ().backgroundColor = differenceColor;

	}

	public void ShowLives() {
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		scaleTimeBand = false;
		imageEffects = true;
		timeBandEnded = true;
		TimeBandEnd(showLivesSpeed / GameLogic.Instance.getLevelSpeed() / 6.0f);

		StartCoroutine (ShowLivesNumerator(showLivesSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, GameLogic.Instance.getNumberOfLives()));
	}

	IEnumerator ShowLivesNumerator(float time, int lives) {

		scoreIsShown = false;

		livesOld.GetComponent<Text> ().text = lives + 1 + "";
		livesOld.GetComponent<Text> ().enabled = true;
		this.lives.GetComponent<Image> ().enabled = true;

		tryAgainYPosition = tryAgain.GetComponent<RectTransform> ().localPosition.y;
		gameOverYPosition = gameOver.GetComponent<RectTransform> ().localPosition.y;
		highscoreYPosition = highscore.GetComponent<RectTransform> ().localPosition.y;
		livesYPosition = this.lives.GetComponent<RectTransform> ().localPosition.y;
		livesOldYPosition = livesOld.GetComponent<RectTransform> ().localPosition.y;
		livesNewYPosition = livesNew.GetComponent<RectTransform> ().localPosition.y;

		if (lives != 0) {
			livesNew.GetComponent<Text> ().text = lives + "";
			livesNew.GetComponent<RectTransform> ().localPosition = new Vector2 (livesNew.GetComponent<RectTransform> ().localPosition.x, livesNewYPosition + Screen.height);
			livesNew.GetComponent<Text> ().enabled = true;
			//GameObject.Find ("livesNew").GetComponent<Text> ().color = backgroundColor;

		} else {
			gameOver.GetComponent<Image>().enabled = true;
			tryAgain.GetComponent<Text>().enabled = true;
			highscore.GetComponent<Text>().enabled = true;
			highscore.GetComponent<Text>().text = "highscore " + PlayerPrefs.GetInt("highscore") + " ... your score " + GameLogic.Instance.getNumberOfLevelsCompleted();
			highscore.GetComponent<RectTransform> ().localPosition = new Vector2 (highscore.GetComponent<RectTransform> ().localPosition.x, highscoreYPosition + Screen.height);
			tryAgain.GetComponent<RectTransform> ().localPosition = new Vector2 (tryAgain.GetComponent<RectTransform> ().localPosition.x, tryAgainYPosition + Screen.height);
			gameOver.GetComponent<RectTransform> ().localPosition = new Vector2 (gameOver.GetComponent<RectTransform> ().localPosition.x, gameOverYPosition + Screen.height);
		}

		float newTime = time / 3.0f + 0.2f;

		yield return new WaitForSeconds (newTime);

		if (lives != 0) {
			LeanTween.move (livesNew.GetComponent<RectTransform> (), new Vector2 (livesNew.GetComponent<RectTransform> ().localPosition.x,livesNewYPosition), 0.4f).setEase(LeanTweenType.easeInOutBack);
		} else {

			LeanTween.move (tryAgain.GetComponent<RectTransform> (), new Vector2 (tryAgain.GetComponent<RectTransform> ().localPosition.x, tryAgainYPosition), 0.4f).setEase(LeanTweenType.easeInOutBack);
			LeanTween.move (gameOver.GetComponent<RectTransform> (), new Vector2 (gameOver.GetComponent<RectTransform> ().localPosition.x, gameOverYPosition), 0.4f).setEase(LeanTweenType.easeInOutBack);
			LeanTween.move (highscore.GetComponent<RectTransform> (), new Vector2 (highscore.GetComponent<RectTransform> ().localPosition.x, highscoreYPosition), 0.4f).setEase(LeanTweenType.easeInOutBack);
			LeanTween.move (this.lives.GetComponent<RectTransform> (), new Vector2 (this.lives.GetComponent<RectTransform> ().localPosition.x, livesYPosition - Screen.height), 0.4f).setEase(LeanTweenType.easeInOutBack);

			startButton.GetComponent<Button>().onClick.AddListener(() => { OnRestartButtonGameClicked();});
			startButton.GetComponent<Image>().enabled = true;

		}

		LeanTween.move (livesOld.GetComponent<RectTransform> (), new Vector2 (livesOld.GetComponent<RectTransform> ().localPosition.x, livesOldYPosition - Screen.height), 0.4f).setEase(LeanTweenType.easeInOutBack);

		yield return new WaitForSeconds (time *  2.0f / 3.0f);

		if (lives != 0) {
			livesOld.GetComponent<Text> ().enabled = false;
			livesNew.GetComponent<Text> ().enabled = false;
			this.lives.GetComponent<Image> ().enabled = false;
		}

	}

	public void StartScoreNumerator() {
		ShowScoreNumerator(0.5f / GameLogic.Instance.getLevelSpeed(), GameLogic.Instance.getNumberOfLevelsCompleted());
	}
	
	void ShowScoreNumerator(float time, int lives) {

		//scoreOldY = scoreOld.GetComponent<RectTransform> ().localPosition.y;
		//scoreNewY = scoreNew.GetComponent<RectTransform> ().localPosition.y;

		scoreImage.GetComponent<RectTransform> ().localPosition = new Vector2 (scoreImage.GetComponent<RectTransform> ().localPosition.x, scoreImageY);
		scoreNew.GetComponent<RectTransform> ().localPosition = new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNewY + Screen.height);

		scoreOld.GetComponent<Text> ().text = lives - 1 + "";
		scoreOld.GetComponent<Text> ().enabled = true;
		scoreImage.GetComponent<Image> ().enabled = true;
		
		scoreNew.GetComponent<Text> ().text = lives + "";
		scoreNew.GetComponent<Text> ().enabled = true;

		LeanTween.move (scoreNew.GetComponent<RectTransform> (), new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNewY), 0.4f).setEase(LeanTweenType.easeInOutBack);
		LeanTween.move (scoreOld.GetComponent<RectTransform> (), new Vector2 (scoreOld.GetComponent<RectTransform> ().localPosition.x, scoreOldY - Screen.height), 0.4f).setEase(LeanTweenType.easeInOutBack);

		scoreIsShown = true;

	}

	public void HideScoreNumerator () {
		scoreOld.GetComponent<Text> ().enabled = false;
		scoreImage.GetComponent<Image> ().enabled = false;
		scoreNew.GetComponent<Text> ().enabled = false;
	}

	public void StartScreen () {

		imageEffects = true;

		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = true;
		blackOverlay.GetComponent<Image> ().enabled = true;
		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = true;

		logo.GetComponent<Image> ().enabled = true;
		startButton.GetComponent<Button>().onClick.AddListener(() => { OnButtonGameClicked();});
		startButton.GetComponent<Image>().enabled = true;

	}

	public void OnButtonGameClicked() {

		startButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		startButton.GetComponent<Image>().enabled = false;

		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
		//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;

		logo.GetComponent<Image> ().enabled = false;
		//GameObject.Find ("BlackOverlay").GetComponent<Image> ().enabled = false;

		GameLogic.Instance.startNewSinglePlayerGame ();
	}

	public void OnRestartButtonGameClicked() {
		startButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		startButton.GetComponent<Image>().enabled = false;

		gameOver.GetComponent<Image>().enabled = false;
		tryAgain.GetComponent<Text>().enabled = false;
		highscore.GetComponent<Text>().enabled = false;

		scoreIsShown = false;

		GameLogic.Instance.restart ();

	}

	public void ShowInstruction () {

		if (Application.loadedLevelName.Equals ("Road_Scene")) {
			instructionText.GetComponent<Text> ().text = "to run";
		} else if (Application.loadedLevelName.Equals ("TreeSawing")) {
			instructionText.GetComponent<Text> ().text = "high and low by turns";
		} else if (Application.loadedLevelName.Equals ("Plattformen-Szene")) {
			instructionText.GetComponent<Text> ().text = "high or low";
		} else if (Application.loadedLevelName.Equals ("FlappyScream")) {
			instructionText.GetComponent<Text> ().text = "to fly";
		} else if (Application.loadedLevelName.Equals ("Tod-Szene-Spiel")) {
			instructionText.GetComponent<Text> ().text = "to scare enemies";
		} else if (Application.loadedLevelName.Equals ("GlassDestroying")) {
			instructionText.GetComponent<Text> ().text = "and find the right pitch";
		} else if (Application.loadedLevelName.Equals ("Tennis")) {
			instructionText.GetComponent<Text> ().text = "to swing";
		} else if (Application.loadedLevelName.Equals ("JumpAndDuck")) {
			instructionText.GetComponent<Text> ().text = "hight to jump, low to duck";
		} else if (Application.loadedLevelName.Equals ("BabyScream")) {
			instructionImageScreen.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("quiet");
			instructionText.GetComponent<Text> ().text = "don’t wake the baby";
		} else if (Application.loadedLevelName.Equals ("PedestrianScare")) {
			instructionText.GetComponent<Text> ().text = "to scare";
		} else {
			instructionText.GetComponent<Text> ().text = "do soitast net sein";
		}

		originalInstructionImageScreamY = instructionImageScreen.GetComponent<RectTransform> ().localPosition.y;
		originalInstructionTextY = instructionText.GetComponent<RectTransform> ().localPosition.y;

		instructionImageScreen.GetComponent<Image> ().enabled = true;
		instructionImageScreen.GetComponent<RectTransform> ().localPosition = new Vector2 (instructionImageScreen.GetComponent<RectTransform> ().localPosition.x, originalInstructionImageScreamY + Screen.height);

		instructionText.GetComponent<Text> ().enabled = true;
		instructionText.GetComponent<RectTransform> ().localPosition = new Vector2 (instructionText.GetComponent<RectTransform> ().localPosition.x, originalInstructionTextY + Screen.height);

		LeanTween.move (instructionText.GetComponent<RectTransform> (), new Vector2 (instructionText.GetComponent<RectTransform> ().localPosition.x, originalInstructionTextY), 0.4f).setEase(LeanTweenType.easeInOutBack);
		LeanTween.move (instructionImageScreen.GetComponent<RectTransform> (), new Vector2 (instructionImageScreen.GetComponent<RectTransform> ().localPosition.x, originalInstructionImageScreamY), 0.4f).setEase(LeanTweenType.easeInOutBack);
	
		if (scoreIsShown) {

			scoreNewY = scoreNew.GetComponent<RectTransform> ().localPosition.y;
			scoreImageY = scoreImage.GetComponent<RectTransform> ().localPosition.y;

			LeanTween.move (scoreNew.GetComponent<RectTransform> (), new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNewY - Screen.height), 0.4f).setEase(LeanTweenType.easeInOutBack);
			LeanTween.move (scoreImage.GetComponent<RectTransform> (), new Vector2 (scoreImage.GetComponent<RectTransform> ().localPosition.x, scoreImageY - Screen.height), 0.4f).setEase(LeanTweenType.easeInOutBack);
		
		}
	}

	public void HideInstruction () {

		imageEffects = false;
		scaleTimeBand = true;
		instructionText.GetComponent<Text> ().enabled = false;
		instructionImageScreen.GetComponent<Image> ().enabled = false;

		scoreOld.GetComponent<Text> ().enabled = false;
		scoreImage.GetComponent<Image> ().enabled = false;
		scoreNew.GetComponent<Text> ().enabled = false;

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
			zoomToObjectColor = CreatePastelColor(); //new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
		}
		backgroundColor = CreatePastelColor(); //new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
	}

	Color CreatePastelColor() {
		float middle = 128f / 255f;
		float add = 127f / 255f;
		return new Color (Random.Range (0.0f, middle) + add, Random.Range (0.0f, middle) + add, Random.Range (0.0f, middle) + add);
	}

	Color CreatePastelColorFromColor(Color color) {
		
		float red = Random.Range (0f, 1f);
		float green = Random.Range (0f, 1f);
		float blue = Random.Range (0f, 1f);

		// mix the color
		if (color != null) {
			red = (red + color.r) / 2;
			green = (green + color.g) / 2;
			blue = (blue + color.b) / 2;
		}

		return new Color(red, green, blue);

	}
	
	public void AdjustCamera() {

		screenRatio = (float)Screen.width / (float)Screen.height;
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

		screenRatio = (float)Screen.width / (float)Screen.height;
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

	Color ColorFromHSL(double h, double s, double l) {
		
		double r = 0, g = 0, b = 0;
		if (l != 0)
		{
			if (s == 0)
				r = g = b = l;
			else
			{
				double temp2;
				if (l < 0.5)
					temp2 = l * (1.0 + s);
				else
					temp2 = l + s - (l * s);

				double temp1 = 2.0 * l - temp2;

				r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
				g = GetColorComponent(temp1, temp2, h);
				b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
			}
		}

		return new Color((float)r,(float)g,(float)b);

	}

	double GetColorComponent(double temp1, double temp2, double temp3)
	{
		if (temp3 < 0.0)
			temp3 += 1.0;
		else if (temp3 > 1.0)
			temp3 -= 1.0;

		if (temp3 < 1.0 / 6.0)
			return temp1 + (temp2 - temp1) * 6.0 * temp3;
		else if (temp3 < 0.5)
			return temp2;
		else if (temp3 < 2.0 / 3.0)
			return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
		else
			return temp1;
	}
		
}
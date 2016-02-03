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

	GameObject[] images = new GameObject[4];

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

	float tryAgainYPosition;
	float gameOverYPosition;
	float highscoreYPosition;
	float livesYPosition;
	float scoreOldY;
	float originalInstructionImageScreamY;
	float originalInstructionTextY;
	float firstValue;

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

		blackOverlay = GameObject.Find ("BlackOverlay");

		images [0] = GameObject.Find ("timeBarLeft").transform.gameObject as GameObject;
		images [1] = GameObject.Find ("timeBarTop").transform.gameObject as GameObject;
		images [2] = GameObject.Find ("timeBarRight").transform.gameObject as GameObject;
		images [3] = GameObject.Find ("timeBarBottom").transform.gameObject as GameObject;

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
			iTween.MoveTo(Camera.main.gameObject, iTween.Hash("x", originalCameraPosition.x, "y", originalCameraPosition.y, "z", originalCameraPosition.z, "easetype",iTween.EaseType.easeInOutSine, "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.RotateTo(Camera.main.gameObject,iTween.Hash("x", originalCameraRotation.x, "y", originalCameraRotation.y, "z", originalCameraRotation.z, "easetype",iTween.EaseType.easeInOutSine,"time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f));
			iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from",Camera.main.orthographicSize, "to", originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height), "time", cameraStartSpeed / GameLogic.Instance.getLevelSpeed() * 5.0f / 6.0f, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeCameraSize"));
	
			imageEffects = true;
			TimeBandStart();

			endSizeSet = false;
			endRotSet = false;
			
		}
	}

	public void TimeBandStart() {

		images [0].GetComponent<Image>().enabled = true;
		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, 0);
		images [1].GetComponent<Image>().enabled = true;
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);
		images [2].GetComponent<Image>().enabled = true;
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, 0);
		images [3].GetComponent<Image>().enabled = true;
		images [3].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 0);

		ChangeTimeBandSize (GameLogic.Instance.getRemainingLevelTime () / GameLogic.Instance.getLevelTime ());

	}
	
	void ChangeTimeBandSize (float value) {

		float ratio = Screen.width / Screen.height;

		Color overlay = new Color ();
		overlay.a = value;

		blackOverlay.GetComponent<Image> ().color = overlay;
		
		if (imageEffects) {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().blurSize = 3.0f * value;
			blackOverlay.GetComponent<Image> ().enabled = true;
			//Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().saturation = 1.0f * value;
		} else {
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized> ().enabled = false;
			blackOverlay.GetComponent<Image> ().enabled = false;
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ().enabled = false;
		}

		ChangeOrthographicCameraSize (originalCameraSize + (originalCameraSize *  timeBandWidth / Screen.height * value));

		images [0].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, timeBandWidth * value / firstValue);
		images [1].GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, timeBandWidth * value / firstValue);
		images [2].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height, timeBandWidth * value / firstValue);
		images [3].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, timeBandWidth * value / firstValue);

		if (value < 0.9f) {
			if (GameLogic.Instance.getIsSurviveLevel ()) {
				images [0].GetComponent<Image> ().color = new Color (1.0f * value / 0.9f, 1.0f, 1.0f * value / 0.9f);
				images [1].GetComponent<Image> ().color = new Color (1.0f * value / 0.9f, 1.0f, 1.0f * value / 0.9f);
				images [2].GetComponent<Image> ().color = new Color (1.0f * value / 0.9f, 1.0f, 1.0f * value / 0.9f);
				images [3].GetComponent<Image> ().color = new Color (1.0f * value / 0.9f, 1.0f, 1.0f * value / 0.9f);
			} else {
				images [0].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / 0.9f, 1.0f * value / 0.9f);
				images [1].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / 0.9f, 1.0f * value / 0.9f);
				images [2].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / 0.9f, 1.0f * value / 0.9f);
				images [3].GetComponent<Image> ().color = new Color (1.0f, 1.0f * value / 0.9f, 1.0f * value / 0.9f);
			}
		} else {
			images [0].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
			images [1].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
			images [2].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
			images [3].GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
		}

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

		iTween.ValueTo (Camera.main.gameObject, iTween.Hash("from", GameLogic.Instance.getRemainingLevelTime() / GameLogic.Instance.getLevelTime(), "to",firstValue, "time", time, "onupdatetarget", this.gameObject, "easetype",iTween.EaseType.easeInOutSine, "onupdate", "ChangeTimeBandSize"));
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

		if (lives != 0) {
			livesNew.GetComponent<Text> ().text = lives + "";
			livesNew.GetComponent<RectTransform> ().localPosition = new Vector2 (livesNew.GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
			livesNew.GetComponent<Text> ().enabled = true;
			//GameObject.Find ("livesNew").GetComponent<Text> ().color = backgroundColor;

		} else {
			gameOver.GetComponent<Image>().enabled = true;
			tryAgain.GetComponent<Text>().enabled = true;
			highscore.GetComponent<Text>().enabled = true;
			highscore.GetComponent<Text>().text = "highscore " + PlayerPrefs.GetInt("highscore") + " ... your score " + GameLogic.Instance.getNumberOfLevelsCompleted();
			highscore.GetComponent<RectTransform> ().localPosition = new Vector2 (highscore.GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
			tryAgain.GetComponent<RectTransform> ().localPosition = new Vector2 (tryAgain.GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
			gameOver.GetComponent<RectTransform> ().localPosition = new Vector2 (gameOver.GetComponent<RectTransform> ().localPosition.x, Screen.height * 2f);
		}

		float newTime = time / 3.0f + 0.2f;

		yield return new WaitForSeconds (newTime);

		if (lives != 0) {
			iTween.ValueTo (GameObject.Find ("livesNew").gameObject, iTween.Hash (
				"from", livesNew.GetComponent<RectTransform> ().localPosition.y,
				"to", livesOld.GetComponent<RectTransform> ().localPosition.y,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInOutBack,
				"onupdatetarget", this.gameObject, 
				"onupdate", "MoveGuiElementNewLives"));

		} else {

			iTween.ValueTo (
				Camera.main.gameObject,
				iTween.Hash("from",0.0f,
			            "to", 1.0f,
			            "time", 0.4f,
			            "onupdatetarget", this.gameObject,
			            "easetype",iTween.EaseType.easeInOutBack,
			            "onupdate", "LivesMovementOne"));

			startButton.GetComponent<Button>().onClick.AddListener(() => { OnRestartButtonGameClicked();});
			startButton.GetComponent<Image>().enabled = true;

		}

		iTween.ValueTo(livesOld.gameObject, iTween.Hash(
			"from", livesOld.GetComponent<RectTransform> ().localPosition.y,
			"to", -Screen.height * 2f,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutBack,
			"onupdatetarget", this.gameObject, 
			"onupdate", "MoveGuiElementOldLives"));

		yield return new WaitForSeconds (time *  2.0f / 3.0f);

		if (lives != 0) {
			livesOld.GetComponent<Text> ().enabled = false;
			livesNew.GetComponent<Text> ().enabled = false;
			this.lives.GetComponent<Image> ().enabled = false;
		}

	}

	void LivesMovementOne(float value) {

		tryAgain.GetComponent<RectTransform> ().localPosition =
			new Vector2 (tryAgain.GetComponent<RectTransform> ().localPosition.x, tryAgainYPosition * value);

		gameOver.GetComponent<RectTransform> ().localPosition =
			new Vector2 (gameOver.GetComponent<RectTransform> ().localPosition.x, gameOverYPosition * value);

		highscore.GetComponent<RectTransform> ().localPosition =
			new Vector2 (highscore.GetComponent<RectTransform> ().localPosition.x, highscoreYPosition * value);

		lives.GetComponent<RectTransform> ().localPosition =
			new Vector2 (lives.GetComponent<RectTransform> ().localPosition.x, livesYPosition - Screen.height * value);

	}

	public void MoveGuiElementNewLives(float yPosition){
		livesNew.GetComponent<RectTransform> ().localPosition = new Vector2 (livesNew.GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void MoveGuiElementOldLives(float yPosition){
		livesOld.GetComponent<RectTransform> ().localPosition = new Vector2 (livesOld.GetComponent<RectTransform> ().localPosition.x,yPosition);
	}

	public void StartScoreNumerator() {
		ShowScoreNumerator(0.5f / GameLogic.Instance.getLevelSpeed(), GameLogic.Instance.getNumberOfLevelsCompleted());
	}
	
	void ShowScoreNumerator(float time, int lives) {

		//scoreOldY = scoreOld.GetComponent<RectTransform> ().localPosition.y;
		//scoreNewY = scoreNew.GetComponent<RectTransform> ().localPosition.y;

		scoreImage.GetComponent<RectTransform> ().localPosition = new Vector2 (scoreImage.GetComponent<RectTransform> ().localPosition.x, scoreImageY);

		scoreOld.GetComponent<Text> ().text = lives - 1 + "";
		scoreOld.GetComponent<Text> ().enabled = true;
		scoreImage.GetComponent<Image> ().enabled = true;
		
		scoreNew.GetComponent<Text> ().text = lives + "";
		scoreNew.GetComponent<RectTransform> ().localPosition = new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNew.GetComponent<RectTransform> ().localPosition.y + Screen.height);
		scoreNew.GetComponent<Text> ().enabled = true;

		iTween.ValueTo (
			Camera.main.gameObject,
			iTween.Hash("from",0.0f,
		            "to", 1.0f,
		            "time", 0.4f,
		            "onupdatetarget", this.gameObject,
		            "easetype",iTween.EaseType.easeInOutBack,
		            "onupdate", "ShowScoreMovementOne"));

		scoreIsShown = true;

	}

	void ShowScoreMovementOne(float value) {

		scoreNew.GetComponent<RectTransform> ().localPosition =
			new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNewY + Screen.height *  (1 - value));

		scoreOld.GetComponent<RectTransform> ().localPosition =
			new Vector2 (scoreOld.GetComponent<RectTransform> ().localPosition.x, scoreOldY - Screen.height * value);

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

		iTween.ValueTo (
			Camera.main.gameObject,
			iTween.Hash("from",1.0f,
		            "to", 0.0f,
		            "time", 0.4f,
		            "onupdatetarget", this.gameObject,
		            "easetype",iTween.EaseType.easeInOutBack,
		            "onupdate", "ShowInstructionMovementOne"));

		if (scoreIsShown) {

			scoreNewY = scoreNew.GetComponent<RectTransform> ().localPosition.y;
			scoreImageY = scoreImage.GetComponent<RectTransform> ().localPosition.y;

			iTween.ValueTo (
				Camera.main.gameObject,
				iTween.Hash("from",0.0f,
			            "to", 1.0f,
			            "time", 0.4f,
			            "onupdatetarget", this.gameObject,
			            "easetype",iTween.EaseType.easeInOutBack,
			            "onupdate", "ShowInstructionMovementTwo"));

		}
	}

	void ShowInstructionMovementOne(float value) {
		
		instructionImageScreen.GetComponent<RectTransform> ().localPosition =
			new Vector2 (instructionImageScreen.GetComponent<RectTransform> ().localPosition.x, originalInstructionImageScreamY + Screen.height * value);

		instructionText.GetComponent<RectTransform> ().localPosition =
			new Vector2 (instructionText.GetComponent<RectTransform> ().localPosition.x, originalInstructionTextY + Screen.height * value);

	}

	void ShowInstructionMovementTwo(float value) {
		
		scoreNew.GetComponent<RectTransform> ().localPosition =
			new Vector2 (scoreNew.GetComponent<RectTransform> ().localPosition.x, scoreNewY - Screen.height * value);
		
		scoreImage.GetComponent<RectTransform> ().localPosition =
			new Vector2 (scoreImage.GetComponent<RectTransform> ().localPosition.x, scoreImageY - Screen.height * value);
			
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

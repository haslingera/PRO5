using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Singleton Methods
	private static GameLogic instance;
	
	// Static singleton property
	public static GameLogic Instance {
		// Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		// otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameObject("GameLogic").AddComponent<GameLogic>()); }
	}



	// --------------------
	//    MonoBehaviour
	// --------------------

	void Awake() {
		DontDestroyOnLoad(this.transform.gameObject);
	}

	void Start() {
		AudioPlayer.Instance.Init ();
	}

	void Update() {
		// if level is started, decrease level time
		if (this.didLoadLevel && !this.isGameOver) {

			if (this.isInTickTockMode) {
				// send out events that level is ready to start
				if (this.didTriggerReadyToStartEvent == false) {
					if (this.OnLevelReadyToStart != null) {
						this.OnLevelReadyToStart ();
						this.didTriggerReadyToStartEvent = true;
					}
				}

				// decrease level time
				this.actualLevelTime -= Time.deltaTime;

				// check if the level is "over", meaning that a survivor level has failed, or a task-level has finished
				if (this.isFailed) {
					if (this.didTriggerRescheduleTickTockEnd == false) {
						this.didTriggerRescheduleTickTockEnd = true;

						// if there's more than 4 beats left, set the time to exactly one "takt" less (subtract 4 beats)
						float timePerBeat = 60.0f / this.currentBPM;
						bool shouldReschedule = false;
						while (timePerBeat * 8 < this.actualLevelTime) {
							// more than 4 beats left, subtract 4 beats from actualLevelTime
							this.actualLevelTime -= timePerBeat * 4;
							shouldReschedule = true;
						}

						if (shouldReschedule) {
							AudioPlayer.Instance.reScheduleTickTockEndWithDelay (this.actualLevelTime - (timePerBeat * 4));
							AudioPlayer.Instance.stopLoopingTickTock ();
						}
					}
				}

				if (this.actualLevelTime < (60.0f / this.currentBPM)) {

					this.isInTickTockMode = false;

					// time's over. check if level succeeded or not
					bool win = this.isSurviveLevel;
					if (this.isFailed) win = false;
					if (this.isSucceeded) win = true;

					if (win) {
						Debug.Log ("did Win");
						// level was successful
						this.numberOfLevelsCompleted++;

						// send out broadcast event
						if (this.OnLevelTimeRanOutSuccess != null)
							this.OnLevelTimeRanOutSuccess ();

						// start transition to next scene
						Invoke("sendOnStartTransitionEvent", (60.0f / this.currentBPM) * 1.0f);

						// TODO: maybe should not be a fixed time later on, but rather a callback from the animation class
						Invoke ("loadNextLevel", (60.0f / this.currentBPM) * 4.5f); 

						// send show instructions after 2 more beats
						Invoke("sendOnShowLevelInstructionsEvent", (60.0f / this.currentBPM) * 4.5f);

						// send hide instructions after 6 more beats
						Invoke("sendOnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 8.5f);

					} else {
						Debug.Log ("did Fail");
						// level was lost
						this.numberOfLives--;

						// check if game over
						if (this.numberOfLives < 0) {
							this.isGameOver = true;

							// show lives
							if (this.OnShowLives != null)
								Invoke ("sendOnShowLivesEvent", (60.0f / this.currentBPM) * 1.0f);
							
							return;
						} 

						// send out broadcast event
						if (this.OnLevelTimeRanOutFail != null)
							this.OnLevelTimeRanOutFail ();

						// after 1 beat show the remaining lives
						Invoke ("sendOnShowLivesEvent", (60.0f / this.currentBPM) * 1.0f);

						// after 1 beat play the lose sound
						Invoke("playLoseSound", (60.0f / this.currentBPM) * 1.0f);

						// after 5 beats hide the lives and start transition to next level
						Invoke ("sendOnStartTransitionEvent", (60.0f / this.currentBPM) * 5.0f);

						// TODO: maybe should not be a fixed time later on, but rather a callback from the animation class
						Invoke ("loadNextLevel", (60.0f / this.currentBPM) * 8.5f);

						// send show instructions
						Invoke("sendOnShowLevelInstructionsEvent", (60.0f / this.currentBPM) * 8.5f);

						// send hide instructions after 4 more beats
						Invoke("sendOnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 12.5f);
					}
			
					// prepare next level
					this.prepareNextLevel();
				}

			}
		}
	}

	void OnLevelWasLoaded(int level) {
		this.didLoadLevel = true;
	}




	// -------------------------
	//    Actual Class Data
	// -------------------------

	public delegate void OnLevelReadyToStartAction ();
	public event OnLevelReadyToStartAction OnLevelReadyToStart;

	public delegate void OnLevelTimeRanOutFailAction ();
	public event OnLevelTimeRanOutFailAction OnLevelTimeRanOutFail;

	public delegate void OnLevelTimeRanOutSuccessAction ();
	public event OnLevelTimeRanOutSuccessAction OnLevelTimeRanOutSuccess;

	public delegate void OnShowTransitionToNextLevelAction();
	public event OnShowTransitionToNextLevelAction OnShowTransitionToNextLevel;

	public delegate void OnShowLevelInstructionsAction();
	public event OnShowLevelInstructionsAction OnShowLevelInstructions;

	public delegate void OnHideLevelInstructionsAction();
	public event OnHideLevelInstructionsAction OnHideLevelInstructions;

	public delegate void OnShowLivesAction();
	public event OnShowLivesAction OnShowLives;


	//private float defaultLevelTime = 5.0f; // Default Time in Seconds
	private float actualLevelTime;
	private float currentLevelMaxTime;

	private bool didTriggerReadyToStartEvent;
	private bool didTriggerRescheduleTickTockEnd;
	private bool isInTickTockMode;

	private const int defaultBPM = 80;
	private const int defaultLevelNumberOfBeats = 16;
	private int currentBPM = defaultBPM;
	private int currentLevelNumberOfBeats = 8;

	private bool didLoadLevel = false;
	private bool isSurviveLevel;
	private bool isGameOver = false;
	private bool isSucceeded;
	private bool isFailed;

	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"TreeSawing", "Tennis", "FlappyScream", "Road_Scene", "Plattformen-Szene", "Tod-Szene-Spiel", "JumpAndDuck", "GlassDestroying"};
	private string actualLevel = "";
	private string nextLevel;

	private bool showMainMenu = false;

	public void startNewSinglePlayerGame() {
		this.showMainMenu = false;
		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = true;
		this.isFailed = false;
		this.isSucceeded = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		// load a random first level
		/*int randomNumber = Random.Range(0, this.levels.Length * 5);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);*/

		// send out broadcast to show level information
		if (this.OnShowLevelInstructions != null) {
			this.OnShowLevelInstructions ();
		}

		// register for broadcast event and waits until the audio player tells that the tick tock sound started, then the countdown will start.
		AudioPlayer.Instance.OnTickTockStarted += tickTockStarted;
	}

	public void restart() {
		this.showMainMenu = false;
		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = true;
		this.isFailed = false;
		this.isSucceeded = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		// load a random first level
		int randomNumber = Random.Range(0, this.levels.Length * 5);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);

		// send out broadcast to show level information
		if (this.OnShowLevelInstructions != null) {
			this.OnShowLevelInstructions ();
		}
	}
		

	public void startGameWithLevel(string level) {
		this.levels = new string[] {level};

		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = false;
		this.isFailed = false;
		this.isSucceeded = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = 80;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		this.actualLevel = level;
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (level);

		// send out broadcast to show level information
		if (this.OnShowLevelInstructions != null) {
			this.OnShowLevelInstructions ();
		}

		// register for broadcast event and waits until the audio player tells that the tick tock sound started, then the countdown will start.
		AudioPlayer.Instance.OnTickTockStarted += tickTockStarted;
	}

	public void startNewDemoGame() {
		this.showMainMenu = false;
		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = true;
		this.isFailed = false;
		this.isSucceeded = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		// load a random first level
		int randomNumber = Random.Range(0, this.levels.Length * 5);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);

		// send out broadcast to show level information
		if (this.OnShowLevelInstructions != null) {
			this.OnShowLevelInstructions ();
		}

		// register for broadcast event and waits until the audio player tells that the tick tock sound started, then the countdown will start.
		AudioPlayer.Instance.OnTickTockStarted += tickTockStarted;
	}

	public void loadFirstLevelOnHold() {
		this.showMainMenu = true;
		this.isGameOver = true;
		this.didLoadLevel = false;

		// load a random first level
		int randomNumber = Random.Range(0, this.levels.Length * 5);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);
	}

	private void loadNextLevel() {
		// load the next level
		Application.LoadLevel (this.nextLevel);
	}

	private void prepareNextLevel() {
		// increase bpm
		int plusBeats = (this.numberOfLevelsCompleted / 2) * 16;
		this.currentBPM = Mathf.Min(defaultBPM + plusBeats, defaultBPM * 2);

		this.didLoadLevel = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isFailed = false;
		this.isSucceeded = false;
		this.isInTickTockMode = false;

		// set tiem for new level
		this.actualLevelTime = 60.0f / this.currentBPM * defaultLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// get the next level to load
		int randomNumber = Random.Range(0, this.levels.Length * 5);
		this.nextLevel = this.levels[randomNumber % this.levels.Length];
	}

	private void tickTockStarted() {
		this.isInTickTockMode = true;
	}

	private void sendOnShowLivesEvent() {
		if (this.OnShowLives != null) this.OnShowLives ();
	}

	private void sendOnStartTransitionEvent() {
		if (this.OnShowTransitionToNextLevel != null) this.OnShowTransitionToNextLevel ();

		AudioPlayer.Instance.startIntroAudio (this.currentBPM);
	}

	private void sendOnShowLevelInstructionsEvent() {
		if (this.OnShowLevelInstructions != null) this.OnShowLevelInstructions ();
	}

	private void sendOnHideLevelInstructionsEvent() {
		if (this.OnHideLevelInstructions != null) this.OnHideLevelInstructions ();
	}

	private void playLoseSound() {
		// play lose sound
		AudioPlayer.Instance.playLoseSound();
	}


	// ------------------------
	//       GameLogic
	// ------------------------

	private void gameOver() {
		this.isGameOver = true;
		Application.LoadLevel ("GameOver");
	}



	// ---------------------------
	//  Getter and Setter Methods
	// ---------------------------

	public void didFinishLevel() {
		if (this.isInTickTockMode) {
			Debug.Log ("isSucceeded()");
			this.isSucceeded = true;
		}
	}

	public void didFailLevel() {
		if (this.isInTickTockMode) {
			Debug.Log ("isFailed()");
			this.isFailed = true;
		}
	}

	public void setIsSurviveLevel(bool isSurviveLevel) {
		this.isSurviveLevel = isSurviveLevel;
	}

	public void setLevelNumberOfBeats(int numberOfBeats) {
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;
	}

	public int getNumberOfLives() {
		return this.numberOfLives;
	}

	public int getNumberOfLevelsCompleted() {
		return this.numberOfLevelsCompleted;
	}

	public float getLevelSpeed() {
		return ((float) this.currentBPM) / ((float) defaultBPM);
	}
		
	public bool getLevelIsReadyToStart() {
		return this.isInTickTockMode;
	}
		
	public float getRemainingLevelTime() {
		return this.actualLevelTime;
	}

	public float getLevelTime() {
		return this.currentLevelMaxTime;
	}

	public int getCurrentLevelNumberOfBeats() {
		return this.currentLevelNumberOfBeats;
	}

	public int getCurrentBPM() {
		return this.currentBPM;
	}

	public bool getShowMainMenu() {
		return this.showMainMenu;
	}
}

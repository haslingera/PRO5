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
		if (this.didStartLevel && !this.isGameOver) {

			// first check if preparation time is done, then cound down the actual level time
			if (this.remainingLevelPreparationTime > 0) {

				// still in preparation mode, count down
				this.remainingLevelPreparationTime -= Time.deltaTime;
			
			} else {
				// preparation done, do actual code
				this.levelIsReadyToStart = true;

				// send out events that level is ready to start
				if (this.didTriggerReadyToStartEvent == false) {
					if (OnLevelReadyToStart != null) {
						OnLevelReadyToStart ();
						this.didTriggerReadyToStartEvent = true;
					}
				}

				// if level time is below 0, then set that this level is lost
				this.actualLevelTime -= Time.deltaTime;
				//Debug.Log ("actual Level Time: " + actualLevelTime);

				if (this.actualLevelTime < 0) {
					if (this.isSucceeded) {
						// level succeeded
						this.didStartLevel = false;

						// send out events
						if (this.OnLevelTimeRanOutSuccess != null) this.OnLevelTimeRanOutSuccess();

						// increase numberOfLevelsCompleted and load next level
						this.numberOfLevelsCompleted++;
						this.loadNextLevel ();

					} else {
						// level failed
						this.numberOfLives--;

						// send out events
						if (this.OnLevelTimeRanOutFail != null) this.OnLevelTimeRanOutFail();

						// if game over, go to game over scene
						if (this.numberOfLives < 0) {
							this.gameOver ();

							// load next level
						} else {
							this.loadNextLevel();
						}
					}
				}

				// check if the level is "over", meaning that a survivor level has failed, or a task-level has finished
				if ((this.isSurviveLevel && !this.isSucceeded) ||
				    (!this.isSurviveLevel && this.isSucceeded)) {

					if (this.didTriggerRescheduleTickTockEnd == false) {
						// if there's more than 4 beats left, set the time to exactly one "takt" less (subtract 4 beats)
						float timePerBeat = 60.0f / this.currentBPM;
						while (timePerBeat * 8 < this.actualLevelTime) {
							// more than 4 beats left, subtract 4 beats from actualLevelTime
							this.actualLevelTime -= timePerBeat * 4;
						}

						AudioPlayer.Instance.reScheduleTickTockEndWithDelay (this.actualLevelTime - (timePerBeat * 4));
						this.didTriggerRescheduleTickTockEnd = true;
					}
				}

				// calculate how many beats are left (for debugging)
				float beatsLeft = this.actualLevelTime / (60.0f / this.currentBPM);
				//Debug.Log ("Beats Left: " + beatsLeft);

				if (beatsLeft < 5.0f) {
					AudioPlayer.Instance.stopLoopingTickTock ();
				}
			}
		}
	}

	void OnLevelWasLoaded(int level) {
		Debug.Log ("level loaded: " + level);

		if (!this.isGameOver) {
			// tell audioplayer to start new ticktock audio
			AudioPlayer.Instance.startTickTockAudio (this.currentBPM, this.currentLevelNumberOfBeats);

			// do some prep
			this.remainingLevelPreparationTime = (60.0f / this.currentBPM) * 8;
			this.didStartLevel = true;
		}
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


	//private float defaultLevelTime = 5.0f; // Default Time in Seconds
	private float actualLevelTime;
	private float currentLevelMaxTime;

	private float remainingLevelPreparationTime;
	private bool levelIsReadyToStart;
	private bool didTriggerReadyToStartEvent;
	private bool didTriggerRescheduleTickTockEnd;

	private const int defaultBPM = 80;
	private const int defaultLevelNumberOfBeats = 8;
	private int currentBPM = defaultBPM;
	private int currentLevelNumberOfBeats = 8;

	private bool didStartLevel = false;
	private bool isGameOver = false;
	private bool isSurviveLevel;
	private bool isSucceeded;
	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"PedestrianScare"};
	private string actualLevel = "";

	public void startNewSinglePlayerGame() {
		this.isGameOver = false;
		this.levelIsReadyToStart = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		this.loadNextLevel ();
	}

	public void startNewDemoGame(int numberOfBeats) {
		this.isGameOver = false;
		this.levelIsReadyToStart = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		this.didStartLevel = true;

		this.isSurviveLevel = true;
		this.isSucceeded = true;

		// start demo sound
		// calculate delay for ticktackEndClip: delay = (numberOfBeats - 3) * timeABeatLasts // why '-3'? --> the "gong" sound is the last sound, that's why it is not -4
		float delay = (this.currentLevelNumberOfBeats - 3) * (60.0f / this.currentBPM);
		AudioPlayer.Instance.startTickTockAudio (this.currentBPM, this.currentLevelNumberOfBeats);
	}

	public void didFinishLevel() {
		this.isSucceeded = true;
		Debug.Log ("Did Finish Level");
	}

	public void didFailLevel() {
		this.isSucceeded = false;
		Debug.Log ("Did Fail Level");
	}

	private void loadNextLevel() {
		// set didStartLevel to false
		this.didStartLevel = false;
		this.levelIsReadyToStart = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;

		this.currentBPM += 20;

		// set time for new level
		this.actualLevelTime = 60.0f / this.currentBPM * defaultLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// load random next level
		int randomNumber;
		//do {
			randomNumber = Random.Range(0, this.levels.Length * 5);
		//} while(this.levels[randomNumber % this.levels.Length] == this.actualLevel);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];
		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);
	}



	// ------------------------
	//       GameLogic
	// ------------------------

	private void gameOver() {
		this.isGameOver = true;
		Debug.Log ("game Over!!!!");
		Application.LoadLevel ("GameOver");
	}



	// ---------------------------
	//  Getter and Setter Methods
	// ---------------------------

	public void setIsSurviveLevel(bool isSurviveLevel) {
		this.isSurviveLevel = isSurviveLevel;
		this.isSucceeded = isSurviveLevel;
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

	public void setLevelNumberOfBeats(int numberOfBeats) {
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;
	}
		
	public bool getLevelIsReadyToStart() {
		return this.levelIsReadyToStart;
	}
		
	public float getRemainingLevelTime() {
		return this.actualLevelTime;
	}

	public float getLevelTime() {
		return this.currentLevelMaxTime;
	}



	// deprecated
	/*public void setLevelTime(float levelTime) {
		this.actualLevelTime = levelTime;
	}*/

}

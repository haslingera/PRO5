using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		this.levelProbabilities = new float[this.levels.Length];

		// set the default probabilty for each level to be the next playable-level
		for (int i = 0; i < this.levelProbabilities.Length; i++) {
			this.levelProbabilities [i] = this.levelProbabilities.Length;
		}

		this.previousLevelsQueue = new Queue<int> ();
		this.previousLevelsQueue.Enqueue (-1);
		this.previousLevelsQueue.Enqueue (-1);
	}

	void Start() {
		AudioPlayer.Instance.Init ();
	}

	void Update() {

		if (Input.GetKey (KeyCode.Escape)) {
			// TODO: go to menu!
		}

		// hide level instructions if timer is ready
		TimeSpan timeSpanHideLevelInstructions = DateTime.Now - this.dateTimeHideLevelInstructions;
		if (this.delayHideLevelInstructions > 0.0f && timeSpanHideLevelInstructions.TotalSeconds > this.delayHideLevelInstructions) {
			this.delayHideLevelInstructions = -10.0f; 
			this.sendOnHideLevelInstructionsEvent ();
		}

		// show level instructions if timer is ready
		TimeSpan timeSpanShowLevelInstructions = DateTime.Now - this.dateTimeShowLevelInstructions;
		if (this.delayShowLevelInstructions > 0.0f && timeSpanShowLevelInstructions.TotalSeconds > this.delayShowLevelInstructions && this.didLoadLevel) {
			this.delayShowLevelInstructions = -10.0f;
			this.sendOnShowLevelInstructionsEvent ();
		}

		// load next level is timer is ready
		TimeSpan timeSpanLoadNextLevel = DateTime.Now - this.dateTimeLoadNextLevel;
		if (this.delayLoadNextLevel > 0.0f && timeSpanLoadNextLevel.TotalSeconds > this.delayLoadNextLevel) {
			this.delayLoadNextLevel = -10.0f;
			this.loadNextLevel ();
		}

		// start transition if timer is ready
		TimeSpan timeSpanStartTransition = DateTime.Now - this.dateTimeStartTransition;
		if (this.delayStartTransition > 0.0f && timeSpanStartTransition.TotalSeconds > this.delayStartTransition) {
			this.delayStartTransition = -10.0f;
			this.sendOnStartTransitionEvent ();
		}

		// show lives if timer is ready
		TimeSpan timeSpanShowLives = DateTime.Now - this.dateTimeShowLives;
		if (this.delayShowLives > 0.0f && timeSpanShowLives.TotalSeconds > this.delayShowLives) {
			this.delayShowLives = -10.0f;
			this.sendOnShowLivesEvent ();
		}

		// play lose sound if timer is ready
		TimeSpan timeSpanPlayLose = DateTime.Now - this.dateTimePlayLose;
		if (this.delayPlayLose > 0.0f && timeSpanPlayLose.TotalSeconds > this.delayPlayLose) {
			this.delayPlayLose = -10.0f;
			this.playLoseSound ();
		}

		// play game over sound if timer is ready
		TimeSpan timeSpanPlayGameOver = DateTime.Now - this.dateTimePlayGameOver;
		if (this.delayPlayGameOver > 0.0f && timeSpanPlayGameOver.TotalSeconds > this.delayPlayGameOver) {
			this.delayPlayGameOver = -10.0f;
			AudioPlayer.Instance.playGameOverSound ();
		}

		// play menu sound if timer is ready
		TimeSpan timeSpanPlayMenuSound = DateTime.Now - this.dateTimePlayMenuSound;
		if (this.delayMenuSound > 0.0f && timeSpanPlayMenuSound.TotalSeconds > this.delayMenuSound) {
			this.delayMenuSound = -10.0f;
			AudioPlayer.Instance.playMenuSound ();
		}

		// show score if timer is ready
		TimeSpan timeSpanShowScore = DateTime.Now - this.dateTimeShowScore;
		if (this.delayShowScore > 0.0f && timeSpanShowScore.TotalSeconds > this.delayShowScore) {
			this.delayShowScore = -10.0f;
			this.sendOnShowScoreEvent ();
		}

		// hide score if timer is ready
		TimeSpan timeSpanHideScore = DateTime.Now - this.dateTimeHideScore;
		if (this.delayHideScore > 0.0f && timeSpanHideScore.TotalSeconds > this.delayHideScore) {
			this.delayHideScore = -10.0f;
			this.sendOnHideScoreEvent ();
		}


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
				if (this.isFailed || this.isSucceeded) {
					if (this.didTriggerRescheduleTickTockEnd == false) {
						this.didTriggerRescheduleTickTockEnd = true;
						this.frozenLevelTime = this.actualLevelTime;
						this.didFreezeLevelTime = true;

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
							//AudioPlayer.Instance.stopLoopingTickTock ();
						}
					}
				}

				if (this.actualLevelTime < (60.0f / this.currentBPM)) {

					this.isInTickTockMode = false;

					if (!this.didFreezeLevelTime) {
						this.frozenLevelTime = this.actualLevelTime;
						this.didFreezeLevelTime = true;
					}

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
						this.dateTimeStartTransition = DateTime.Now;
						this.delayStartTransition = (60.0f / this.currentBPM) * 1.0f;
						//Invoke("sendOnStartTransitionEvent", (60.0f / this.currentBPM) * 1.0f);

						// send on show score event
						this.dateTimeShowScore = DateTime.Now;
						this.delayShowScore = (60.0f / this.currentBPM) * 1.0f;

						// send hide show score event
						this.dateTimeHideScore = DateTime.Now;
						this.delayHideScore = (60.0f / this.currentBPM) * 3.0f;

						// load next level after sime time
						this.dateTimeLoadNextLevel = DateTime.Now;
						this.delayLoadNextLevel = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 3.5f);
						//Invoke ("loadNextLevel", (60.0f / this.currentBPM) * 4.5f); 

						// send show instructions after 2 more beats
						this.dateTimeShowLevelInstructions = DateTime.Now;
						this.delayShowLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 3.5f);
						//Invoke("sendOnShowLevelInstructionsEvent", (60.0f / this.currentBPM) * 4.5f);

						// send hide instructions after 6 more beats
						this.dateTimeHideLevelInstructions = DateTime.Now;
						this.delayHideLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 7.5f);
						//Invoke("sendOnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 8.5f);

					} else {
						if (this.isPresentationGame)
							this.numberOfLevelsCompleted++;
						Debug.Log ("did Fail");
						// level was lost
						this.numberOfLives--;

						// check if game over
						if (this.numberOfLives <= 0) {

							if (this.isGameOver == false) {
								this.isGameOver = true;

								// show lives
								if (this.OnShowLives != null) {
									this.dateTimeShowLives = DateTime.Now;
									this.delayShowLives = (60.0f / this.currentBPM) * 1.0f;
									//Invoke ("sendOnShowLivesEvent", (60.0f / this.currentBPM) * 1.0f);
								}

								// after 1 beat play the lose sound
								//this.dateTimePlayLose = DateTime.Now;
								//this.delayPlayLose = (60.0f / this.currentBPM) * 1.0f;

								// after 1 beat play the game over sound
								this.dateTimePlayGameOver = DateTime.Now;
								this.delayPlayGameOver = (60.0f / this.currentBPM) * 1.0f;

								// play menu sound after long time
								this.dateTimePlayMenuSound = DateTime.Now;
								this.delayMenuSound = (60.0f / this.currentBPM) * 12.0f;
							}	
							
							return;
						} 

						// send out broadcast event
						if (this.OnLevelTimeRanOutFail != null)
							this.OnLevelTimeRanOutFail ();

						// after 1 beat show the remaining lives
						this.dateTimeShowLives = DateTime.Now;
						this.delayShowLives = (60.0f / this.currentBPM) * 1.0f;
						//Invoke ("sendOnShowLivesEvent", (60.0f / this.currentBPM) * 1.0f);

						// after 1 beat play the lose sound
						this.dateTimePlayLose = DateTime.Now;
						this.delayPlayLose = (60.0f / this.currentBPM) * 1.0f;
						//Invoke("playLoseSound", (60.0f / this.currentBPM) * 1.0f);

						// after 5 beats hide the lives and start transition to next level
						this.dateTimeStartTransition = DateTime.Now;
						if (this.isPresentationGame) this.delayStartTransition = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM ()) * 4.0f);
						else this.delayStartTransition = (60.0f / this.currentBPM) * 5.0f;
						//Invoke ("sendOnStartTransitionEvent", (60.0f / this.currentBPM) * 5.0f);

						// load next level after some time
						this.dateTimeLoadNextLevel = DateTime.Now;
						if (this.isPresentationGame) this.delayLoadNextLevel = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM ()) * 7.5f);
						else this.delayLoadNextLevel = (60.0f / this.currentBPM) * 8.5f;
						//Invoke ("loadNextLevel", (60.0f / this.currentBPM) * 8.5f);

						// send show instructions
						this.dateTimeShowLevelInstructions = DateTime.Now;
						if (this.isPresentationGame) this.delayShowLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM ()) * 7.5f);
						else this.delayShowLevelInstructions = (60.0f / this.currentBPM) * 8.5f;
						//Invoke("sendOnShowLevelInstructionsEvent", (60.0f / this.currentBPM) * 8.5f);

						// send hide instructions after 4 more beats
						this.dateTimeHideLevelInstructions = DateTime.Now;
						if (this.isPresentationGame) this.delayHideLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM ()) * 11.5f);
						else this.delayHideLevelInstructions = (60.0f / this.currentBPM) * 12.5f;
						//Invoke("sendOnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 12.5f);
					}
			
					// prepare next level
					this.prepareNextLevel();
				}
			}
		}
	}

	void OnLevelWasLoaded(int level) {
		this.didLoadLevel = true;
		this.didFreezeLevelTime = false;
		this.frozenLevelTime = actualLevelTime;
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

	public delegate void OnShowScoreAction ();
	public event OnShowScoreAction OnShowScore;

	public delegate void OnHideScoreAction ();
	public event OnHideScoreAction OnHideScore;




	// timing data

	private DateTime dateTimeShowLevelInstructions;
	private float delayShowLevelInstructions;

	private DateTime dateTimeHideLevelInstructions;
	private float delayHideLevelInstructions;

	private DateTime dateTimeLoadNextLevel;
	private float delayLoadNextLevel;

	private DateTime dateTimeStartTransition;
	private float delayStartTransition;

	private DateTime dateTimeShowLives;
	private float delayShowLives;

	private DateTime dateTimePlayLose;
	private float delayPlayLose;

	private DateTime dateTimePlayGameOver;
	private float delayPlayGameOver;

	private DateTime dateTimePlayMenuSound;
	private float delayMenuSound; 

	private DateTime dateTimeShowScore;
	private float delayShowScore;

	private DateTime dateTimeHideScore;
	private float delayHideScore;



	//private float defaultLevelTime = 5.0f; // Default Time in Seconds

	private float actualLevelTime;
	private float currentLevelMaxTime;
	private float frozenLevelTime;

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
	private bool didFreezeLevelTime;

	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"TreeSawing", "Tennis", "FlappyScream", "Road_Scene", "Plattformen-Szene", "Tod-Szene-Spiel", "JumpAndDuck", "GlassDestroying", "BabyScream", "PedestrianScare"};

	private float[] levelProbabilities;
	private Queue<int> previousLevelsQueue;
	private string nextLevel;

	private bool isPresentationGame = false;
	private int levelsDone = 0;

	private bool showMainMenu = false;

	public void startNewSinglePlayerGame() {
		this.setupTimers ();
		this.showMainMenu = false;
		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = true;
		this.isFailed = false;
		this.isSucceeded = false;
		this.didFreezeLevelTime = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.frozenLevelTime = actualLevelTime;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);


		// send out broadcast to show level information
		if (this.OnShowLevelInstructions != null) {
			this.OnShowLevelInstructions ();
		}

		this.dateTimeHideLevelInstructions = DateTime.Now;
		this.delayHideLevelInstructions = ((60.0f / this.currentBPM) * 4.0f);
		//Invoke("OnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 4.0f);

		// register for broadcast event and waits until the audio player tells that the tick tock sound started, then the countdown will start.
		AudioPlayer.Instance.OnTickTockStarted += tickTockStarted;

		Application.targetFrameRate = 60;
	}

	public void restart() {
		this.setupTimers ();
		this.showMainMenu = false;
		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = true;
		this.isFailed = false;
		this.isSucceeded = false;
		this.didFreezeLevelTime = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.frozenLevelTime = actualLevelTime;
		this.currentLevelMaxTime = this.actualLevelTime;
		this.setIsSurviveLevel (true); // default will be survive level

		// reset the logic for not loading levels doubled
		this.previousLevelsQueue.Clear ();
		this.previousLevelsQueue.Enqueue (-1);
		this.previousLevelsQueue.Enqueue(-1);

		// set the default probabilty for each level to be the next playable-level
		for (int i = 0; i < this.levelProbabilities.Length; i++) {
			this.levelProbabilities [i] = this.levelProbabilities.Length;
		}

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		// start transition to next scene
		this.dateTimeStartTransition = DateTime.Now;
		this.delayStartTransition = (60.0f / this.currentBPM) * 0.1f;
		//Invoke("sendOnStartTransitionEvent", (60.0f / this.currentBPM) * 1.0f);

		// load next level after sime time
		this.dateTimeLoadNextLevel = DateTime.Now;
		this.delayLoadNextLevel = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 2.5f);
		//Invoke ("loadNextLevel", (60.0f / this.currentBPM) * 4.5f); 

		// send show instructions after 2 more beats
		this.dateTimeShowLevelInstructions = DateTime.Now;
		this.delayShowLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 2.5f);
		//Invoke("sendOnShowLevelInstructionsEvent", (60.0f / this.currentBPM) * 4.5f);

		// send hide instructions after 6 more beats
		this.dateTimeHideLevelInstructions = DateTime.Now;
		this.delayHideLevelInstructions = (60.0f / this.currentBPM) * 1.0f + ((60.0f / this.getNextLevelBPM()) * 6.5f);
		//Invoke("sendOnHideLevelInstructionsEvent", (60.0f / this.currentBPM) * 8.5f);
	}

	public void startGameWithLevel(string level) {
		this.levels = new string[] {level};
		this.setupTimers ();

		this.isGameOver = false;
		this.didTriggerReadyToStartEvent = false;
		this.didTriggerRescheduleTickTockEnd = false;
		this.isInTickTockMode = false;
		this.didLoadLevel = false;
		this.isFailed = false;
		this.isSucceeded = false;
		this.didFreezeLevelTime = false;

		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = 80;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.frozenLevelTime = actualLevelTime;
		this.currentLevelMaxTime = this.actualLevelTime;

		// tell audioplayer to start new ticktock audio
		AudioPlayer.Instance.startIntroAudio (this.currentBPM);

		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (level);

		// send out broadcast to show level information
		this.dateTimeShowLevelInstructions = DateTime.Now;
		this.delayShowLevelInstructions = 1.0f;

		this.dateTimeHideLevelInstructions = DateTime.Now;
		this.delayHideLevelInstructions = 60.0f / this.currentBPM * 4;

		// register for broadcast event and waits until the audio player tells that the tick tock sound started, then the countdown will start.
		AudioPlayer.Instance.OnTickTockStarted += tickTockStarted;

		Application.targetFrameRate = 60;
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
		int randomNumber = UnityEngine.Random.Range(0, this.levels.Length * 5);

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
		AudioPlayer.Instance.playMenuSound ();
		this.showMainMenu = true;
		this.isGameOver = true;
		this.didLoadLevel = false;

		// load a random first level
		int randomNumber = UnityEngine.Random.Range(0, this.levels.Length * 5);
		this.previousLevelsQueue.Enqueue (randomNumber % this.levels.Length);
		this.previousLevelsQueue.Dequeue ();

		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel (this.levels[randomNumber % this.levels.Length]);
	}

	public void loadPresentationLevelOnHold() {
		this.isPresentationGame = true;
		AudioPlayer.Instance.playMenuSound ();
		this.showMainMenu = true;
		this.isGameOver = true;
		this.didLoadLevel = false;

		// load flappy scream at the beginning

		this.setIsSurviveLevel (true); // default will be survive level
		Application.LoadLevel ("FlappyScream");
	}

	private void loadNextLevel() {
		// load the next level
		Application.LoadLevel (this.nextLevel);
	}

	private void prepareNextLevel() {

		if (this.isPresentationGame) {
			// increase bpm every level
			int plusBeats = this.numberOfLevelsCompleted * 16;
			this.currentBPM = Mathf.Min (defaultBPM + plusBeats, defaultBPM * 2);

		} else {
			// increase bpm
			int plusBeats = (this.numberOfLevelsCompleted / 2) * 16;
			this.currentBPM = Mathf.Min (defaultBPM + plusBeats, defaultBPM * 2);
		}

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
		if (this.isPresentationGame) {
			switch (this.levelsDone) {
			case 0:
				this.nextLevel = "TreeSawing";
				this.levelsDone = 1;
				break;

			case 1:
				this.nextLevel = "Road_Scene";
				this.levelsDone = 2;
				break;

			case 2:
				this.nextLevel = "GlassDestroying";
				this.levelsDone = 3;
				break;

			case 3:
				this.nextLevel = "FlappyScream";
				this.levelsDone = 0;
				break;

			default:
				break;
			}

		} else { 
			// get the total number of elements
			int probabilitySum = 0;
			foreach (float probability in this.levelProbabilities) {
				probabilitySum += (int) probability;
			}

			// create an array of length probability sum, and fill each level as often as its probability is in the array
			int[] probabilities = new int[probabilitySum];
			int probabilitiesSoFar = 0;
			int loopCounter = 0;
			foreach (float probability in this.levelProbabilities) {
				for (int i = 0; i < (int)probability; i++) {
					probabilities [probabilitiesSoFar + i] = loopCounter;
				}
				probabilitiesSoFar += (int)probability;
				loopCounter++;
			}

			Debug.Log ("probability sum: " + probabilitySum);

			// get the last played levels
			int[] previousPlayedLevels = this.previousLevelsQueue.ToArray();
			int randomNumber;
			int randomLevel;
			bool repeat = false;
			do {
				repeat = false;
				randomNumber = UnityEngine.Random.Range (0, probabilitySum);
				randomLevel = probabilities [randomNumber];

				// check if the new level is contained in the previous loaded levels
				foreach (int previousPlayedLevel in previousPlayedLevels) {
					if (randomLevel == previousPlayedLevel) {
						repeat = true;
						Debug.Log("repeating");
					}
				}
			} while (repeat);

			this.nextLevel = this.levels[randomLevel % this.levels.Length];
			this.previousLevelsQueue.Enqueue(randomLevel);
			this.previousLevelsQueue.Dequeue ();

			// increase probability for loaded level
			for (int i = 0; i < this.levels.Length; i++) {
				if (i == randomLevel) {
					// this level is the loaded one, decrease by one
					this.levelProbabilities [i] = Math.Max (this.levelProbabilities [i] - 3.0f, 0);
				
				} else {
					this.levelProbabilities [i] += (1.0f / this.levels.Length) * 3.0f;
				}
			}


			/*int randomNumber = UnityEngine.Random.Range (0, this.levels.Length * 5);
			this.nextLevel = this.levels [randomNumber % this.levels.Length];*/
		}
	}

	private int getNextLevelBPM() {
		if (this.isPresentationGame) {
			int plusBeats = (this.levelsDone + 1) * 16;
			int nextLevelBPM = Mathf.Min (defaultBPM + plusBeats, defaultBPM * 2);
			return nextLevelBPM;
		} else {

			int plusBeats = ((this.numberOfLevelsCompleted + 1) / 2) * 16;
			int nextLevelBPM = Mathf.Min (defaultBPM + plusBeats, defaultBPM * 2);
			return nextLevelBPM;
		}
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
		Debug.Log ("show Level Instructions");
		if (this.OnShowLevelInstructions != null) this.OnShowLevelInstructions ();
	}

	private void sendOnHideLevelInstructionsEvent() {
		if (this.OnHideLevelInstructions != null) this.OnHideLevelInstructions ();
	}

	private void sendOnShowScoreEvent() {
		if (this.OnShowScore != null) this.OnShowScore ();
	}

	private void sendOnHideScoreEvent() {
		if (this.OnHideScore != null) this.OnHideScore ();
	}

	private void playLoseSound() {
		// play lose sound
		AudioPlayer.Instance.playLoseSound();
	}

	private void setupTimers() {
		this.dateTimeHideLevelInstructions = DateTime.Now;
		this.delayHideLevelInstructions = -10.0f;

		this.dateTimeShowLevelInstructions = DateTime.Now;
		this.delayShowLevelInstructions = -10.0f;

		this.dateTimeLoadNextLevel = DateTime.Now;
		this.delayLoadNextLevel = -10.0f;

		this.dateTimeStartTransition = DateTime.Now;
		this.delayStartTransition = -10.0f;

		this.dateTimeShowLives = DateTime.Now;
		this.delayShowLives = -10.0f;

		this.dateTimePlayLose = DateTime.Now;
		this.delayPlayLose = -10.0f;

		this.dateTimePlayGameOver = DateTime.Now;
		this.delayPlayGameOver = -10.0f;

		this.dateTimePlayMenuSound = DateTime.Now;
		this.delayMenuSound = -10.0f;

		this.dateTimeShowScore = DateTime.Now;
		this.delayShowScore = -10.0f;

		this.dateTimeHideScore = DateTime.Now;
		this.delayHideScore = -10.0f;
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

			if (this.isSucceeded == false) {
				this.isSucceeded = true;
				AudioPlayer.Instance.playSucceedSound ();
			}
		}
	}

	public void didFailLevel() {
		if (this.isInTickTockMode) {
			Debug.Log ("isFailed()");

			if (this.isFailed == false) {
				this.isFailed = true;
				AudioPlayer.Instance.playFailSound ();
			}
		}
	}

	public void setIsSurviveLevel(bool isSurviveLevel) {
		this.isSurviveLevel = isSurviveLevel;
	}

	public void setLevelNumberOfBeats(int numberOfBeats) {
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.frozenLevelTime = actualLevelTime;
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
		// get time for one beat
		float oneBeat = (60.0f / this.currentBPM);

		if (this.didFreezeLevelTime) {
			return Mathf.Max(this.frozenLevelTime - oneBeat, 0);

		} else {
			return Mathf.Max(this.actualLevelTime - oneBeat, 0);

		}
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

	public bool getIsLevelActive() {
		// returns true if level is currently active, meaning that the player still has to play and has neither succeeded nor failed yet
		if (this.isInTickTockMode) {
			if (this.isSucceeded || this.isFailed) {
				return false;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}

	public bool getIsSurviveLevel() {
		return this.isSurviveLevel;
	}
}

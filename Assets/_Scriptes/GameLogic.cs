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
			// if level time is below 0, then set that this level is lost
			this.actualLevelTime -= Time.deltaTime;
			//Debug.Log ("actual Level Time: " + actualLevelTime);
			if (this.actualLevelTime < 0) {
				this.didFailLevel ();
			}

			// calculate how many beats are left (for debugging)
			float beatsLeft = this.actualLevelTime / (60.0f / this.currentBPM);
			Debug.Log ("Beats Left: " + beatsLeft);

			if (beatsLeft < 5.0f) {
				AudioPlayer.Instance.stopLoopingTickTock();
			}

			// update metronom timer for metronomsound
			/*this.metronomTimer += Time.deltaTime;
			if (this.metronomTimer > (60.0f / ((float) this.currentBPM))) {
				this.metronomTimer -= (60.0f / ((float) this.currentBPM));

				// play metronom sound
				//AudioPlayer.Instance.playSoundEffect (this.metronomClip);
			}*/
		}
	}

	void OnLevelWasLoaded(int level) {
		Debug.Log ("level loaded: " + level);
		this.didStartLevel = true;

		float delay = (this.currentLevelNumberOfBeats - 3) * (60.0f / this.currentBPM);
		AudioPlayer.Instance.startTickTockAudio (delay);
	}




	// -------------------------
	//    Actual Class Data
	// -------------------------

	private bool isInDemoMode = false;

	//private float defaultLevelTime = 5.0f; // Default Time in Seconds
	private float actualLevelTime;
	private float currentLevelMaxTime;

	private const int defaultBPM = 80;
	private const int defaultLevelNumberOfBeats = 8;
	private int currentBPM = defaultBPM;
	private int currentLevelNumberOfBeats = 8;

	private float metronomTimer = 0.0f;

	private bool didStartLevel = false;
	private bool isGameOver = false;
	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"Tod-Szene-Spiel", "Road_Scene", "", "TreeSawing", "JumpAndDuck"};
	private string actualLevel = "";

	public void startNewSinglePlayerGame() {
		this.isInDemoMode = false;
		this.isGameOver = false;
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats-1;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		this.loadNextLevel ();
	}

	public void startNewDemoGame(int numberOfBeats) {
		this.isInDemoMode = true;
		this.isGameOver = false;
		this.numberOfLives = 0;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = numberOfBeats-1;
		this.actualLevelTime = 60.0f / currentBPM * this.currentLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		this.didStartLevel = true;

		// start demo sound
		// calculate delay for ticktackEndClip: delay = (numberOfBeats - 3) * timeABeatLasts // why '-3'? --> the "gong" sound is the last sound, that's why it is not -4
		float delay = (this.currentLevelNumberOfBeats - 3) * (60.0f / this.currentBPM);
		AudioPlayer.Instance.startTickTockAudio (delay);
	}

	public void didFinishLevel() {
		// set didStartLevel to false
		this.didStartLevel = false;

		// increase numberOfLevelsCompleted and load next level
		this.numberOfLevelsCompleted++;
		this.loadNextLevel ();
	}

	public void didFailLevel() {
		this.numberOfLives--;

		// if game over, go to game over scene
		if (this.numberOfLives < 0) {
			this.gameOver ();

		// load next level
		} else {
			this.loadNextLevel();
		}
	}

	private void loadNextLevel() {
		// set didStartLevel to false
		this.didStartLevel = false;

		// set time for new level
		this.actualLevelTime = 60.0f / this.currentBPM * defaultLevelNumberOfBeats;
		this.currentLevelMaxTime = this.actualLevelTime;

		// load random next level
		int randomNumber;
		do {
			randomNumber = Random.Range(0, this.levels.Length * 5);
		} while(this.levels[randomNumber % this.levels.Length] == this.actualLevel);

		this.actualLevel = this.levels[randomNumber % this.levels.Length];

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
	}

	// deprecated
	public void setLevelTime(float levelTime) {
		this.actualLevelTime = levelTime;
	}
		
	public float getRemainingLevelTime() {
		return this.actualLevelTime;
	}

	public float getLevelTime() {
		return this.currentLevelMaxTime;
	}
}

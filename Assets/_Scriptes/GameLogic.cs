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

	}

	void Update() {
		// if level is started, decrease level time
		if (this.didStartLevel) {
			this.actualLevelTime -= Time.deltaTime;

			// if level time is below 0, then set that this level is lost
			if (this.actualLevelTime < 0) {
				this.didFailLevel ();
			}
		}
	}

	void OnLevelWasLoaded(int level) {
		Debug.Log ("level loaded: " + level);
		this.didStartLevel = true;
	}




	// -------------------------
	//    Actual Class Data
	// -------------------------

	private bool isInDemoMode = false;

	//private float defaultLevelTime = 5.0f; // Default Time in Seconds
	private float actualLevelTime;

	private const int defaultBPM = 60;
	private const int defaultLevelNumberOfBeats = 8;
	private int currentBPM = defaultBPM;
	private int currentLevelNumberOfBeats = 8;

	private bool didStartLevel = false;
	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"Destroy Schrei", "Flappy Schrei", "Fliegenesser"};
	private string actualLevel = "";

	public void startNewSinglePlayerGame() {
		this.isInDemoMode = false;
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = defaultLevelNumberOfBeats;
		this.actualLevelTime = 60.0f / defaultBPM * defaultLevelNumberOfBeats;

		this.loadNextLevel ();
	}

	public void startNewDemoGame(int numberOfBeats) {
		this.isInDemoMode = true;

		this.numberOfLives = 0;
		this.numberOfLevelsCompleted = 0;
		this.currentBPM = defaultBPM;
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / defaultBPM * this.currentLevelNumberOfBeats;

		this.didStartLevel = true;
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

	public void loadNextLevel() {
		// set didStartLevel to false
		this.didStartLevel = false;

		// set time for new level
		this.actualLevelTime = 60.0f / this.currentBPM * defaultLevelNumberOfBeats;

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

	public void setLevelNumberOfBeats(int numberOfBeats) {
		this.currentLevelNumberOfBeats = numberOfBeats;
		this.actualLevelTime = 60.0f / this.currentBPM * this.currentLevelNumberOfBeats;
	}

	// deprecated
	public void setLevelTime(float levelTime) {
		this.actualLevelTime = levelTime;
	}
}

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
		DontDestroyOnLoad (this.transform.gameObject);
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

	private float defaultLevelTime = 5.0f; // Default Time in Seconds
	private float actualLevelTime;
	private bool didStartLevel = false;
	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"Destroy Schrei", "Flappy Schrei", "Fliegenesser"};

	public void startNewSinglePlayerGame() {
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
		this.actualLevelTime = defaultLevelTime;
		this.didStartLevel = false;

		this.loadNextLevel ();
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
		this.actualLevelTime = this.defaultLevelTime;

		// load random next level
		int randomNumber = Random.Range(0, this.levels.Length * 5);
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

	public void setLevelTime(float levelTime) {
		this.actualLevelTime = levelTime;
	}
}

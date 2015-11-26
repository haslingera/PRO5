using UnityEngine;
using System.Collections;

public class GameLogic {

	// Singleton Methods
	private static GameLogic instance;
	
	// Static singleton property
	public static GameLogic Instance {
		// Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		// otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameLogic()); }
	}

	


	// -------------------------
	//    Actual Class Data
	// -------------------------
	
	private int numberOfLives;
	private int numberOfLevelsCompleted;
	private string[] levels = new string[] {"Test_Blendshapes", "Flappy Schrei", "Pong Schrei", "Fliegenesser", "Plattformen-Szene-Absolut"};

	public void startNewSinglePlayerGame() {
		this.numberOfLives = 3;
		this.numberOfLevelsCompleted = 0;
	}

	public void didFinishLevel() {
		this.numberOfLevelsCompleted++;
	}

	public void loadNextLevel() {
		// load random next level
		int randomNumber = Random.Range(0, this.levels.Length);
		Application.LoadLevel (this.levels[randomNumber]);
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
}

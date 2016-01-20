using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class AudioPlayer : MonoBehaviour {

	//slider input
	/*
	public float soundVolume = 0.5;
	public float musicVolume = 0.5;
	*/

	private static AudioPlayer instance;

	private Queue<AudioClip> audioQueue;
	private AudioSource audioSource;
	private AudioSource tickTockAudioSource;
	private AudioSource tickTockEndAudioSource;
	private AudioSource melodyAudioSource;
	private float timeSinceLastPlay; // in seconds
	private float timeOfLastPlayedClip; // in seconds

	public delegate void OnTickTockStartedAction();
	public event OnTickTockStartedAction OnTickTockStarted;
 
	// Singleton Methods
	// Static singleton property
	public static AudioPlayer Instance {
		// Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		// otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameObject ("AudioPlayer").AddComponent<AudioPlayer> ()); }
	}
	
	void Awake() {
		DontDestroyOnLoad(this.transform.gameObject);

		this.audioSource = this.gameObject.AddComponent<AudioSource> ();

		this.tickTockAudioSource = this.gameObject.AddComponent<AudioSource> ();
		this.tickTockEndAudioSource = this.gameObject.AddComponent<AudioSource> ();
		this.melodyAudioSource = this.gameObject.AddComponent<AudioSource> ();

		AudioClip tickTockClip = Resources.Load ("TickTock") as AudioClip;
		AudioClip tickTockEndClip = Resources.Load ("TickTockEnd") as AudioClip;
		AudioClip melodyClip = Resources.Load ("Melody") as AudioClip;

		this.tickTockAudioSource.clip = tickTockClip;
		this.tickTockEndAudioSource.clip = tickTockEndClip;
		this.melodyAudioSource.clip = melodyClip;

		audioQueue = new Queue<AudioClip>();
	}

	protected void Update() {
		timeSinceLastPlay += Time.deltaTime;
	}

	public void Init() { /* do nothing */ }

	public void startIntroAudio(int bpm) {
		this.melodyAudioSource.pitch = GameLogic.Instance.getLevelSpeed ();
		this.melodyAudioSource.PlayDelayed (0);
		this.tickTockEndAudioSource.loop = false;

		Invoke ("playTickTockAudioSourceDelayed", 8 * (60.0f / bpm));
		Invoke ("sendTickTockStartedEvent", 8 * (60.0f / bpm));
		//Invoke ("playTickTockEndAudioSourceDelayed", ((8 + numberOfBeats - 4) * (60.0f / bpm)));
	}

	public void reScheduleTickTockEndWithDelay(float delay) {
		Debug.Log ("reSchedule, delay: " + delay);
		CancelInvoke ("playTickTockEndAudioSourceDelayed");
		Invoke ("playTickTockEndAudioSourceDelayed", delay);
	}

	public void stopLoopingTickTock() {
		this.tickTockAudioSource.loop = false;
	}

	public void playTickTockAudioSourceDelayed() {
		this.tickTockAudioSource.pitch = GameLogic.Instance.getLevelSpeed ();
		this.tickTockAudioSource.Play();
		this.tickTockAudioSource.loop = true;
	}

	public void playTickTockEndAudioSourceDelayed() {
		this.tickTockEndAudioSource.pitch = GameLogic.Instance.getLevelSpeed ();
		this.tickTockEndAudioSource.Play();
		this.tickTockEndAudioSource.loop = false;
	}

	private void sendTickTockStartedEvent() {
		Debug.Log ("Seinding Tick Tock Started Event");
		// send broadcast event that tick tock phase has started
		if (this.OnTickTockStarted != null) {
			this.OnTickTockStarted ();
		}

		// by now the gamelogic should know how many beats the level lasts, use this information to call the playTickTockEndAudioSourceDelayed
		Invoke("playTickTockEndAudioSourceDelayed", (GameLogic.Instance.getCurrentLevelNumberOfBeats() - 4) * (60.0f / GameLogic.Instance.getCurrentBPM()));
		Invoke ("stopLoopingTickTock", (GameLogic.Instance.getCurrentLevelNumberOfBeats() - 5) * (60.0f / GameLogic.Instance.getCurrentBPM ()));
	}








	// --------------------------
	// Traditional AudioManager Code
	// --------------------------


	public void queueAudioClip(AudioClip audioClip) {
		this.queueAudioClip (audioClip, 0.0f);
	}

	public void queueAudioClip(AudioClip audioClip, float delay) {
		Debug.Log ("queue audio clip called");
		// calculate time for next audio playback
		AudioClip[] audioClips = audioQueue.ToArray();
		float totalWaitingTime = delay; // seconds
		foreach (AudioClip ac in audioClips) {
			totalWaitingTime += ac.length;
		}
			
		totalWaitingTime += Mathf.Max (0, timeOfLastPlayedClip - timeSinceLastPlay);
		int totalWaitingTimeInt = (int) totalWaitingTime * 1000;

		audioQueue.Enqueue(audioClip);
		Invoke ("playNextClipInQueue", totalWaitingTime);
	}

	public bool playAudioClipIfFree(AudioClip audioClip) {
		if (timeSinceLastPlay > timeOfLastPlayedClip && audioQueue.Count == 0) {
			timeOfLastPlayedClip = audioClip.length;
			timeSinceLastPlay = 0.0f;

			audioSource.PlayOneShot (audioClip);
			return true;
		}

		return false;
	}

	public void playAudioClipForced(AudioClip audioClip) {
		//  stop all other playback and delete the queue
		audioQueue.Clear ();
		audioSource.Stop ();

		timeOfLastPlayedClip = audioClip.length;
		timeSinceLastPlay = 0.0f;

		audioSource.PlayOneShot (audioClip);
	}

	public void playSoundEffect(AudioClip audioClip) {
		audioSource.PlayOneShot (audioClip);
	}

	private void playNextClipInQueue() {
		Debug.Log ("Playing next shot out of queue: ");

		// save the time and lenght of the clip that is going to be played
		timeOfLastPlayedClip = audioQueue.Peek ().length;
		timeSinceLastPlay = 0.0f;

		// play the audio clip
		audioSource.PlayOneShot (audioQueue.Dequeue ());
	}




	// ----------------------
	// Global Volume Control
	// ----------------------


	public void ChangeSoundVolume(float soundVolume){ //between 0 and 1
		//change volume of sound effects (speech)
		//AudioListener audioListner = GameObject.Find ("Main Camera").GetComponent<AudioListener> ();
		//AudioListener.volume = soundVolume;
		audioSource.volume = soundVolume;
		PlayerPrefs.SetFloat("SoundVolume", (float) soundVolume);
		// Best tutorial:
		// http://answers.unity3d.com/questions/306684/how-to-change-volume-on-many-audio-objects-with-sp.html
	}

	public void ChangeMusicVolume(float musicVolume){ // between 0 and 1
		//change volume of background music
		tickTockAudioSource.volume = musicVolume;
		PlayerPrefs.SetFloat("MusicVolume", (float) musicVolume);
		// Best tutorial:
		// http://answers.unity3d.com/questions/306684/how-to-change-volume-on-many-audio-objects-with-sp.html
	}
}

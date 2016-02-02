using UnityEngine;
using System.Collections;

public class GlassDestroyer : MonoBehaviour {

	private StationaryMovement movement;
	private float totalScreamTime;
	private float startTime;
	private bool screaming = false;
	private bool stopscream = false;
	private float endTime;
	private float targetPitch;
	private float targetPitchOffset = 50.0f;
    private SkinnedMeshRenderer scream;

	public GameObject broken;

	private bool levelDidStart;

	void Awake() {
		AudioAnalyzer.Instance.Init ();
	}

	void Start () {
		movement = GetComponent<StationaryMovement> ();
		totalScreamTime = 0;
		screaming = false;
		stopscream = false;
		endTime = 0;
		startTime = Time.time;
		targetPitch = Random.Range (150, 550);
		this.levelDidStart = false; // set this to false when game is ready for distribution
		this.levelDidStart = GameLogic.Instance.getLevelIsReadyToStart();
        scream = GameObject.Find("punk").GetComponent<SkinnedMeshRenderer>();

	}

	void OnEnable() {
		GameLogic.Instance.OnLevelReadyToStart += levelReadyToStart;
	}

	void OnDisable() {
		GameLogic.Instance.OnLevelReadyToStart -= levelReadyToStart;
	}

	private void levelReadyToStart() {
		this.levelDidStart = true;
	}

	void Update () {
		if (this.levelDidStart) {
            //moves while screaming
            lost();
			if (screaming) {
				transform.position = new Vector3 (Mathf.Sin ((Time.time - startTime) * 20) * (Time.time - startTime) / 20f, transform.position.y, transform.position.z);
			}

			//proves if still screaming
			if (AudioAnalyzer.Instance.getPitch () >= targetPitch - targetPitchOffset && AudioAnalyzer.Instance.getPitch () <= targetPitch + targetPitchOffset) {
				endTime = Time.time;
			}

			//proves
			if (AudioAnalyzer.Instance.getMicLoudness () > -30 && (AudioAnalyzer.Instance.getPitch () <= targetPitch - targetPitchOffset || AudioAnalyzer.Instance.getPitch () >= targetPitch + targetPitchOffset)) {
				endTime = Time.time;
			}


			if (!screaming && (AudioAnalyzer.Instance.getPitch () >= targetPitch - targetPitchOffset && AudioAnalyzer.Instance.getPitch () <= targetPitch + targetPitchOffset)) {
				screaming = true;
				startTime = Time.time;
			}

			if (Time.time - endTime >= 0.5f) {
				screaming = false;
				startTime = Time.time;
			}
			
			if (screaming && Time.time - startTime >= 4f/GameLogic.Instance.getLevelSpeed()) {
				GameObject clone;
				clone = Instantiate (broken, transform.position, broken.transform.rotation) as GameObject; 
				Destroy (gameObject);
				GameLogic.Instance.didFinishLevel ();
                scream.SetBlendShapeWeight(0,0.0f);
			}
		}
	}

    private void lost()
    {
        if (!GameLogic.Instance.getIsLevelActive())
        {
            scream.SetBlendShapeWeight(0, 0.0f);
        }
    }

}

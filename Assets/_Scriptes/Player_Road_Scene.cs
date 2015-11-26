using UnityEngine;
using System.Collections;

public class Player_Road_Scene : MonoBehaviour {

	public bool talkDirtyToMe = false;
	int count = 1;
	int blink;
	SkinnedMeshRenderer skinnedMeshRendererEyes;
	float db;
	Vector3 start;
	Vector3 end;
	bool move = false;

	// Use this for initialization
	void Start () {

		blink = Random.Range (70,200);
		skinnedMeshRendererEyes = GameObject.Find ("Eyes").GetComponent<SkinnedMeshRenderer> ();
		start = this.transform.position;
		end = new Vector3 (20f, start.y,start.z);
	
	}
	
	// Update is called once per frame
	void Update () {

		Blink ();

		db = AudioAnalyzer.Instance.getMicLoudness();

		if (!move) {
			if (db > 20f) {
				iTween.MoveTo (this.gameObject, iTween.Hash ("x", 20, "easetype", "linear", "time", 4f));
				move = true;
			}
		} else {
			if (db > 20f) {
				iTween.Stop (this.gameObject);
				move = false;
			}
		}
	
	}

	void Blink(){
		
		if (count > blink) {
			skinnedMeshRendererEyes.SetBlendShapeWeight (4, 100);
			skinnedMeshRendererEyes.SetBlendShapeWeight (5, 100);
			if (count > blink+20){
				count = 0;
				blink = Random.Range (100,300);
			}
		} else {
			skinnedMeshRendererEyes.SetBlendShapeWeight (4, 0);
			skinnedMeshRendererEyes.SetBlendShapeWeight (5, 0);
		}
		
		count++;
	}

	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}

	public void resetPlayer(){

		this.transform.position = start;

	}
}

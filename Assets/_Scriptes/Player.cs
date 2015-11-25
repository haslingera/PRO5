using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public int lives = 2;
	//int blendShapeCount;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Mesh skinnedMesh;
	SkinnedMeshRenderer skinnedMeshRendererEyes;
	Mesh skinnedMeshEyes;
	//float blendOne = 0f;
	//float blendTwo = 0f;
	//float blendSpeed = 1f;
	//bool blendOneFinished = false;
	public bool talkDirtyToMe = false;
	public bool talkFrequ = false;
	public bool rotate = false;
	int count = 1;
	int blink;
	float oldAngle = 0;

	// Use this for initialization
	void Start () {

		blink = Random.Range (70,200);
	
		skinnedMeshRenderer = GameObject.Find ("Neutral").GetComponent<SkinnedMeshRenderer> ();
		skinnedMesh = GameObject.Find ("Neutral").GetComponent<SkinnedMeshRenderer> ().sharedMesh;

		skinnedMeshRendererEyes = GameObject.Find ("Eyes").GetComponent<SkinnedMeshRenderer> ();
		skinnedMeshEyes = GameObject.Find ("Eyes").GetComponent<SkinnedMeshRenderer> ().sharedMesh;

	}
	
	// Update is called once per frame
	void Update () {

		Blink ();
		if (talkDirtyToMe) {
			float tempDB = getDB();
			skinnedMeshRenderer.SetBlendShapeWeight (0, tempDB);
			skinnedMeshRendererEyes.SetBlendShapeWeight(8, tempDB);
		}

		if (talkFrequ) {
			
			float tempFQ = GameObject.Find ("Audio").GetComponent<InputAnalyser> ().getPitch();
			if(tempFQ != -1){
				skinnedMeshRenderer.SetBlendShapeWeight (0, tempFQ/8);
				skinnedMeshRendererEyes.SetBlendShapeWeight(6, tempFQ/8);
			}
			else
				skinnedMeshRenderer.SetBlendShapeWeight(0,0f);
		}

		/*if (blendShapeCount > 1) {
			
			if (blendOne < 100f) {
				skinnedMeshRenderer.SetBlendShapeWeight (0, blendOne);
				blendOne += blendSpeed;
			} else {
				blendOneFinished = true;
			}
			
			if (blendOneFinished == true && blendTwo < 100f) {
				skinnedMeshRenderer.SetBlendShapeWeight (1, blendTwo);
				blendTwo += blendSpeed;
			}

		}
	*/
	
		if (rotate) {

			var targetPos = GameObject.Find ("Enemy").transform.position;
			targetPos.y = this.transform.position.y; //set targetPos y equal to mine, so I only look at my own plane
			var targetDir = Quaternion.LookRotation(targetPos - this.transform.position);
			this.transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, 6*Time.deltaTime);
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

	float getDB(){

		float db;
		float endDB = 0;
		db = GameObject.Find ("Audio").GetComponent<InputAnalyser> ().MicLoudness;
		endDB = (Mathf.InverseLerp(-70, 40, db))*100;
		//Debug.Log(endDB);
		if (endDB < 40)
			return 0;
		return endDB;
	}

}

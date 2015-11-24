using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public int lives = 2;
	//int blendShapeCount;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Mesh skinnedMesh;
	//float blendOne = 0f;
	//float blendTwo = 0f;
	//float blendSpeed = 1f;
	//bool blendOneFinished = false;
	public bool talkDirtyToMe = false;
	public bool talkFrequ = false;

	// Use this for initialization
	void Start () {
	
		skinnedMeshRenderer = GameObject.Find ("blobBody_neutral").GetComponent<SkinnedMeshRenderer> ();
		skinnedMesh = GameObject.Find ("blobBody_neutral").GetComponent<SkinnedMeshRenderer> ().sharedMesh;

	}
	
	// Update is called once per frame
	void Update () {

		if (talkDirtyToMe) {

			float tempDB = GameObject.Find ("Audio").GetComponent<InputAnalyser> ().MicLoudness;

			skinnedMeshRenderer.SetBlendShapeWeight (0, tempDB + 60);
		}

		if (talkFrequ) {
			
			float tempFQ = GameObject.Find ("Audio").GetComponent<InputAnalyser> ().getPitch();
			if(tempFQ != -1)
				skinnedMeshRenderer.SetBlendShapeWeight (0, tempFQ/8);
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

	
	}
}

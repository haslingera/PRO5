﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public int lives = 2;
	int blendShapeCount;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Mesh skinnedMesh;
	float blendOne = 0f;
	float blendTwo = 0f;
	float blendSpeed = 1f;
	bool blendOneFinished = false;

	// Use this for initialization
	void Start () {
	
		skinnedMeshRenderer = GameObject.Find ("blobBody_neutral").GetComponent<SkinnedMeshRenderer> ();
		skinnedMesh = GameObject.Find ("blobBody_neutral").GetComponent<SkinnedMeshRenderer> ().sharedMesh;

	}
	
	// Update is called once per frame
	void Update () {

		float tempDB = GameObject.Find ("Audio").GetComponent<InputAnalyser>().MicLoudness;

		skinnedMeshRenderer.SetBlendShapeWeight (0, tempDB+60);

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

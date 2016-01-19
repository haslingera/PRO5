using UnityEngine;
using System.Collections;

public class Player_Animation : MonoBehaviour {

	public int lives = 2;
	//int blendShapeCount;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Mesh skinnedMesh;
	SkinnedMeshRenderer skinnedMeshRendererEyes;
	Mesh skinnedMeshEyes;

	public bool talkDirtyToMe = false;
	public bool talkFrequ = false;
	int count = 1;
	int blink;
    public bool neutral = false;
    public bool highRes = false;
    bool blinken = false;
	
	// Use this for initialization
	void Start () {
		
		blink = Random.Range (70,200);

        if (neutral)
        {
            skinnedMeshRenderer = GameObject.Find("Neutral").GetComponent<SkinnedMeshRenderer>();
            skinnedMesh = GameObject.Find("Neutral").GetComponent<SkinnedMeshRenderer>().sharedMesh;

            skinnedMeshRendererEyes = GameObject.Find("Eyes").GetComponent<SkinnedMeshRenderer>();
            skinnedMeshEyes = GameObject.Find("Eyes").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            blinken = true;
        }

        if (highRes)
        {
            blinken = false;
            skinnedMeshRenderer = GameObject.Find("Player").GetComponent<SkinnedMeshRenderer>();
            skinnedMesh = GameObject.Find("Player").GetComponent<SkinnedMeshRenderer>().sharedMesh;

        }

    }
	
	// Update is called once per frame
	void Update () {
		
        if(blinken)
		    Blink ();

		if (talkDirtyToMe) {
			float tempDB = getDB();

			skinnedMeshRenderer.SetBlendShapeWeight (0, tempDB);
            if(blinken)
			    skinnedMeshRendererEyes.SetBlendShapeWeight(8, tempDB);
		}
		
		if (talkFrequ) {
			
			float tempFQ = AudioAnalyzer.Instance.getPitch();
			if(tempFQ != -1){
				skinnedMeshRenderer.SetBlendShapeWeight (0, tempFQ/8);
                if(blinken)
				    skinnedMeshRendererEyes.SetBlendShapeWeight(6, tempFQ/8);
			}
			else
				skinnedMeshRenderer.SetBlendShapeWeight(0,0f);
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
		db = AudioAnalyzer.Instance.getMicLoudness();
		endDB = (Mathf.InverseLerp(-70, 40, db))*100;
		//Debug.Log(endDB);
		if (endDB < 40)
			return 0;
		return endDB;
	}
	
	void Awake(){
		AudioAnalyzer.Instance.Init ();
	}
	
}


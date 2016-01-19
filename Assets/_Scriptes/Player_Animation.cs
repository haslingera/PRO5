using UnityEngine;
using System.Collections;

public class Player_Animation : MonoBehaviour {

	int counter = 0;
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
    int blinker = 45;
    public float blinkSpeed = 0.0f;
	
	// Use this for initialization
	void Start () {
		
		blink = Random.Range (70,200);

        if (neutral)
        {
            skinnedMeshRenderer = GameObject.Find("Neutral").GetComponent<SkinnedMeshRenderer>();
            skinnedMesh = GameObject.Find("Neutral").GetComponent<SkinnedMeshRenderer>().sharedMesh;

            skinnedMeshRendererEyes = GameObject.Find("Eyes").GetComponent<SkinnedMeshRenderer>();
            skinnedMeshEyes = GameObject.Find("Eyes").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        if (highRes)
        {
            skinnedMeshRenderer = GameObject.Find("Player").GetComponent<SkinnedMeshRenderer>();
            skinnedMesh = GameObject.Find("Player").GetComponent<SkinnedMeshRenderer>().sharedMesh;

        }

    }
	
	// Update is called once per frame
	void Update () {

        if (neutral)
        {
            Blink();
        }
        else
        {
            Blink2();
        }

		if (talkDirtyToMe) {
			float tempDB = getDB();

			skinnedMeshRenderer.SetBlendShapeWeight (0, tempDB);
            if(neutral)
			    skinnedMeshRendererEyes.SetBlendShapeWeight(8, tempDB);
		}
		
		if (talkFrequ) {
			
			float tempFQ = AudioAnalyzer.Instance.getPitch();
			if(tempFQ != -1){
				skinnedMeshRenderer.SetBlendShapeWeight (0, tempFQ/8);
                if(neutral)
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

    void Blink2() {

        GameObject temp = GameObject.Find("eyes_default");

        if(counter == 0)
        {
            blinker = (int)Random.Range(1, 50);
        }

        if (blinker == 45)
        {

            if (counter < 20)
            {
                temp.transform.localScale -= new Vector3(0f, blinkSpeed, 0f);
                counter++;
            }
            else {
                temp.transform.localScale += new Vector3(0f, blinkSpeed, 0f);
                counter++;
            }

            if (counter == 40)
            {
                counter = 0;
            }

        }
    }

}


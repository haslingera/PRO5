using UnityEngine;
using System.Collections;

public class Player_Animation : MonoBehaviour {

	int counter = 0;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Mesh skinnedMesh;
	SkinnedMeshRenderer skinnedMeshRendererEyes;
	Mesh skinnedMeshEyes;
    SkinnedMeshRenderer skinnedMeshRendererTongue;
    Mesh skinnedMeshTongue;

	public bool usingOwnTalk = false;
    public bool talkDirtyToMe = false;
	public bool talkFrequ = false;
    public bool tongue = false;
	int count = 1;
	int blink;
    public bool neutral = false;
    public bool highRes = false;
    bool blinken = false;
    int blinker = 45;
    public float blinkSpeed = 0.05f;
    public bool präsi = false;
    private bool stop;
	
	// Use this for initialization
	void Start () {
		
		blink = Random.Range (70,200);

        if (neutral)
        {
            skinnedMeshRenderer = GameObject.Find("Neutral").GetComponent<SkinnedMeshRenderer>();

            skinnedMeshRendererEyes = GameObject.Find("Eyes").GetComponent<SkinnedMeshRenderer>();

            if (tongue)
            {
                skinnedMeshRendererTongue = GameObject.Find("Tounge_default1").GetComponent<SkinnedMeshRenderer>();
            }
        }

        if (highRes)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        }


    }

    // Update is called once per frame
    void Update() {
		if (!talkDirtyToMe && !usingOwnTalk) {
			skinnedMeshRenderer.SetBlendShapeWeight (0, 0.0f);
		}

        int tmp = 0;
        int tmp2 = 0;
        float tempDB = 0;

        if (!präsi) { 
            stopAnimation();
        }

        if (neutral)
        {
            Blink();
        }
        else
        {
            Blink2();
        }
        if (stop)
        {
            if (talkDirtyToMe)
            {
                tempDB = getDB();
                tmp = ConvertRange(0, 100, 30, 48, (int)tempDB);
                tmp2 = ConvertRange(0, 100, 11, 48, (int)tempDB);
                skinnedMeshRenderer.SetBlendShapeWeight(0, tempDB);
                if (neutral)
                    skinnedMeshRendererEyes.SetBlendShapeWeight(8, tempDB);
            }

            if (talkFrequ)
            {

                float tempFQ = AudioAnalyzer.Instance.getPitch();
                //Debug.Log(tempFQ);
                tmp = ConvertRange(100, 650, 30, 48, (int)tempFQ);
                tmp2 = ConvertRange(100, 650, 11, 48, (int)tempFQ);
                if (tempFQ != -1)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(0, tempFQ / 8);
                    if (neutral)
                        skinnedMeshRendererEyes.SetBlendShapeWeight(6, tempFQ / 8);
                }
                else
                    skinnedMeshRenderer.SetBlendShapeWeight(0, 0f);
            }
        }

        if (tongue && neutral)
        {
            //Debug.Log("Yes  "+tmp);
            skinnedMeshRendererTongue.SetBlendShapeWeight(0, (float)tmp2);
            skinnedMeshRendererTongue.SetBlendShapeWeight(1, (float)tmp-Random.Range(1,5));
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

    public static int ConvertRange(
    int originalStart, int originalEnd, // original range
    int newStart, int newEnd, // desired range
    int value) // value to convert
    {
        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (int)(newStart + ((value - originalStart) * scale));
    }

    void stopAnimation()
    {
        stop = GameLogic.Instance.getIsLevelActive();
    }

}


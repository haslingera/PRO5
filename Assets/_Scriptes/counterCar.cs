using UnityEngine;
using System.Collections;

public class counterCar : MonoBehaviour {

	public int c1 = 1;
	public int c2 = 1;
	public int c3 = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		
		if (GameObject.Find("Car1") != null)
			addC1 ();
		if (GameObject.Find("Car2") != null)
			addC2 ();
		if (GameObject.Find("Car3") != null)
			addC3 ();
	
	}

	public void addC1(){
		c1++;
	}
	public void addC2(){
		c2++;
	}
	public void addC3(){
		c3++;
	}
	public void subC1(){
		c1--;
	}
	public void subC2(){
		c2--;
	}
	public void subC3(){
		c3--;
	}
	public int getCars(int x){
		if (x==1)
			return c1;
		if (x==2)
			return c2;
		if (x==3)
			return c3;
		return 0;
	}

}

using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

	private Renderer mesh;
	public Material[] materials;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<Renderer> ();
		mesh.enabled = true;

	}

	// Update is called once per frame
	void FixedUpdate () {
		if (GetComponent<InputAnalyser>().MicLoudness > 0.1f) {
			mesh.material = materials[0] ;
		} else {
			mesh.material = materials[1];
		}
	}
}

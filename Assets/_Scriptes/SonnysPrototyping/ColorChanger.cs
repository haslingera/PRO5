using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

	private Renderer mesh;
	public Material defaultColor;
	public Material changingColor;
	public float minPitch;
	public float maxPitch;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<Renderer> ();
		mesh.enabled = true;
		AudioAnalyzer.Instance.Init ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (AudioAnalyzer.Instance.getPitch () > minPitch && AudioAnalyzer.Instance.getPitch () < maxPitch) {
			mesh.material = changingColor;
		} else {
			mesh.material = defaultColor;
		}
	}
}

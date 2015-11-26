using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

	private Renderer mesh;
	public Material defaultColor;
	public Material changingColor;
	public bool volume;
	public float min;
	public float max;


	// Use this for initialization
	void Start () {
		mesh = GetComponent<Renderer> ();
		mesh.enabled = true;
		AudioAnalyzer.Instance.Init ();
	}

	// Update is called once per frame
	void Update () {
		if (!volume) {
			if (AudioAnalyzer.Instance.getPitch() > min && AudioAnalyzer.Instance.getPitch() < max) {
				mesh.material = changingColor;
			} else {
				mesh.material = defaultColor;
			}
		} else {
			if (AudioAnalyzer.Instance.getMicLoudness() > min && AudioAnalyzer.Instance.getMicLoudness() < max) {
				mesh.material = changingColor;
			} else {
				mesh.material = defaultColor;
			}
		}

	}
}

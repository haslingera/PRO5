using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BlurFade : MonoBehaviour {
	public int iterations = 3;
	public float blurSpread = 0.6f;
	public float qualityReduction = 1f;
	public Shader blurShader;
	
	private Material mat;
	private Material material {
		get {
			if(mat == null) {
				mat = new Material(blurShader);
				mat.hideFlags = HideFlags.DontSave;
			}
			return mat;
		}
	}
	
	void OnDisable() {
		if(mat) {
			DestroyImmediate(mat);
		}
	}
	
	void Start() {
		if(!SystemInfo.supportsImageEffects || !blurShader || !material.shader.isSupported) {
			enabled = false;
			return;
		}
	}
	
	private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration) {
		float offset = blurSpread;
		Graphics.BlitMultiTap(source, dest, material,
		                      new Vector2(-offset, -offset),
		                      new Vector2(-offset,  offset),
		                      new Vector2(offset,  offset),
		                      new Vector2(offset, -offset)
		                      );
	}
	
	private void DownSample4x(RenderTexture source, RenderTexture dest) {
		float offset = Mathf.Clamp01(blurSpread * 0.5f);
		Graphics.BlitMultiTap(source, dest, material,
		                      new Vector2(-offset, -offset),
		                      new Vector2(-offset, offset),
		                      new Vector2(offset, -offset),
		                      new Vector2(offset, offset)
		                      );
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		float blurFactor = Mathf.Clamp01(blurSpread);
		if(blurFactor <= 0f) {
			Graphics.Blit(source, destination);
			return;
		}
		
		qualityReduction = Mathf.Clamp(qualityReduction, 0f, 7f);
		iterations = Mathf.Clamp(iterations, 0, 10);
		blurSpread = Mathf.Clamp(blurSpread, 0f, 16f);
		int rtW = (int)((source.width * 1f) / (1f + (qualityReduction * blurFactor)));
		int rtH = (int)((source.height * 1f) / (1f + (qualityReduction * blurFactor)));
		source.filterMode = FilterMode.Bilinear;
		RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
		
		DownSample4x(source, buffer);
		
		for(int i = 0; i < iterations; i++) {
			RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
			FourTapCone(buffer, buffer2, i);
			RenderTexture.ReleaseTemporary(buffer);
			buffer = buffer2;
		}
		
		Graphics.Blit(buffer, destination);
		RenderTexture.ReleaseTemporary(buffer);
	}
}
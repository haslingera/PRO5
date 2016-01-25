using UnityEngine;
using System.Collections;

public class ScreenWipe : MonoBehaviour 
{
	public enum ZoomType
	{
		Grow, Shrink
	}

	public enum TransitionType 
	{
		Left, Right, Up, Down
	}
	
	private Texture tex;
	private RenderTexture renderTex;
	private Texture2D tex2D;
	private float alpha;
	private bool  reEnableListener;
	private Material shapeMaterial;
	private Transform shape;
	private AnimationCurve curve;
	private bool  useCurve;
	public static ScreenWipe use;
	
	

	void Awake ()
	{
		if (use) 
		{
			Debug.LogWarning("Only one instance of ScreenCrossFadePro is allowed");
			return;
		}
		use = this;

		this.enabled = false;
	}

	void OnGUI ()
	{
		GUI.depth = -9999999;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), tex);
	}

	IEnumerator AlphaTimer ( float time )
	{
		float rate = 1.0f/time;
		
		if( useCurve )
		{
			float t = 0.0f;
			
			while( t < 1.0f )
			{
				alpha = curve.Evaluate( t );
				t += Time.deltaTime * rate;
				yield return 0;
			}
		}
		else
		{
			for (alpha = 1.0f; alpha > 0.0f; alpha -= Time.deltaTime * rate)
			{
				yield return 0;
			}
		}
	}

	void CameraSetup( Camera cam1, Camera cam2, bool cam1Active, bool enableThis )
	{
		if (enableThis) 
		{
			this.enabled = true;
		}
		
		cam1.gameObject.active = cam1Active;
		cam2.gameObject.active = true;
		AudioListener listener = cam2.GetComponent<AudioListener>();
		
		if (listener)
		{
			reEnableListener = listener.enabled? true : false;
			listener.enabled = false;
		}
	}

	void CameraCleanup ( Camera cam1, Camera cam2 )
	{
		AudioListener listener = cam2.GetComponent<AudioListener>();
		if (listener && reEnableListener) {
			listener.enabled = true;
		}
		cam1.gameObject.active = false;
		this.enabled = false;
	}

	public IEnumerator CrossFadePro ( Camera cam1, Camera cam2, float time )
	{
		if (!renderTex) 
		{
			renderTex = new RenderTexture(Screen.width, Screen.height, 24);
		}
		
		cam1.targetTexture = renderTex;
		tex = renderTex;
		CameraSetup (cam1, cam2, true, true);
		
		// Debug.Log("CrossFadePro() begun");
		yield return StartCoroutine( AlphaTimer( time ) );
		
		// Debug.Log("CrossFadePro() finished");
		cam1.targetTexture = null;
		renderTex.Release();
		CameraCleanup (cam1, cam2);
	}

	IEnumerator CrossFade ( Camera cam1, Camera cam2, float time )
	{
		yield return CrossFade( cam1, cam2, time, null );
	}
	
	public IEnumerator CrossFade ( Camera cam1, Camera cam2, float time, AnimationCurve _curve )
	{
		curve = _curve;
		useCurve = curve != null;
		
		if (!tex2D) 
		{
			tex2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		}
		
		tex2D.ReadPixels( new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
		tex2D.Apply();
		tex = tex2D;
		yield return 0;
		
		CameraSetup (cam1, cam2, false, true);

		yield return AlphaTimer(time);

		CameraCleanup (cam1, cam2);
	}

	IEnumerator RectWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   ZoomType zoom  )
	{
		yield return RectWipe( cam1, cam2, time, zoom, null );
	}
	
	public IEnumerator RectWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   ZoomType zoom ,   AnimationCurve _curve  )
	{
		curve = _curve;
		useCurve = curve != null;
		CameraSetup (cam1, cam2, true, false);
		Camera useCam = (zoom == ZoomType.Shrink)? cam1 : cam2;
		Camera otherCam = (zoom == ZoomType.Shrink)? cam2 : cam1;
		Rect originalRect = useCam.rect;
		float originalDepth = useCam.depth;
		useCam.depth = otherCam.depth+1;

		if( useCurve )
		{
			float rate= 1.0f/(time);
			
			if (zoom == ZoomType.Shrink) 
			{
				for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * rate) {
					float t = curve.Evaluate( i )*0.5f;
					cam1.rect = new Rect(t, t, 1.0f-t*2, 1.0f-t*2);
					yield return 0;
				}
			}
			else 
			{
				for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * rate) {
					float t = curve.Evaluate( i )*0.5f;
					cam2.rect = new Rect(.5f - t, .5f - t, t * 2.0f, t * 2.0f);
					yield return 0;
				}
			}
		}
		else
		{
			float rate = 1.0f/(time*2);
			if (zoom == ZoomType.Shrink) 
			{
				for (float i = 0.0f; i < .5; i += Time.deltaTime * rate) 
				{
					float t = Mathf.Lerp(0.0f, .5f, Mathf.Sin(i * Mathf.PI));	// Slow down near the end
					cam1.rect = new Rect(t, t, 1.0f-t*2, 1.0f-t*2);
					yield return 0;
				}
			}
			else 
			{
				for (float i = 0.0f; i < .5f; i += Time.deltaTime * rate) 
				{
					float t = Mathf.Lerp(.5f, 0.0f, Mathf.Sin((.5f-i) * Mathf.PI));	// Start out slower
					cam2.rect = new Rect(.5f-t, .5f-t, t*2.0f, t*2.0f);
					yield return 0;
				}
			}
		}

		useCam.rect = originalRect;
		useCam.depth = originalDepth;
		CameraCleanup (cam1, cam2);
	}

	IEnumerator ShapeWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   ZoomType zoom ,   Mesh mesh ,   float rotateAmount  )
	{
		yield return ShapeWipe( cam1, cam2, time, zoom, mesh, rotateAmount, null );
	}
	
	public IEnumerator ShapeWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   ZoomType zoom ,   Mesh mesh ,   float rotateAmount ,   AnimationCurve _curve  )
	{
		curve = _curve;
		
		useCurve = curve != null;
		
		if (!shapeMaterial) 
		{
			shapeMaterial = new Material (
				"Shader \"DepthMask\" {" +
				"   SubShader {" +
				"	   Tags { \"Queue\" = \"Background\" }" +
				"	   Lighting Off ZTest LEqual ZWrite On Cull Off ColorMask 0" +
				"	   Pass {}" +
				"   }" +
				"}"
			);
		}
		
		if (!shape) 
		{
			GameObject gobjShape = new GameObject("Shape");
			gobjShape.AddComponent<MeshFilter>();
			gobjShape.AddComponent<MeshRenderer>();
			shape = gobjShape.transform;
			shape.GetComponent<Renderer>().material = shapeMaterial;
		}

		CameraSetup (cam1, cam2, true, false);
		Camera useCam = (zoom == ZoomType.Shrink)? cam1 : cam2;
		Camera otherCam = (zoom == ZoomType.Shrink)? cam2 : cam1;
		float originalDepth = otherCam.depth;
		CameraClearFlags originalClearFlags = otherCam.clearFlags;
		otherCam.depth = useCam.depth+1;
		otherCam.clearFlags = CameraClearFlags.Depth;

		shape.gameObject.active = true;
		(shape.GetComponent<MeshFilter>() as MeshFilter).mesh = mesh;
		shape.position = otherCam.transform.position + otherCam.transform.forward * (otherCam.nearClipPlane+.01f);
		shape.parent = otherCam.transform;
		shape.localRotation = Quaternion.identity;

		if( useCurve )
		{
			float rate = 1.0f/time;
			
			if (zoom == ZoomType.Shrink) 
			{
				for (float i = 1.0f; i > 0.0f; i -= Time.deltaTime * rate) 
				{
					float t = curve.Evaluate( i );
					shape.localScale = new Vector3(t, t, t);
					shape.localEulerAngles = new Vector3(0.0f, 0.0f, i * rotateAmount);
					yield return 0;
				}
			}
			else 
			{
				for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * rate) 
				{
					float t = curve.Evaluate( i );
					shape.localScale = new Vector3(t, t, t);
					shape.localEulerAngles = new Vector3(0.0f, 0.0f, -i * rotateAmount);
					yield return 0;
				}   
			}
		}
		else{
			float rate = 1.0f/time;
			if (zoom == ZoomType.Shrink) {
				for (float i = 1.0f; i > 0.0f; i -= Time.deltaTime * rate) {
					float t = Mathf.Lerp(1.0f, 0.0f, Mathf.Sin((1.0f-i) * Mathf.PI * 0.5f));	// Slow down near the end
					shape.localScale = new Vector3(t, t, t);
					shape.localEulerAngles = new Vector3(0.0f, 0.0f, i * rotateAmount);
					yield return 0;
				}
			}
			else {
				for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * rate) {
					float t = Mathf.Lerp(1.0f, 0.0f, Mathf.Sin((1.0f-i) * Mathf.PI * 0.5f));		// Start out slower
					shape.localScale = new Vector3(t, t, t);
					shape.localEulerAngles = new Vector3(0.0f, 0.0f, -i * rotateAmount);
					yield return 0;
				}   
			}
		}

		otherCam.clearFlags = originalClearFlags;
		otherCam.depth = originalDepth;
		CameraCleanup (cam1, cam2);
		shape.parent = null;
		shape.gameObject.active = false;
	}

	IEnumerator SquishWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   TransitionType transitionType  )
	{
		yield return SquishWipe( cam1, cam2, time, transitionType, null );
	}
	
	public IEnumerator SquishWipe ( Camera cam1 ,   Camera cam2 ,   float time ,   TransitionType transitionType ,   AnimationCurve _curve  )
	{
		curve = _curve;
		useCurve = curve != null;
		Rect originalCam1Rect = cam1.rect;
		Rect originalCam2Rect = cam2.rect;
		Matrix4x4 cam1Matrix = cam1.projectionMatrix;
		Matrix4x4 cam2Matrix = cam2.projectionMatrix;
		CameraSetup (cam1, cam2, true, false);
		
		float rate = 1.0f/time;
		float t = 0.0f;
		float i = 0.0f;
		
		while( i < 1.0f )
		{
			if( useCurve )
			{ 
				i = curve.Evaluate(t);
				t += Time.deltaTime * rate;
			} 
			else
			{
				i += Time.deltaTime * rate;
			}
				
			switch (transitionType) 
			{
				case TransitionType.Right:
					cam1.rect = new Rect(i, 0, 1.0f, 1.0f);
					cam2.rect = new Rect(0, 0, i, 1.0f);
					break;
				case TransitionType.Left:
					cam1.rect = new Rect(0, 0, 1.0f-i, 1.0f);
					cam2.rect = new Rect(1.0f-i, 0, 1.0f, 1.0f);
					break;
				case TransitionType.Up:
					cam1.rect = new Rect(0, i, 1.0f, 1.0f);
					cam2.rect = new Rect(0, 0, 1.0f, i);
					break;
				case TransitionType.Down:
					cam1.rect = new Rect(0, 0, 1.0f, 1.0f-i);
					cam2.rect = new Rect(0, 1.0f-i, 1.0f, 1.0f);
					break;
			}
			
			cam1.projectionMatrix = cam1Matrix;
			cam2.projectionMatrix = cam2Matrix;
			
			yield return 0;
		}
		
		cam1.rect = originalCam1Rect;
		cam2.rect = originalCam2Rect;
		CameraCleanup (cam1, cam2);
	}

	public int planeResolution = 90;	// Higher numbers make the DreamWipe effect smoother, but take more CPU time
	private Vector3[] baseVertices;
	private Vector3[] newVertices;
	private Material planeMaterial;
	private GameObject plane;
	private RenderTexture renderTex2;

	public void InitializeDreamWipe ()
	{
	
		if (planeMaterial && plane) return;
		
		Debug.Log("InitializeDreamWipe here!");
		
		planeMaterial = new Material 
		(
			"Shader \"Unlit2Pass\" {" +
			"Properties {" +
			"	_Color (\"Main Color\", Color) = (1,1,1,1)" +
			"	_Tex1 (\"Base\", Rect) = \"white\" {}" +
			"	_Tex2 (\"Base\", Rect) = \"white\" {}" +
			"}" +
			"Category {" +
			"	ZWrite Off Alphatest Greater 0 ColorMask RGB Lighting Off" +
			"	Tags {\"Queue\"=\"Transparent\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}" +
			"	Blend SrcAlpha OneMinusSrcAlpha" +
			"	SubShader {" +
			"		Pass {SetTexture [_Tex2]}" +
			"		Pass {SetTexture [_Tex1] {constantColor [_Color] Combine texture * constant, texture * constant}}" +
			"	}" +
			"}}"
		);
		
		// Set up plane object
		plane = new GameObject("Plane");
		plane.AddComponent<MeshFilter>();
		plane.AddComponent<MeshRenderer>();
		plane.GetComponent<Renderer>().material = planeMaterial;
		plane.GetComponent<Renderer>().castShadows = false;
		plane.GetComponent<Renderer>().receiveShadows = false;
		plane.GetComponent<Renderer>().enabled = false;

		// Create the mesh used for the distortion effect
		Mesh planeMesh = new Mesh();
		(plane.GetComponent<MeshFilter>() as MeshFilter).mesh = planeMesh;
		
		planeResolution = Mathf.Clamp(planeResolution, 1, 16380);
		baseVertices = new Vector3[4*planeResolution + 4];
		newVertices = new Vector3[baseVertices.Length];
		Vector2[] newUV = new Vector2[baseVertices.Length];
		int[] newTriangles = new int[18*planeResolution];
		
		int idx = 0;
		for (int i = 0; i <= planeResolution; i++) 
		{
			float add = 1.0f/planeResolution*i;
			newUV[idx] = new Vector2(0.0f, 1.0f-add);
			baseVertices[idx++] = new Vector3(-1.0f, .5f-add, 0.0f);
			newUV[idx] = new Vector2(0.0f, 1.0f-add);
			baseVertices[idx++] = new Vector3(-.5f, .5f-add, 0.0f);
			newUV[idx] = new Vector2(1.0f, 1.0f-add);
			baseVertices[idx++] = new Vector3(.5f, .5f-add, 0.0f);
			newUV[idx] = new Vector2(1.0f, 1.0f-add);
			baseVertices[idx++] = new Vector3(1.0f, .5f-add, 0.0f);
		}
		
		idx = 0;
		
		for (int y = 0; y < planeResolution; y++) 
		{
			for (int x = 0; x < 3; x++) {
				newTriangles[idx++] = (y*4	  )+x;
				newTriangles[idx++] = (y*4	  )+x+1;
				newTriangles[idx++] = ((y+1)*4)+x;

				newTriangles[idx++] = ((y+1)*4)+x;
				newTriangles[idx++] = (y	*4)+x+1;
				newTriangles[idx++] = ((y+1)*4)+x+1;
			}
		}
		
		planeMesh.vertices = baseVertices;
		planeMesh.uv = newUV;
		planeMesh.triangles = newTriangles;
			
		// Set up rendertextures
		renderTex = new RenderTexture(Screen.width, Screen.height, 24);
		renderTex2 = new RenderTexture(Screen.width, Screen.height, 24);
		renderTex.filterMode = renderTex2.filterMode = FilterMode.Point;
		planeMaterial.SetTexture("_Tex1", renderTex);
		planeMaterial.SetTexture("_Tex2", renderTex2);
	}

	// IEnumerator DreamWipe ( Camera cam1 ,   Camera cam2 ,   float time  )
	// {
		// yield return DreamWipe (cam1, cam2, time, .07f, 25.0f);
	// }

	public IEnumerator DreamWipe ( Camera cam1, Camera cam2, float time, float waveScale, float waveFrequency )
	{
		if (!plane) 
		{
			InitializeDreamWipe();
		}
		
		waveScale = Mathf.Clamp(waveScale, -.5f, .5f);
		waveFrequency = .25f/(planeResolution/waveFrequency);
		
		CameraSetup (cam1, cam2, true, false);
		
		Debug.Log("DreamWipe() begun");

		// Make a camera that will show a plane with the combined rendertextures from cam1 and cam2,
		// and make it have the highest depth so it's always on top
		Camera cam2Clone = Instantiate(cam2, cam2.transform.position, cam2.transform.rotation) as Camera;
		cam2Clone.depth = cam1.depth+1;
		
		if (cam2Clone.depth <= cam2.depth) 
		{
			cam2Clone.depth = cam2.depth+1;
		}
		
		// Get screen coodinates of 0,0 in local spatial coordinates, so we know how big to scale the plane (make sure clip planes are reasonable)
		cam2Clone.nearClipPlane = .5f;
		cam2Clone.farClipPlane = 1.0f;
		Vector3 p = cam2Clone.transform.InverseTransformPoint(cam2.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cam2Clone.nearClipPlane)));
		plane.transform.localScale = new Vector3(-p.x*2.0f, -p.y*2.0f, 1.0f);
		plane.transform.parent = cam2Clone.transform;
		plane.transform.localPosition = plane.transform.localEulerAngles = Vector3.zero;
		
		// Must be a tiny bit beyond the nearClipPlane, or it might not show up
		plane.transform.Translate(Vector3.forward * (cam2Clone.nearClipPlane+.00005f));
		
		// Move the camera back so cam2 won't see the renderPlane, and parent it to cam2 so that if cam2 is moving, it won't see the plane
		cam2Clone.transform.Translate(Vector3.forward * -1.0f);
		cam2Clone.transform.parent = cam2.transform;
			
		// Initialize some stuff
		plane.GetComponent<Renderer>().enabled = true;
		float scale = 0.0f;
		Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
		cam1.targetTexture = renderTex;
		cam2.targetTexture = renderTex2;

		// Do the cross-fade
		float rate = 1.0f/time;
		
		for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * rate) 
		{
			planeMaterial.color = new Color( planeMaterial.color.r, planeMaterial.color.g, planeMaterial.color.b, Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(.75f, .15f, i)));
			
			// Make plane undulate
			for (int j = 0; j < newVertices.Length; j++) {
				newVertices[j] = baseVertices[j];
				newVertices[j].x += Mathf.Sin(j*waveFrequency + i*time) * scale;
			}
			
			planeMesh.vertices = newVertices;
			scale = Mathf.Sin(Mathf.PI * Mathf.SmoothStep(0.0f, 1.0f, i)) * waveScale;
			yield return 0;
		}
		
		Debug.Log("DreamWipe() finished");
		
		// Clean up
		CameraCleanup (cam1, cam2);
		plane.GetComponent<Renderer>().enabled = false;
		plane.transform.parent = null;
		Destroy(cam2Clone.gameObject);
		cam1.targetTexture = cam2.targetTexture = null;
		renderTex.Release();
		renderTex2.Release();
	}
}


















































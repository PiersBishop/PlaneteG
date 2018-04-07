using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSetup : MonoBehaviour {

	// gradient editing
	public Image[] Gradients;
	public Canvas[] GradientCanvases;
	public Slider[] GradientSliders;

	// resolution settings
	public InputField[] ResolutionInputs;
	public Camera[] Cameras;

	// camera settings
	public Slider[] CamLRAngles;
	public Slider[] CamUDAngles;
	public Slider[] CamDistances;
	public InputField[] CamFOV;
	public InputField[] CamTransformOptions;
	public InputField[] CamParentTransformOptions;

	//debug
	public Toggle dbToggle;
	public GameObject dbFrame;

	void Start () {
		SAVE (10);
		LOAD (100);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}



	void UpdateEverything() {
		ChangedGradient ();
		ChangedResolution ();
		ChangedCameraSettings (0);
		ChangedCameraSettings (1);
		SAVE (100);
	}

	public void SetDebug() {
		dbFrame.SetActive (dbToggle.isOn);

		foreach (Camera c in Cameras) {
			c.backgroundColor = dbToggle.isOn ? Color.red : Color.black;
		}
	}

	public void ChangedGradient(){
		float Reach1 = GradientSliders [0].value * 200f;
		float Reach2 = GradientSliders [1].value * 500f;

		foreach (Image i in Gradients) {
			i.rectTransform.sizeDelta = new Vector2 (Reach2, 150);
		}

		foreach (Canvas c in GradientCanvases) {
			CanvasScaler cs = c.GetComponent<CanvasScaler> ();
			cs.referencePixelsPerUnit = (int) Reach1;
		}
		SAVE (100);

	}

	public void ChangedResolution() {
		Vector2Int p1 = new Vector2Int (int.Parse(ResolutionInputs[0].text), int.Parse(ResolutionInputs[1].text));
		Vector2Int p2 = new Vector2Int (int.Parse(ResolutionInputs[2].text), int.Parse(ResolutionInputs[3].text));
		Vector2Int scr = new Vector2Int (int.Parse(ResolutionInputs[4].text), int.Parse(ResolutionInputs[5].text));

		Vector2Int tot = new Vector2Int (p1.x + p2.x + scr.x, Mathf.Max (p1.y, p2.y, scr.y));
		Screen.SetResolution(tot.x,tot.y, false);

		Vector2 p1prop = new Vector2 (p1.x / (float)tot.x, p1.y / (float)tot.y);
		Vector2 p2prop = new Vector2 (p2.x / (float)tot.x, p2.y / (float)tot.y);
		Vector2 scrprop = new Vector2 (scr.x / (float)tot.x, scr.y / (float)tot.y);

		Cameras[2].rect = new Rect (0, 1-scrprop.y, scrprop.x, scrprop.y);
		Cameras [0].rect = new Rect (scrprop.x, 0, p1prop.x, p1prop.y);
		Cameras [1].rect = new Rect (p1prop.x+scrprop.x, 0, p2prop.x, p2prop.y);

		SAVE (100);
	}

	public void ChangedCameraSettings(int cameraIndex){
		Transform cam = Cameras [cameraIndex].transform;
		Transform dist = cam.parent;
		Transform ud = dist.parent;
		Transform lr = ud.parent;
		Transform absolute = lr.parent;

		Cameras [cameraIndex].fieldOfView = float.Parse (CamFOV [cameraIndex].text);

		float z = CamDistances [cameraIndex].value * 5 + 1;
		float y = CamLRAngles [cameraIndex].value * 90 * (cameraIndex * 2 - 1);
		float x = CamUDAngles [cameraIndex].value * 90 - 45;

		dist.localPosition = Vector3.forward * z * -1;
		lr.localEulerAngles = Vector3.up * y;
		ud.localEulerAngles = Vector3.right * x;

		Vector3 camPos = new Vector3 (
			float.Parse( CamTransformOptions[cameraIndex*6 + 0].text),
			float.Parse( CamTransformOptions[cameraIndex*6 + 1].text),
			float.Parse( CamTransformOptions[cameraIndex*6 + 2].text)
		);
		Vector3 camRot = new Vector3 (
			float.Parse( CamTransformOptions[cameraIndex*6 + 3].text),
			float.Parse( CamTransformOptions[cameraIndex*6 + 4].text),
			float.Parse( CamTransformOptions[cameraIndex*6 + 5].text)
		);

		Vector3 parPos = new Vector3 (
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 0].text),
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 1].text),
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 2].text)
		);
		Vector3 parRot = new Vector3 (
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 3].text),
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 4].text),
			float.Parse( CamParentTransformOptions[cameraIndex*6 + 5].text)
		);

		cam.localPosition = camPos;
		cam.localEulerAngles = camRot;

		absolute.position = parPos;
		absolute.eulerAngles = parRot;

		SAVE (100);
	}

	public void SAVE (int index){
		string s = index.ToString ();

		// Gradients
		PlayerPrefs.SetFloat(s+"Reach1", GradientSliders [0].value);
		PlayerPrefs.SetFloat(s+"Reach2", GradientSliders [1].value);

		// Resolution
		PlayerPrefs.SetInt(s+"P1W", int.Parse(ResolutionInputs[0].text));
		PlayerPrefs.SetInt(s+"P1H", int.Parse(ResolutionInputs[1].text));
		PlayerPrefs.SetInt(s+"P2W", int.Parse(ResolutionInputs[2].text));
		PlayerPrefs.SetInt(s+"P2H", int.Parse(ResolutionInputs[3].text));
		PlayerPrefs.SetInt(s+"ScrW", int.Parse(ResolutionInputs[4].text));
		PlayerPrefs.SetInt(s+"ScrH", int.Parse(ResolutionInputs[5].text));

		// Camera1 Parameters
		PlayerPrefs.SetFloat(s+"C1FOV", float.Parse(CamFOV[0].text));
		PlayerPrefs.SetFloat (s+"C1X", CamUDAngles [0].value);
		PlayerPrefs.SetFloat (s+"C1Y", CamLRAngles [0].value);
		PlayerPrefs.SetFloat (s+"C1Z", CamDistances [0].value);
		// local params
		PlayerPrefs.SetFloat(s+"C1LocPosX", float.Parse( CamTransformOptions[0].text));
		PlayerPrefs.SetFloat(s+"C1LocPosY", float.Parse( CamTransformOptions[1].text));
		PlayerPrefs.SetFloat(s+"C1LocPosZ", float.Parse( CamTransformOptions[2].text));
		PlayerPrefs.SetFloat(s+"C1LocRotX", float.Parse( CamTransformOptions[3].text));
		PlayerPrefs.SetFloat(s+"C1LocRotY", float.Parse( CamTransformOptions[4].text));
		PlayerPrefs.SetFloat(s+"C1LocRotZ", float.Parse( CamTransformOptions[5].text));
		// parent params
		PlayerPrefs.SetFloat(s+"C1ParPosX", float.Parse( CamParentTransformOptions[0].text));
		PlayerPrefs.SetFloat(s+"C1ParPosY", float.Parse( CamParentTransformOptions[1].text));
		PlayerPrefs.SetFloat(s+"C1ParPosZ", float.Parse( CamParentTransformOptions[2].text));
		PlayerPrefs.SetFloat(s+"C1ParRotX", float.Parse( CamParentTransformOptions[3].text));
		PlayerPrefs.SetFloat(s+"C1ParRotY", float.Parse( CamParentTransformOptions[4].text));
		PlayerPrefs.SetFloat(s+"C1ParRotZ", float.Parse( CamParentTransformOptions[5].text));

		// Camera2 Parameters
		PlayerPrefs.SetFloat(s+"C2FOV", float.Parse(CamFOV[1].text));
		PlayerPrefs.SetFloat (s+"C2X", CamUDAngles [1].value);
		PlayerPrefs.SetFloat (s+"C2Y", CamLRAngles [1].value);
		PlayerPrefs.SetFloat (s+"C2Z", CamDistances [1].value);
		// local params
		PlayerPrefs.SetFloat(s+"C2LocPosX", float.Parse( CamTransformOptions[6].text));
		PlayerPrefs.SetFloat(s+"C2LocPosY", float.Parse( CamTransformOptions[7].text));
		PlayerPrefs.SetFloat(s+"C2LocPosZ", float.Parse( CamTransformOptions[8].text));
		PlayerPrefs.SetFloat(s+"C2LocRotX", float.Parse( CamTransformOptions[9].text));
		PlayerPrefs.SetFloat(s+"C2LocRotY", float.Parse( CamTransformOptions[10].text));
		PlayerPrefs.SetFloat(s+"C2LocRotZ", float.Parse( CamTransformOptions[11].text));
		// parent params
		PlayerPrefs.SetFloat(s+"C2ParPosX", float.Parse( CamParentTransformOptions[6].text));
		PlayerPrefs.SetFloat(s+"C2ParPosY", float.Parse( CamParentTransformOptions[7].text));
		PlayerPrefs.SetFloat(s+"C2ParPosZ", float.Parse( CamParentTransformOptions[8].text));
		PlayerPrefs.SetFloat(s+"C2ParRotX", float.Parse( CamParentTransformOptions[9].text));
		PlayerPrefs.SetFloat(s+"C2ParRotY", float.Parse( CamParentTransformOptions[10].text));
		PlayerPrefs.SetFloat(s+"C2ParRotZ", float.Parse( CamParentTransformOptions[11].text));
	}

	public void LOAD (int index) {
		string s = index.ToString ();
		// Gradients
		GradientSliders [0].value = PlayerPrefs.GetFloat(s+"Reach1");
		GradientSliders [1].value = PlayerPrefs.GetFloat(s+"Reach2");

		// Resolution
		ResolutionInputs[0].text = PlayerPrefs.GetInt(s+"P1W").ToString();
		ResolutionInputs[1].text = PlayerPrefs.GetInt(s+"P1H").ToString();
		ResolutionInputs[2].text = PlayerPrefs.GetInt(s+"P2W").ToString();
		ResolutionInputs[3].text = PlayerPrefs.GetInt(s+"P2H").ToString();
		ResolutionInputs[4].text = PlayerPrefs.GetInt(s+"ScrW").ToString();
		ResolutionInputs[5].text = PlayerPrefs.GetInt(s+"ScrH").ToString();

		// Camera1 Parameters
		CamFOV[0].text = PlayerPrefs.GetFloat(s+"C1FOV").ToString();
		CamUDAngles [0].value = PlayerPrefs.GetFloat (s+"C1X");
		CamLRAngles [0].value = PlayerPrefs.GetFloat (s+"C1Y");
		CamDistances [0].value = PlayerPrefs.GetFloat (s+"C1Z");
		// local params
		CamTransformOptions[0].text = PlayerPrefs.GetFloat(s+"C1LocPosX").ToString();
		CamTransformOptions[1].text = PlayerPrefs.GetFloat(s+"C1LocPosY").ToString();
		CamTransformOptions[2].text = PlayerPrefs.GetFloat(s+"C1LocPosZ").ToString();
		CamTransformOptions[3].text = PlayerPrefs.GetFloat(s+"C1LocRotX").ToString();
		CamTransformOptions[4].text = PlayerPrefs.GetFloat(s+"C1LocRotY").ToString();
		CamTransformOptions[5].text = PlayerPrefs.GetFloat(s+"C1LocRotZ").ToString();
		// parent params
		CamParentTransformOptions[0].text = PlayerPrefs.GetFloat(s+"C1ParPosX").ToString();
		CamParentTransformOptions[1].text = PlayerPrefs.GetFloat(s+"C1ParPosY").ToString();
		CamParentTransformOptions[2].text = PlayerPrefs.GetFloat(s+"C1ParPosZ").ToString();
		CamParentTransformOptions[3].text = PlayerPrefs.GetFloat(s+"C1ParRotX").ToString();
		CamParentTransformOptions[4].text = PlayerPrefs.GetFloat(s+"C1ParRotY").ToString();
		CamParentTransformOptions[5].text = PlayerPrefs.GetFloat(s+"C1ParRotZ").ToString();

		// Camera2 Parameters
		CamFOV[1].text = PlayerPrefs.GetFloat(s+"C2FOV").ToString();
		CamUDAngles [1].value = PlayerPrefs.GetFloat (s+"C2X");
		CamLRAngles [1].value = PlayerPrefs.GetFloat (s+"C2Y");
		CamDistances [1].value = PlayerPrefs.GetFloat (s+"C2Z");
		// local params
		CamTransformOptions[6].text = PlayerPrefs.GetFloat(s+"C2LocPosX").ToString();
		CamTransformOptions[7].text = PlayerPrefs.GetFloat(s+"C2LocPosY").ToString();
		CamTransformOptions[8].text = PlayerPrefs.GetFloat(s+"C2LocPosZ").ToString();
		CamTransformOptions[9].text = PlayerPrefs.GetFloat(s+"C2LocRotX").ToString();
		CamTransformOptions[10].text = PlayerPrefs.GetFloat(s+"C2LocRotY").ToString();
		CamTransformOptions[11].text = PlayerPrefs.GetFloat(s+"C2LocRotZ").ToString();
		// parent params
		CamParentTransformOptions[6].text = PlayerPrefs.GetFloat(s+"C2ParPosX").ToString();
		CamParentTransformOptions[7].text = PlayerPrefs.GetFloat(s+"C2ParPosY").ToString();
		CamParentTransformOptions[8].text = PlayerPrefs.GetFloat(s+"C2ParPosZ").ToString();
		CamParentTransformOptions[9].text = PlayerPrefs.GetFloat(s+"C2ParRotX").ToString();
		CamParentTransformOptions[10].text = PlayerPrefs.GetFloat(s+"C2ParRotY").ToString();
		CamParentTransformOptions[11].text = PlayerPrefs.GetFloat(s+"C2ParRotZ").ToString();



		UpdateEverything ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorPlanet : MonoBehaviour {

	public bool TestWithFixedColor = false;

	private bool canChange = true;

	public float timeForAFullTurn = 10f;
	float rotateAmount;
	public float timeForTransition = 1f;
	[Range(0,1)]
	public float ColorDistance = 0.5f;
	 float initialHueAngle = 60;
	 float randomValueToSV = 0.3f;
	 float gradientReach = 0.4f;
	 float randomisationValue = 0.1f;

	private HSVtoRGB HSVConverter;

	public GameObject[] prefabs_buildings;
	public GameObject[] prefabs_sideStuff;
	public Transform cylinder;

	public Material mainMat;
	public Material floorMat;
	public Material stuffMat;
	public Material floorStuffMat;
	public Material skyMat;


	public Image[] displaypalette;
	public GameObject parentPalette;


	public Material[] ProtoPlanetMats;

	void Start () {
		HSVConverter = GetComponent<HSVtoRGB> ();

		SetStartValues ();
	}


	void SetPlanetColorsProto() {
		Color[] cs = NewSet ();
		ProtoPlanetMats [0].color = cs [0];
		ProtoPlanetMats [1].color = Color.Lerp (cs [0], cs [1], 0.5f);
		ProtoPlanetMats [2].color = cs [1];
		ProtoPlanetMats [3].color = cs [2];
		ProtoPlanetMats [4].color = Color.Lerp (cs [2], cs [3], 0.5f);
		ProtoPlanetMats [5].color = cs [3];
		ProtoPlanetMats [6].color = cs [4];
		ProtoPlanetMats [7].color = Color.Lerp (cs [4], cs [5], 0.5f);
		ProtoPlanetMats [8].color = cs [5] * new Color (1,1,1, 0.5f);
	}



	void SetStartValues() {

		/*initialHueAngle = Mathf.Lerp (160, 20, ColorDistance);
		randomValueToSV = Mathf.Lerp (0.1f, 0.5f, ColorDistance);
		gradientReach = Mathf.Lerp(0.9f, 0.2f, ColorDistance);
		randomisationValue = Mathf.Lerp(0, 0.5f, ColorDistance);*/

		initialHueAngle = 30;
		randomValueToSV = 0.2f;
		gradientReach = 0.5f;
		randomisationValue = 0.1f;
	}

	void Update () {

		/*cylinder.Rotate (new Vector3 (0, rotateAmount, 0) * Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.Space)) {
			ColorDistance = 1;
		}

		if (Input.GetKey (KeyCode.Space)) {
			ColorDistance -= Time.deltaTime * 0.5f;
			if (ColorDistance < 0)
				ColorDistance = 0;
		}

		if (canChange && Input.GetKeyUp(KeyCode.Space)) {
			canChange = false;
			SetStartValues ();
			ChangeColorsCall ();
		}





		parentPalette.SetActive (Input.GetKey (KeyCode.P));*/



		if (Input.GetKeyDown (KeyCode.Space)) {
			SetPlanetColorsProto ();
		}
	}

	void NewSection (float angle, float lifetime){
		GameObject g = new GameObject();
		// generate one random building on each side

		for (int i = -2; i <= 2; i++) {
			if (i != 0) {
				GameObject t = Instantiate (prefabs_buildings [Random.Range (0, prefabs_buildings.Length)]) as GameObject;
				t.transform.position = new Vector3 ( ((i < 0 ) ? - 1 : 1) + (1.5f * i), 0, 0);
				t.transform.eulerAngles = new Vector3 (0, Random.Range (0, 4) * 90, 0);
				t.transform.parent = g.transform;

				if (Random.value < 0.2f) {
					t.transform.GetChild (0).GetComponent<Renderer> ().material = stuffMat;
				}
			}
		}
		// generate random props
		// every prop is compatible with other props, i may change it later
		// so let's say every prop has a 20% chance of appearing. 
		for (int i = -1; i <= 1; i++) {
			if (i != 0) {
				foreach (GameObject p in prefabs_sideStuff) {
					if (Random.value < 0.2f) {
						GameObject t = Instantiate (p) as GameObject;
						t.transform.position = new Vector3 ( ((i < 0 ) ? - 1 : 1) + (1.5f * i), 0, 0);
						t.transform.eulerAngles = new Vector3 (0, 90 * (i-1), 0);
						t.transform.parent = g.transform;

						if (Random.value < 0.3f) {
							t.transform.GetChild (0).GetComponent<Renderer> ().material = floorStuffMat;
						}
					}
				}
			}
		}

		// start a coroutine with the instantiated objects to destroy them in lifetime seconds, then calls a newsection

		g.transform.eulerAngles = new Vector3 (angle, 0, 0);
		g.transform.parent= cylinder;
		StartCoroutine(SectionLife(lifetime, g));
	}

	IEnumerator SectionLife(float lifetime, GameObject section){
		float t = lifetime;
		while (t > 0) {
			t -= Time.deltaTime;
			yield return null;
		}
		Destroy (section);
		NewSection (-180, timeForAFullTurn);
	}

	void FirstGen(){
		int sections = 20;
		for (int i = 0; i < sections; i++) {
			NewSection (180 + (sections-(i+1) * (360/sections)),
				(timeForAFullTurn/sections) * (sections-(i+1)));
		}
	}

	void ChangeColorsCall () {
		StartCoroutine (ChangeColors (timeForTransition));
	}

	IEnumerator ChangeColors(float time){
		Color[] newSet = NewSet ();
		Color[] oldSet = {
			stuffMat.GetColor ("_ColorFront"),
			stuffMat.GetColor ("_ColorBack"),
			mainMat.GetColor ("_ColorFront"),
			mainMat.GetColor ("_ColorBack"),
			skyMat.GetColor ("_ColorFront"),
			skyMat.GetColor ("_ColorBack")
		};

		Color[] lerpSet = oldSet;

		float t = 0;
		while (t < 1) {
			for (int i = 0; i < 6; i ++){
				lerpSet [i] = Color.Lerp (oldSet [i], newSet [i], t);
			}
			stuffMat.SetColor ("_ColorFront", lerpSet [0]);
			stuffMat.SetColor ("_ColorBack", lerpSet [1]);
			floorStuffMat.SetColor ("_ColorFront", newSet [0]);
			floorStuffMat.SetColor ("_ColorBack", newSet [1]);
			mainMat.SetColor ("_ColorFront", lerpSet [2]);
			mainMat.SetColor ("_ColorBack", lerpSet [3]);
			floorMat.SetColor ("_ColorFront", lerpSet [2]);
			floorMat.SetColor ("_ColorBack", lerpSet [3]);
			skyMat.SetColor ("_ColorFront", lerpSet [4]);
			skyMat.SetColor ("_ColorBack", lerpSet [5]);

			t += Time.deltaTime * time;
			yield return null;
		}

		stuffMat.SetColor ("_ColorFront", newSet [0]);
		stuffMat.SetColor ("_ColorBack", newSet [1]);
		floorStuffMat.SetColor ("_ColorFront", newSet [0]);
		floorStuffMat.SetColor ("_ColorBack", newSet [1]);
		mainMat.SetColor ("_ColorFront", newSet [2]);
		mainMat.SetColor ("_ColorBack", newSet [3]);
		floorMat.SetColor ("_ColorFront", newSet [2]);
		floorMat.SetColor ("_ColorBack", newSet [3]);
		skyMat.SetColor ("_ColorFront", newSet [4]);
		skyMat.SetColor ("_ColorBack", newSet [5]);

		canChange = true;
		yield return null;
	}

	Color[] NewSet(){
		float CenterColorH = Random.value * 360;
		float CenterColorS = Random.value;// 1 - (Mathf.Pow(Random.value, 2));
		float CenterColorV = 1 - (Mathf.Pow(Random.value, 2));

		if (TestWithFixedColor) {
			CenterColorH = 0;
			CenterColorS = 1;
			CenterColorV = 1;
		}

		float r = Random.Range (-1, 1) * 2 + 1; // -1 or 1

		float MainColorH = CenterColorH + 180 + (r * initialHueAngle);
		float MainColorS = Mathf.Lerp(CenterColorS, Random.value, randomValueToSV);
		float MainColorV = Mathf.Lerp(CenterColorV, Random.value, randomValueToSV);

		float SkyColorH = CenterColorH + 180 + (r * initialHueAngle * -1);
		float SkyColorS = Mathf.Lerp(CenterColorS, Random.value, randomValueToSV);
		float SkyColorV = Mathf.Lerp(CenterColorV, Random.value, randomValueToSV);

		// picking gradients
		float CenterColor2H = Mathf.Lerp (CenterColorH, MainColorH, gradientReach * 1.5f);
		float CenterColor2S = Mathf.Lerp (CenterColorS, MainColorS, gradientReach * 1.5f);
		float CenterColor2V = Mathf.Lerp (CenterColorV, MainColorV, gradientReach * 1.5f);

		float MainColor2H = Mathf.Lerp (MainColorH, CenterColorH, gradientReach);
		float MainColor2S = Mathf.Lerp (MainColorS, CenterColorS, gradientReach);
		float MainColor2V = Mathf.Lerp (MainColorV, CenterColorV, gradientReach);

		float SkyColor2H = Mathf.Lerp (SkyColorH, CenterColorH, gradientReach);
		float SkyColor2S = Mathf.Lerp (SkyColorS, CenterColorS, gradientReach);
		float SkyColor2V = Mathf.Lerp (SkyColorV, CenterColorV, gradientReach);

		// creating colors
		Color CenterColor = HSVConverter.Convert(CenterColorH, CenterColorS, CenterColorV);
		Color CenterColor2 = HSVConverter.Convert(CenterColor2H, CenterColor2S, CenterColor2V);

		Color MainColor = HSVConverter.Convert(MainColorH, MainColorS, MainColorV);
		Color MainColor2 = HSVConverter.Convert (MainColor2H, MainColor2S, MainColor2V);

		Color SkyColor = HSVConverter.Convert(SkyColorH, SkyColorS, SkyColorV);
		Color SkyColor2 = HSVConverter.Convert(SkyColor2H, SkyColor2S, SkyColor2V);

		// experiment with the order here ?
		Color[] cs = {CenterColor, CenterColor2, MainColor, MainColor2, SkyColor, SkyColor2};

		// Randomising them a bit for imperfections
		for (int i = 0; i < 6; i ++){
			cs [i] = Randomise (cs [i], randomisationValue);
		}

		//DisplayPalette (cs);

		return cs;
	}

	Color Randomise (Color input, float mixingValue){
		// mixingvalue should be the amoiunt of new color you want miwed into the old one
		Color output = Color.Lerp (input, Random.ColorHSV (), mixingValue);
		return output;
	}

	void DisplayPalette(Color[] cs){
		displaypalette [0].color = cs [0];
		displaypalette [1].color = Color.Lerp (cs [0], cs [1], 0.5f);
		displaypalette [2].color = cs [1];
		displaypalette [3].color = cs [2];
		displaypalette [4].color = Color.Lerp (cs [2], cs [3], 0.5f);
		displaypalette [5].color = cs [3];
		displaypalette [6].color = cs [4];
		displaypalette [7].color = Color.Lerp (cs [4], cs [5], 0.5f);
		displaypalette [8].color = cs [5];

	}
}

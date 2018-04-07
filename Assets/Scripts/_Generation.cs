using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Generation : MonoBehaviour {

	public ParticleSystem[] destruction;

	private int step = 0;
	private float moonScale = 10;
	int ringType = 0;

	public GameObject[] spherePrefabs;
	private int scale = 0;
	private GameObject spherePrefabToUse;
	private GameObject spherePlacement;

	private GameObject g_parent;
	public GameObject g_spark;
	public GameObject g_dust;
	private GameObject g_clouds;

	public Transform[] planetBits;
	public GameObject g_pBits;
	public Transform[] ringBits;
	public GameObject g_rBits;
	public GameObject g_flatRing;
	private Vector3 targetFlatRingRot;
	public GameObject g_particleRing;
	private List<Transform> moonsPlaceholders = new List<Transform>();
	private GameObject g_moons;

	private GameObject g_water;
	private GameObject g_land;
	private GameObject g_trees;
	private GameObject g_urban;

	private List<Transform> treesBits = new List<Transform>();
	private List<Transform> urbanBits = new List<Transform>();
	private List<Vector3> treesScl = new List<Vector3> ();
	private List<Vector3> urbanScl = new List<Vector3> ();

	private float noiseXOffset = 0;
	private float noiseYOffset = 0;
	private float noiseZOffset = 0;

	public GameObject[] moons_prefabs;
	public GameObject[] nature_prefabs;
	public GameObject[] urban_prefabs;

	public Material LandMat;
	public Material SeaMat;
	public Material CloudMat;

	public Material FlatRingMat;
	public Material Life1M;
	public Material Life2M;
	public Material Life3M;
	public Material[] MoonMatSet1;
	public Material[] MoonMatSet2;
	public Material[] MoonMatSet3;

	private HSVtoRGB HSVConverter;

	private float initialHueAngle = 80;
	private float randomValueToSV = 0.3f;
	private float gradientReach = 0.3f;
	private float randomisationValue = 0.1f;


	private bool candestroy = true;

	public AudioClip[] musics;

	void Start () {
		HSVConverter = GetComponent<HSVtoRGB> ();
		musics = ShuffleACList (musics);
		Setup ();
	}

	void Setup() {
		g_parent = new GameObject ("Planet");

		moons_prefabs = ShuffleGOList (moons_prefabs);
		nature_prefabs = ShuffleGOList (nature_prefabs);
		urban_prefabs = ShuffleGOList (urban_prefabs);

		spherePrefabToUse = spherePrefabs [scale];
		spherePlacement = spherePrefabs [scale + 2];

		ringType = Random.value < 0.5f ? 0 : Random.value < 0.5f ? 1 : Random.value < 0.5f ? 2 : 3;

		// generate water, clouds, land, and trees/urban here
		SetNoiseParameters ();
		GenerateLand();
		GenerateWater();
		SetNoiseParameters ();
		GenerateClouds();
		SetNoiseParameters ();
		GenerateTreesAndUrban();
		GenerateMoons ();

		// assign colors to materials and to particles
		Color[] colors= NewSet();

		Color Life1 = colors [0];
		Color Life2 = colors [1];
		Color Life3 = colors [2];

		Color Land1 = colors [3];
		Color Land2 = colors [4];
		Color Land3 = colors [5];

		Color Water1 = colors [6];
		Color Water2 = colors [7];
		Color Water3 = colors [8];

		LandMat.SetColor ("_Color", Land1);
		LandMat.SetColor ("_Color1", Land2);
		LandMat.SetColor ("_Color2", Land3);

		SeaMat.SetColor ("_Color", Water3);
		SeaMat.SetColor ("_Color1", Water2);
		SeaMat.SetColor ("_Color2", Water1);

		CloudMat.SetColor ("_Color", Water1);

		Color[] rc = { Land1, Land3, Water1, Water2, Water3, Life1, Life2, Life3 };
		Color r = rc [Random.Range (0, 8)];
		FlatRingMat.SetColor ("_Color", r);
		r = rc [Random.Range (0, 8)];
		FlatRingMat.SetColor ("_Color1", r);
		r = rc [Random.Range (0, 8)];
		FlatRingMat.SetColor ("_Color2", r);

		Life1M.color = Life1;
		Life2M.color = Life2;
		Life3M.color = Life3;
		MoonMatSet3 [0].color = Land1;
		MoonMatSet3 [1].color = Land3;
		MoonMatSet3 [5].color = Water1;
		MoonMatSet3 [6].color = Water2;
		MoonMatSet3 [7].color = Water3;
		//MoonMatSet3 [2].color = Life1;
		//MoonMatSet3 [3].color = Life2;
		MoonMatSet3 [4].color = Life3;

		rc = new Color[] { Land1, Land3, Life1, Life2, Life3 };
		ParticleSystem.MainModule m = g_spark.GetComponent<ParticleSystem> ().main;
		m.startColor = rc [Random.Range (2, 5)];
		m = g_dust.GetComponent<ParticleSystem> ().main;
		m.startColor = rc[Random.Range(0,3)];
		m = g_particleRing.GetComponent<ParticleSystem> ().main;
		m.startColor = rc[Random.Range(0,5)];

		//set every object to it's starting scale / coordinates
		foreach (GameObject g in new GameObject[]{g_land, g_water}){
			g.transform.localScale = Vector3.zero;
		}
		foreach (Transform g in planetBits){
			g.localScale = Vector3.zero;
		}
		foreach (Transform g in ringBits){
			g.localScale = Vector3.zero;
		}
		foreach (Transform g in moonsPlaceholders){
			g.localScale = Vector3.zero;
		}
		foreach (Transform g in treesBits){
			g.localScale *= 0.0001f;
		}
		foreach (Transform g in urbanBits){
			g.localScale *= 0.0001f;
		}
		RandomizeRingBits ();
		RandomizePlanetBits ();
		Color c = CloudMat.color;
		c.a = 0;
		CloudMat.color = c;
		if (ringType == 3) {
			g_particleRing.transform.Rotate (Vector3.right * Random.Range (-30, 30));
			_WorldRotate wrp = g_particleRing.AddComponent<_WorldRotate> ();
			wrp.r = Vector3.up * 4 * (Random.value - 0.5f) + Vector3.right * -1.5f;
		}
		if (ringType == 2) {
			targetFlatRingRot = (Vector3.forward * Random.Range (-30, 30)) + (Vector3.right * Random.Range (-5, 5));
			g_flatRing.transform.localScale = Vector3.one * 5;
			g_flatRing.transform.eulerAngles = Vector3.right * -90;
			_WorldRotate wrp = g_flatRing.AddComponent<_WorldRotate> ();
			wrp.r = Vector3.up * 4 * (Random.value - 0.5f) + Vector3.right * -1.5f;
			wrp.noInitRotate = true;
		} else {
			g_flatRing.transform.localScale = Vector3.zero;
		}

		// parenting everything to parent
		foreach (GameObject g in new GameObject[]{g_pBits, g_rBits, g_flatRing}){
			g.transform.parent = g_parent.transform;
		}

		// go
		step = -1;
		NextStep ();
	}

	void GenerateLand () {
		// setting up the gameobject
		g_land = Instantiate (spherePrefabToUse) as GameObject;
		g_land.name = "Land";
		g_land.transform.parent = g_parent.transform;
		g_land.transform.localScale = Vector3.one;
		g_land.transform.eulerAngles = Vector3.zero;

		// rotation
		_WorldRotate wr =  g_land.AddComponent <_WorldRotate> ();
		wr.r = Vector3.up * 2f;

		// setting up mesh
		Mesh m = g_land.GetComponent<MeshFilter> ().mesh;
		int points = m.vertexCount;

		// setting up vertices
		List<Vector3> vs = new List<Vector3>();
		foreach (Vector3 v in m.vertices) {
			vs.Add(v * 100); // multiplying by 100 for scale
		}
		//noise offset
		for (int i = 0; i < points; i++) {
			vs[i] += Perlin3D(vs[i])*0.15f;
		}
		// final vertices set
		m.SetVertices (vs);
		m.RecalculateNormals ();

		// setting material
		g_land.GetComponent<Renderer> ().material = LandMat;
	}

	void GenerateWater () {
		// setting up the gameobject
		g_water = Instantiate (spherePrefabToUse) as GameObject;
		g_water.name = "Water";
		g_water.transform.parent = g_parent.transform;
		g_water.transform.localScale = Vector3.one;
		g_water.transform.rotation = Random.rotation;

		// rotation
		_WorldRotate wr =  g_water.AddComponent <_WorldRotate> ();
		wr.r = Vector3.up * 1.5f + Vector3.right * 0.5f;

		// setting up mesh
		Mesh m = g_water.GetComponent<MeshFilter> ().mesh;
		int points = m.vertexCount;

		// setting up vertices
		List<Vector3> vs = new List<Vector3>();
		foreach (Vector3 v in m.vertices) {
			vs.Add(v * 100); // multiplying by 100 for scale
		}
		//noise offset
		for (int i = 0; i < points; i++) {
			vs[i] += Perlin3D(vs[i])*0.15f;
			vs[i] = Vector3.Normalize(vs[i]) * 1.0f;
		}
		// final vertices set
		m.SetVertices (vs);
		m.RecalculateNormals ();

		// setting material
		g_water.GetComponent<Renderer> ().material = SeaMat;
	}

	void GenerateClouds() {
		// setting up the gameobject
		//g_clouds = Instantiate (spherePrefabToUse) as GameObject;
		g_clouds = Instantiate (spherePrefabs[scale+1]) as GameObject;
		g_clouds.name = "Clouds";
		g_clouds.transform.parent = g_parent.transform;
		g_clouds.transform.localScale = Vector3.one;
		g_clouds.transform.rotation = Random.rotation;

		// rotation
		_WorldRotate wr =  g_clouds.AddComponent <_WorldRotate> ();
		wr.r = Vector3.up * -3f + Vector3.right * -2f;

		// setting up mesh
		Mesh m = g_clouds.GetComponent<MeshFilter> ().mesh;
		int points = m.vertexCount;
		int tris = m.triangles.Length / 3;
		int[] triarray = m.triangles;

		// setting up vertices
		List<Vector3> vs = new List<Vector3>();
		float cloudSize = 1.3f - 0.1f * scale;
		foreach (Vector3 v in m.vertices) {
			vs.Add(v * 100 * cloudSize); // multiplying by 100 for scale
		}
		// add them a second time for two sided
		foreach (Vector3 v in m.vertices) {
			vs.Add(v * 100 * cloudSize); 
		}

		// setting up triangles. going through noise to get cloud density
		List<int> ts = new List<int> ();
		float cloudsRatio = 0.25f;
		for (int i = 0; i < tris; i++) {
			Vector3 refPoint = vs [triarray [i * 3]];
			refPoint = Vector3.Scale(refPoint, new Vector3 (1, 2, 1));
			refPoint = (Perlin3D (refPoint) + Vector3.one) * 0.5f;
			if (refPoint.y < cloudsRatio) {
				ts.Add (triarray [i * 3]);
				ts.Add (triarray [i * 3]+1);
				ts.Add (triarray [i * 3]+2);
				ts.Add (triarray [i * 3] + points);
				ts.Add (triarray [i * 3]+2 + points);
				ts.Add (triarray [i * 3]+1 + points);
			}
		}

		// final vertices set
		m.SetVertices (vs);
		m.SetTriangles (ts, 0);
		m.RecalculateNormals ();

		// setting material
		g_clouds.GetComponent<Renderer> ().material = CloudMat;
	}

	void GenerateTreesAndUrban() {
		// prepping parents
		g_trees = new GameObject("Trees");
		g_urban = new GameObject("Urban");
		g_trees.transform.parent = g_land.transform;
		g_urban.transform.parent = g_land.transform;

		// setting up temporary collider for raycasting
		MeshCollider MCL = g_land.AddComponent<MeshCollider> ();

		// foreach point in the placement sphere, 
		// first check if RNGesus says it's ok for there to be a prop
		// then raycast to pinpoint its position

		float invScale = 1 / Mathf.Pow(2, scale);
		float treeDensity = Random.Range (0.25f, 0.5f);// 0.4f;
		float urbanDensity = Random.Range (0.25f, 0.5f);// 0.4f;

		foreach (Vector3 vv in spherePlacement.GetComponent<MeshFilter>().sharedMesh.vertices) {
			Vector3 v = vv* 100;
			float noise = (Perlin3D (v * 0.00269f).x + 1) * 0.5f;
			bool tree = noise < treeDensity;
			bool urban = (1 - noise) < urbanDensity;
			if (tree || urban) {
				RaycastHit hit;
				if (Physics.Raycast(v*2, -v, out hit, 1.03f)){
					GameObject ins = null;
					if (tree) {
						ins = Instantiate (nature_prefabs [Random.Range (0, 2)]);
						ins.transform.parent = g_trees.transform;
						treesBits.Add (ins.transform);
						Vector3 scl = Vector3.one * 2f * (invScale) * Random.Range (0.85f, 1.15f);
						ins.transform.localScale = scl;
						treesScl.Add (scl);
					} else if (urban) {
						ins = Instantiate (urban_prefabs [Random.Range (0, 2)]);
						ins.transform.parent = g_urban.transform;
						urbanBits.Add (ins.transform);
						Vector3 scl = Vector3.one * 2f * (invScale) * Random.Range (0.85f, 1.15f);
						ins.transform.localScale = scl;
						urbanScl.Add (scl);
					}
					if (ins != null) {
						ins.transform.position = hit.point;
						ins.transform.LookAt(v*10);
						ins.transform.Rotate (new Vector3 (0, 0, Random.value * 360));
					}
				}
			}
		}

		// deleting meshcollider
		Destroy (MCL);
	}

	void RandomizePlanetBits(){
		g_pBits.transform.rotation = Random.rotation;
		_WorldRotate wr = g_pBits.AddComponent<_WorldRotate> ();
		wr.r = Vector3.up * -1.2f + Vector3.right * 0.5f;
		foreach (Transform b in planetBits) {
			b.localScale = Vector3.zero;
			b.rotation = Random.rotation;
			b.position *= 2;
			_WorldRotate wr2 = b.gameObject.AddComponent<_WorldRotate> ();
			wr2.r = Random.rotation.eulerAngles * 0.01f;
			b.gameObject.GetComponent<Renderer> ().material = MoonMatSet1 [Random.Range (0, MoonMatSet1.Length)];
		}
	}

	void RandomizeRingBits(){
		g_rBits.transform.eulerAngles = Vector3.right * Random.value * 30f;
		_WorldRotate wr = g_rBits.AddComponent<_WorldRotate> ();
		wr.r = Vector3.up * 1.2f + Vector3.right * -0.5f;
		foreach (Transform b in ringBits) {
			b.localScale = Vector3.zero;
			b.rotation = Random.rotation;
			b.position += b.forward * (Random.value * 2 - 1) * 2;
			_WorldRotate wr2 = b.gameObject.AddComponent<_WorldRotate> ();
			wr2.r = Random.rotation.eulerAngles * 0.05f;
			b.gameObject.GetComponent<Renderer> ().material = MoonMatSet1 [Random.Range (0, MoonMatSet1.Length)];
		}
	}

	void GenerateMoons(){
		g_moons = new GameObject ("Moons");
		g_moons.transform.parent = g_parent.transform;
		int moonsAmt = Random.Range (0, 7);
		moonScale = moonsAmt <= 2 ? 0.2f : moonsAmt <= 4 ? 0.1f : 0.05f;
		Material[] ms = moonsAmt <= 2 ? MoonMatSet1 : moonsAmt <= 4 ? MoonMatSet2 : MoonMatSet3;
		for (int i = 0; i < moonsAmt; i++) {
			GameObject mp = new GameObject ("moonRotator");
			mp.transform.parent = g_moons.transform;
			_WorldRotate wr = mp.AddComponent<_WorldRotate> ();
			wr.r = Vector3.up * (Random.value - 0.5f) * 20 + Vector3.right * (Random.value - 0.5f) * 2;
			GameObject m = Instantiate (moons_prefabs [Random.Range (0, moons_prefabs.Length)]);
			m.transform.localScale = Vector3.one * moonScale * 100;
			m.transform.parent = mp.transform;
			m.transform.rotation = Random.rotation;
			m.transform.Translate (Vector3.forward * Random.Range (1.5f, 2f));
			m.transform.rotation = Random.rotation;
			m.GetComponent<Renderer> ().material = ms [Random.Range (0, ms.Length)];
			wr = m.AddComponent<_WorldRotate> ();
			wr.r = Vector3.up * (Random.value - 0.5f) * 20 + Vector3.right * (Random.value - 0.5f) * 2;

			moonsPlaceholders.Add (m.transform);
		}
	}

	GameObject[] ShuffleGOList(GameObject[] list){
		for (int i = 0; i < list.Length; i++) {
			int r = Random.Range (0, list.Length);
			GameObject temp = list [i];
			list [i] = list [r];
			list[r] = temp;
		}
		return list;
	}
	AudioClip[] ShuffleACList(AudioClip[] list){
		for (int i = 0; i < list.Length; i++) {
			int r = Random.Range (0, list.Length);
			AudioClip temp = list [i];
			list [i] = list [r];
			list[r] = temp;
		}
		return list;
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && candestroy) {
			StopAllCoroutines ();
			candestroy = false;
			//PrevStep ();
			StartCoroutine (Reset(1f));
		}
	}

	void NewAudio(AudioClip ac) {
		AudioSource a = gameObject.AddComponent<AudioSource> ();
		a.clip = ac;
		a.loop = true;
		a.volume = 0;
		a.Play ();
	}

	IEnumerator audioup (AudioSource a, float time) {
		float 	t = time;
		while (t > 0) {
			t -= Time.deltaTime;
			float lerp = 1 - (t / time);
			a.volume = lerp;
			yield return null;
		}
	}

	/*void PrevStep(){
		if (step >= 4) {
			step--;
		} else {
			step = 0;
			// also actually reboot scene instead of =0
			// probably just start a coroutine of parent GO to shrink and reset
			// also stop both particlesystems just in case
		}
		switch (step) {
		case 3:
			step++;
			Appear (g_water, false, 0.5f);
			Appear (g_pBits, 0.5f, 0.2f);
			break;
		case 5 : 
			Appear (g_land, false, 0.5f);
			break;
		case 6 : 
			Appear (g_trees, false, 0.5f);
			break;
		case 7 : 
			Appear (g_urban, false, 0.5f);
			break;
		}
		step--;
	}*/

	IEnumerator Reset(float time) {

		g_spark.GetComponent<ParticleSystem>().Stop();
		g_dust.GetComponent<ParticleSystem>().Stop();
		g_particleRing.GetComponent<ParticleSystem>().Stop();

		foreach (ParticleSystem p in destruction) {
			p.Play ();
		}



		//yield return new WaitForSeconds (3);
		float t = 3;
		while (t > 0) {
			t -= Time.deltaTime;
			float lerp = 1 - (t / time);
			foreach (AudioSource a in GetComponents<AudioSource>()) {
				a.volume = 1 - lerp;
			}
			yield return null;
		}
		yield return new WaitForSeconds (0);


		t = time;
		while (t > 0) {
			t -= Time.deltaTime;
			float lerp = 1 - (t / time);
			g_parent.transform.localScale =Vector3.one * ( 1 - lerp);
			yield return null;
		}
		g_parent.transform.localScale = Vector3.zero;
		yield return new WaitForSeconds (2);
		UnityEngine.SceneManagement.SceneManager.LoadScene (1);
	}

	void NextStep(){
		StopAllCoroutines ();
		if (step < 8) step++;
		switch (step) {
		case 0 :
			Appear (g_spark, 0.001f, 1f);
			break;
		case 1: 
			NewAudio (musics [0]);
			NewAudio (musics [1]);
			NewAudio (musics [2]);
			StartCoroutine (audioup(GetComponents<AudioSource>()[0], 1));
			Appear (g_spark, false, 0.001f, 2f);
			Appear (g_dust, 0.001f);
			break;
		case 2 : 
			Appear (g_pBits, 4f, 2f);
			Appear (g_rBits, 5f);
			Appear (g_moons, 5f);
			break;
		case 3 : 
			Appear (g_pBits, true, 7f, 1f, true);
			Appear (g_rBits, true, 7f, 1000f, true);
			Appear (g_dust, false,  0.001f);
			break;
		case 4 : 
			StartCoroutine (audioup(GetComponents<AudioSource>()[1], 1));
			Appear (g_water, 3f, 1f);
			break;
		case 5: 
			Appear (g_pBits, false, 5f, 1f);
			Appear (g_clouds, 5f);
			break;
		case 6 : 
			Appear (g_land, 4f, 2f);
			break;
		case 7 : 
			StartCoroutine (audioup(GetComponents<AudioSource>()[2], 1));
			Appear (g_trees, 5f, 2f);
			break;
		case 8 : 
			Appear (g_urban, 5f, 2f);
			break;
		}
	}

	void Appear (GameObject part, float time){
		Appear (part, time, 1000f);
	}

	void Appear (GameObject part, float time, float pauseAfter){
		Appear (part, true, time, pauseAfter);
	}

	void Appear (GameObject part, bool isAppear, float time){
		Appear (part, isAppear, time, 1000f);
	}

	void Appear (GameObject part, bool isAppear, float time, float pauseAfter){
		Appear (part, isAppear, time, pauseAfter, false);
	}

	void Appear (GameObject part, bool isAppear, float time, float pauseAfter, bool mergeBits) {
		StartCoroutine (Apparition (part, isAppear, time, pauseAfter, mergeBits));
	}

	IEnumerator Apparition (GameObject part, bool isAppear, float time, float pauseAfter, bool mergeBits) {

		float t = time;
		float r = Random.value;


		while (t > 0) {
			t -= Time.deltaTime;
			float lerp = (t / time);
			if (isAppear) lerp = 1 - lerp;

			switch (part.name) {
			case "Spark" :
			case "Dust" :
				if (isAppear)
					part.GetComponent<ParticleSystem>().Play();
				else
					part.GetComponent<ParticleSystem>().Stop();
				break;

			case "Clouds":
				Color c = CloudMat.color;
				c.a = lerp * 0.5f;
				CloudMat.color = c;
				break;

			case "PlanetBits":
				if (!mergeBits) {
					foreach (Transform b in planetBits) {
						b.localScale = Vector3.one * 100 * lerp;
					}
				} else {
					foreach (Transform b in planetBits) {
						b.localPosition = Vector3.Lerp (b.localPosition, Vector3.zero, lerp);
						b.localRotation = Quaternion.Slerp (b.localRotation, Quaternion.Euler (-90,0,0), lerp);
					}
				}
				break;

			case "RingBits":
				if (ringType == 1) {
					if (!mergeBits) {
						foreach (Transform b in ringBits) {
							b.localScale = Vector3.one * 100 * lerp;
						}
					} else {
						foreach (Transform b in ringBits) {
							b.localPosition = Vector3.Lerp (b.localPosition, Vector3.zero, lerp);
						}
					}
				}
				if (ringType == 2) {
					if (mergeBits) {
						lerp *= lerp;
						g_flatRing.transform.localScale = Vector3.Lerp(g_flatRing.transform.localScale, Vector3.one, lerp);
						g_flatRing.transform.rotation = Quaternion.Slerp (g_flatRing.transform.rotation, Quaternion.Euler (targetFlatRingRot), lerp);
					}
				}
				if (ringType == 3) {
					if (!mergeBits) {
						if (isAppear)
							g_particleRing.GetComponent<ParticleSystem>().Play();
						else
							g_particleRing.GetComponent<ParticleSystem>().Stop();
					}
				}
				break;

			case "Moons":
				foreach (Transform b in moonsPlaceholders) {
					b.localScale = Vector3.one * moonScale * 100 * lerp;
				}
				break;

			case "Trees":
				for (int i = 0; i < treesBits.Count; i++) {
					float mylerp = Mathf.Clamp01( (lerp * (treesBits.Count + 4)  - i) * 0.25f);
					treesBits [i].localScale = treesScl [i] * mylerp;
				}
				break;

			case "Urban":
				for (int i = 0; i < urbanBits.Count; i++) {
					float mylerp = Mathf.Clamp01( (lerp * (urbanBits.Count + 4)  - i) * 0.25f);
					urbanBits [i].localScale = urbanScl [i] * mylerp;
				}
				break;

			default :
				part.transform.localScale = Vector3.one * lerp;
				break;
			}


			yield return null;
		}
		part.transform.localScale = Vector3.one;

		yield return new WaitForSeconds (pauseAfter);
		if (step < 8) NextStep ();
	}

	void SetNoiseParameters() {
		noiseXOffset = Random.Range (-10000, 10000);
		noiseYOffset = Random.Range (-10000, 10000);
		noiseZOffset = Random.Range (-10000, 10000);
	}

	Vector3 Perlin3D(Vector3 input){
		return Perlin3D (input, 1);
	}

	Vector3 Perlin3D (Vector3 input, float offsetOffset) {
		float x = input.x * 600 * (1+ scale) + noiseXOffset * offsetOffset;
		float y = input.y * 600 * (1+ scale) + noiseYOffset * offsetOffset;
		float z = input.z * 600 * (1+ scale) + noiseZOffset * offsetOffset;

		Vector3 output = new Vector3 (
			Mathf.PerlinNoise (y, z),
			Mathf.PerlinNoise (z, x),
			Mathf.PerlinNoise (x, y)
		);

		output *= 2;
		output -= Vector3.one;

		return (output);
	}





	Color[] NewSet(){
		float CenterColorH = Random.value * 360;
		float CenterColorS =  1 - (Mathf.Pow(Random.value, 2));
		float CenterColorV = 1 - (Mathf.Pow(Random.value, 2));


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
		Color CenterColor1 = HSVConverter.Convert(mid(CenterColor2H, CenterColorH), mid(CenterColor2S, CenterColorS), mid(CenterColor2V, CenterColorV));
		Color CenterColor2 = HSVConverter.Convert(CenterColor2H, CenterColor2S, CenterColor2V);

		Color MainColor = HSVConverter.Convert(MainColorH, MainColorS, MainColorV);
		Color MainColor1 = HSVConverter.Convert (mid(MainColor2H, MainColorH), mid(MainColor2S, MainColorS), mid(MainColor2V, MainColorV));
		Color MainColor2 = HSVConverter.Convert (MainColor2H, MainColor2S, MainColor2V);

		Color SkyColor = HSVConverter.Convert(SkyColorH, SkyColorS, SkyColorV);
		Color SkyColor1 = HSVConverter.Convert(mid(SkyColor2H, SkyColorH), mid(SkyColor2S, SkyColorS), mid(SkyColor2V, SkyColorV));
		Color SkyColor2 = HSVConverter.Convert(SkyColor2H, SkyColor2S, SkyColor2V);

		// experiment with the order here ?
		Color[] cs = {CenterColor, CenterColor1, CenterColor2, MainColor, MainColor1, MainColor2, SkyColor, SkyColor1, SkyColor2};

		// Randomising them a bit for imperfections
		for (int i = 0; i < 6; i ++){
			cs [i] = Randomise (cs [i], randomisationValue);
		}

		//DisplayPalette (cs);

		return cs;
	}

	float mid (float f1, float f2) {
		return ((f1 + f2) * 0.5f);
	}

	Color Randomise (Color input, float mixingValue){
		// mixingvalue should be the amoiunt of new color you want miwed into the old one
		Color output = Color.Lerp (input, Random.ColorHSV (), mixingValue);
		return output;
	}

}

using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetGen : MonoBehaviour {
	public GameObject[] spherePrefabs;
	private GameObject spherePrefabToUse;
	private GameObject spherePlacement;
	
	private GameObject g;
	private GameObject g_land;
	private GameObject g_water;
	//private GameObject g_clouds;
	private GameObject g_trees;
	private GameObject g_urban;

	public Material[] matList;

	private float noiseXOffset = 0;
	private float noiseYOffset = 0;
	private float noiseZOffset = 0;

	public GameObject[] NatureProps;
	public GameObject[] UrbanProps;
	public GameObject[] MountainProps;

	private int scale = 0;
	// Use this for initialization
	void Start () {
		Gen ();
		StartCoroutine (ZeroToOneScale ());
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			StartCoroutine (OneToZeroScale ());
		}
	}

	IEnumerator ZeroToOneScale(){
		g.transform.localScale = Vector3.zero;
		g_land.transform.localScale = Vector3.zero;
		g_water.transform.localScale = Vector3.one;
		//g_clouds.transform.localScale = Vector3.zero;
		g_trees.transform.localScale = Vector3.zero;
		g_urban.transform.localScale = Vector3.zero;
		float t = 1;
		while (t > 0) {
			t -= Time.deltaTime;
			g.transform.localScale = Vector3.one * (1-t);
			yield return null;
		}

		t = 1;
		while (t > 0.01f) {
			t -= Time.deltaTime;
			g_land.transform.localScale = Vector3.one * (1.01f-t) * 100;
			yield return null;
		}
		g_land.transform.localScale = Vector3.one * 100;

		t = 1;
		while (t > 0) {
			t -= Time.deltaTime;
			g_trees.transform.localScale = Vector3.one * (1-t) * 0.01f;
			yield return null;
		}
		g_trees.transform.localScale = Vector3.one * 0.01f;

		t = 1;
		while (t > 0) {
			t -= Time.deltaTime;
			g_urban.transform.localScale = Vector3.one * (1-t) * 0.01f;
			yield return null;
		}
		g_urban.transform.localScale = Vector3.one * 0.01f;
	}

	IEnumerator OneToZeroScale(){
		float t = 1;
		while (t > 0) {
			t -= Time.deltaTime;
			g.transform.localScale = Vector3.one * t;
			yield return null;
		}
		g.transform.localScale = Vector3.zero;
		yield return null;
		SceneManager.LoadScene(0);
	}

	void Gen() {

		scale = 1;//Random.Range (0, spherePrefabs.Length - 2);
		spherePrefabToUse = spherePrefabs [scale];
		spherePlacement = spherePrefabs [scale + 2];

		SetNoiseParameters ();
		if (g != null) {
			Destroy (g);
		}
		g = new GameObject ("Planet");
		
		g_urban = new GameObject ("Urban");
		g_trees = new GameObject ("Trees");
		g_water = new GameObject ("Water");
		g_land = Instantiate (spherePrefabToUse) as GameObject;
		g_land.name = "Land";
		
		g_urban.transform.parent = g_land.transform;
		g_trees.transform.parent = g_land.transform;
		g_water.transform.parent = g.transform;
		g_land.transform.parent = g.transform;

		Mesh m = g_land.GetComponent<MeshFilter> ().mesh;
		m.subMeshCount = matList.Length;

		// get vertices in list
		int points = m.vertexCount;
		int tris = m.triangles.Length / 3;

		// adding vertices to list
		List<Vector3> vs = new List<Vector3>();
		foreach (Vector3 v in m.vertices) {
			vs.Add(v);
		}
		
		// noise offset
		for (int i = 0; i < points; i++) {
			vs[i] += Perlin3D(vs[i])*0.0015f;
		}

		// duplicating points for hard edges
		for (int i = 0; i < points; i ++) {
			vs.Add(vs[i]);
		}
		for (int i = 0; i < points; i ++) {
			vs.Add(vs[i]);
		}

		// add centerpoint enough tomes for each face
	/*	for (int i = 0; i < tris*3; i ++) {
			vs.Add (Vector3.zero);
		}
*/
		// get triangles in lists
		List<int> ts = new List<int> ();
		foreach (int t in m.triangles) {
			ts.Add (t);
		}

		// add triangles to centerpoint
		// done in a second list for submeshes
/*		List<int> ts1 = new List<int> ();
		for(int i = 0; i < tris; i ++) {
			ts1.Add (ts[3 * i + 1]  + points); 
			ts1.Add (ts[3 * i + 0] + points); 
			ts1.Add (points*3 + (i*3));
			
			ts1.Add (ts[3 * i + 0] + points*2); 
			ts1.Add (ts[3 * i + 2] + points); 
			ts1.Add (points*3 + (i*3)+1);
			
			ts1.Add (ts[3 * i + 2] + points * 2); 
			ts1.Add (ts[3 * i + 1] + points * 2); 
			ts1.Add (points*3 + (i*3)+2);
		}*/
		// a cleaner way of proceeding might be to ahve a single centerpoint, 
		// or even single points for everything, and to duplicate them when using them for triangles.


		// adding water
		int index = vs.Count;
		for (int i = 0; i < points; i ++) {
			vs.Add(vs[i].normalized * 0.01f * 100);
		}
		List<int> ts2 = new List<int>();
		for (int i = 0; i < tris*3; i++) {
			ts2.Add(ts[i] + index);
		}

		// adding clouds
		index = vs.Count;
		float cloudSize = 0.014f - 0.001f * scale;
		for (int i = 0; i < points; i ++) {
			vs.Add(vs[i].normalized * cloudSize * 100);
		}
		for (int i = 0; i < points; i ++) {
			vs.Add(vs[i].normalized * cloudSize * 100);
		}
		List<int> ts3 = new List<int>();

		float cloudsRatio = Random.Range (0.2f, 0.3f);
		//SetNoiseParameters ();
		for (int i = 0; i<tris; i ++) {
			Vector3 vN = (Perlin3D (vs [ts[i*3] + index], 2) + Vector3.one) * 0.5f;
			float moy = vN.y;
			if (moy < cloudsRatio){
				ts3.Add (ts[i * 3] + index);
				ts3.Add (ts[i * 3 + 1] + index);
				ts3.Add (ts[i * 3 + 2] + index);
				ts3.Add (ts[i * 3] + index + points);
				ts3.Add (ts[i * 3 + 2] + index + points);
				ts3.Add (ts[i * 3 + 1] + index + points);
			}
		}
		
		m.SetVertices (vs);
		m.SetTriangles (ts, 0);  
		m.SetTriangles(new int[3]{0,0,0},1);
		m.SetTriangles(new int[3]{0,0,0},2);
		m.SetTriangles(new int[3]{0,0,0},3);
		// 0 here is for submesh, assign different lists of each submesh. 
		// you'll need different submeshes for different materials
		//m.SetTriangles (ts1, 1);
		
		m.RecalculateNormals ();

		// water GO
		GameObject g2 = new GameObject ("Sea");
		g2.transform.parent = g_water.transform;
		g2.transform.localEulerAngles = Vector3.zero;
		MeshFilter mf2 = g2.AddComponent<MeshFilter> ();
		Mesh m2 = new Mesh ();
		m2.SetVertices (vs);
		m2.subMeshCount = matList.Length;
		m2.SetTriangles(ts2, 2);
		m2.SetTriangles(new int[3]{0,0,0},0);
		m2.SetTriangles(new int[3]{0,0,0},1);
		m2.SetTriangles(new int[3]{0,0,0},3);
		m2.RecalculateNormals ();
		mf2.mesh = m2;
		MeshRenderer mr2 = g2.AddComponent<MeshRenderer> ();
		mr2.materials = matList;
		SelfRotate sr2 = g2.AddComponent<SelfRotate> ();
		sr2.r = Vector3.up * 2f + Vector3.right * 1f;

		// clouds GO
		GameObject g3 = new GameObject ("Clouds");
		g3.transform.parent = g.transform;
		MeshFilter mf3 = g3.AddComponent<MeshFilter> ();
		Mesh m3 = new Mesh ();
		m3.SetVertices (vs);
		m3.subMeshCount = matList.Length;
		m3.SetTriangles(ts3, 3);
		m3.SetTriangles(new int[3]{0,0,0},0);
		m3.SetTriangles(new int[3]{0,0,0},1);
		m3.SetTriangles(new int[3]{0,0,0},2);
		m3.RecalculateNormals ();
		mf3.mesh = m3;
		MeshRenderer mr3 = g3.AddComponent<MeshRenderer> ();
		mr3.materials = matList;
		SelfRotate sr3 = g3.AddComponent<SelfRotate> ();
		sr3.r = Vector3.up * -7.5f + Vector3.up * 1f;

		
		SelfRotate sr = g_land.AddComponent<SelfRotate> ();
		sr.r = Vector3.forward * 2f;

		
		g_land.GetComponent<Renderer> ().materials = matList;




		// placing stuff
		MeshCollider mc = g_land.AddComponent<MeshCollider> ();
		MeshCollider mc2 = g2.AddComponent<MeshCollider> ();
		g_land.layer = 8;
		g2.layer = 9;


		// trees
		float treesRatio = 0.3f;//Random.Range (0.35f, 0.4f);
		float urbanRatio = 0.4f;//Random.Range (0.35f, 0.4f);
		GameObject temp = Instantiate (spherePlacement) as GameObject;
		Vector3[] vt = temp.GetComponent<MeshFilter> ().mesh.vertices;
		List<Vector3> vtl = new List<Vector3> ();
		foreach (Vector3 vvv in vt) {
			bool hasIt = false;
			if (vtl.Count != 0){
				foreach (Vector3 vvvv in vtl){
					if (vvv == vvvv) {
						hasIt = true;
						break;
					}
				}
			}
			if (!hasIt){
				vtl.Add (vvv);
			}
		}
		Destroy (temp);

		float invScale = 1 / Mathf.Pow(2, scale);
		foreach (Vector3 v in vtl) {
			Vector3 vv = (Perlin3D (v, -1.5f) + Vector3.one) * 0.5f;
			if (vv.y * vv.x < Mathf.Pow (treesRatio, 2) || vv.y * vv.x > Mathf.Pow ((1-urbanRatio), 2)){
				RaycastHit hit;
				if (Physics.Raycast(v*300, -v, out hit, 100f, 256)){
					if (hit.point.magnitude > 1.01f){
					
						GameObject ins = null;

						if (hit.point.magnitude > 1.10){
							if ( hit.point.magnitude < 1.12f){
							/*	ins = Instantiate (MountainProps[Random.Range(0, MountainProps.Length)]) as GameObject;
								ins.transform.parent = g_mountains.transform;
								ins.transform.position = hit.point.normalized;
							*/}
						}

						else if (vv.y * vv.x < Mathf.Pow (treesRatio, 2)){
							ins = Instantiate (NatureProps[Random.Range(0, NatureProps.Length)]) as GameObject;
							ins.transform.parent = g_trees.transform;
							ins.transform.position = hit.point;
						}
						else if (vv.y * vv.x > Mathf.Pow ((1-urbanRatio), 2)){
							ins = Instantiate (UrbanProps[Random.Range(0, UrbanProps.Length)]) as GameObject;
							ins.transform.parent = g_urban.transform;
							ins.transform.position = hit.point;
						}

						if (ins != null) {
							
							ins.transform.LookAt(v*20000);
							ins.transform.Rotate (new Vector3 (0, 0, Random.value * 360));
							
							ins.transform.localScale = Vector3.one * 2f * ( invScale) /* * 0.01f*/ * (1 + Random.Range (-0.15f, 0.15f));

}
					}
				}
			}
		}
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
		float x = input.x * 200 * (1+ scale) + noiseXOffset * offsetOffset;
		float y = input.y * 200 * (1+ scale) + noiseYOffset * offsetOffset;
		float z = input.z * 200 * (1+ scale) + noiseZOffset * offsetOffset;

		Vector3 output = new Vector3 (
			Mathf.PerlinNoise (y, z),
			Mathf.PerlinNoise (z, x),
			Mathf.PerlinNoise (x, y)
		);

		output *= 2;
		output -= Vector3.one;

		return (output);
	}

}

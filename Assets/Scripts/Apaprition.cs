using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apaprition : MonoBehaviour {

	public Transform spark;
	public Transform dust;
	public Transform clouds;
	public Transform[] bits;
	private Transform bitparent;
	public Transform water;

	// Use this for initialization
	void Start () {
		bitparent = bits [0].parent.parent;
		StartCoroutine ("Apparition");
	}
	
	// Update is called once per frame
	void Update () {

		water.Rotate ((Vector3.up * 10f + Vector3.right * 1f) * Time.deltaTime, Space.World);
		clouds.Rotate ((Vector3.up * -5f + Vector3.right * -1f) * Time.deltaTime, Space.World);
		bitparent.Rotate (Vector3.up * 5f * Time.deltaTime, Space.World);
		dust.Rotate (Vector3.forward * 5f * Time.deltaTime, Space.World);
		spark.Rotate (Vector3.forward * -10f * Time.deltaTime, Space.World);
	}

	IEnumerator Apparition() {
		float t;

		spark.localScale = Vector3.zero;
		dust.localScale = Vector3.zero;
		clouds.localScale = Vector3.zero;
		water.localScale = Vector3.zero;

		foreach (Transform b in bits) {
			b.localScale = Vector3.zero;
			b.Rotate (new Vector3 (Random.value * 360, Random.value * 360, Random.value * 360));
			//b.position = 1.2f * new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f));
			b.position *= 2;
		}

		// spark on
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime;
			float rt = 1 - t;
			spark.localScale = Vector3.one * rt;
			yield return null;
		}


		yield return new WaitForSeconds (1f);

		// spark off, dust on
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime * 0.5f;
			float rt = 1 - t;
			spark.localScale = Vector3.one * t;
			dust.localScale = Vector3.one * rt * 3; // should be a aprticle system you turn on
			yield return null;
		}
		Destroy (spark.gameObject);


		yield return new WaitForSeconds (1f);

		// bits on, clouds on
		Color c = clouds.GetComponent<Renderer> ().material.color;
		clouds.localScale = Vector3.one * 100;
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime * 0.5f;
			float rt = 1 - t;

			clouds.GetComponent<Renderer> ().material.color = new Color (c.r, c.g, c.b, rt * 0.3f);

			foreach (Transform b in bits) {
				b.localScale = Vector3.one * rt * 100;
				b.Rotate(Vector3.one * 50 * Time.deltaTime);
			}

			yield return null;
		}


		//yield return new WaitForSeconds (1f);
		// bits keep rotating, slowing down
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime * 0.5f;
			foreach (Transform b in bits) {
				b.Rotate(Vector3.one * 50 * Time.deltaTime * t);
			}
			yield return null;
		}

		// bits reunite, dust off
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime * 0.2f;
			float rt = 1 - t;
			foreach (Transform b in bits) {
				b.localRotation = Quaternion.Slerp (b.localRotation, Quaternion.Euler (Vector3.right * -90), rt * rt);//* (1.5f+0.5f));
				b.localPosition = Vector3.Lerp (b.localPosition, Vector3.zero, rt * rt);
			}

			dust.localScale = Vector3.one * t * 3; // should be a particle system you turn off
			yield return null;
		}
		Destroy (dust.gameObject);


		//yield return new WaitForSeconds (1f);

		// water on
		t = 1;
		while (t > 0) {
			t -= Time.deltaTime * 0.5f;
			float rt = 1 - t;
			water.localScale = Vector3.one * rt * 100;
			yield return null;
		}
		Destroy (bitparent.gameObject);


	}

}

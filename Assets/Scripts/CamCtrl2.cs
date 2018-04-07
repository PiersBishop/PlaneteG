using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl2 : MonoBehaviour {

	Transform rota;
	public Material sky;
	HSVtoRGB hsv;

	void Start () {
		rota = transform.parent ;
		DontDestroyOnLoad (rota.gameObject);
		hsv = gameObject.AddComponent<HSVtoRGB> ();
	}

	void Update () {
		transform.localPosition += Vector3.forward * Input.GetAxis ("Horizontal") * 0.3f * Time.deltaTime;
		transform.localPosition += Vector3.up * Input.GetAxis ("Vertical") * 0.3f * Time.deltaTime;
		rota.eulerAngles += Vector3.right * Input.GetAxis ("Page") * 5f * Time.deltaTime;

		float r = sky.GetFloat ("_Rotation");
		r += Time.deltaTime * 0.5f;
		sky.SetFloat ("_Rotation", r);

		float s = Mathf.Sin (Time.realtimeSinceStartup * 0.05f) * 0.5f + 0.8f;
		sky.SetFloat ("_Exposure", s);

		// also have sky change tint
		Color c = hsv.Convert(Time.realtimeSinceStartup * 0.6f, 1, 0.69f);
		sky.SetColor ("_Tint", c);
	}
}

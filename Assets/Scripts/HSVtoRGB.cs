using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSVtoRGB : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Color Convert (Vector3 HSV){

		float H = HSV.x;
		float S = HSV.y;
		float V = HSV.z;

		return (Convert (H, S, V));
	}

	public Color Convert (float H, float S, float V){

		while (H < 0 || H >= 360) {
			if (H >= 360)
				H -= 360;
			if (H < 0)
				H += 360;
		}

		S = Mathf.Clamp01 (S);
		V = Mathf.Clamp01 (V);

		float C = V * S;
		float X = C * (1 - Mathf.Abs(((H/60)%2)-1));
		float m = V - C;

		Color rgb;

		if (H < 60) {
			rgb = new Color (C, X, 0);
		} else if (H < 120) {
			rgb = new Color (X, C, 0);
		} else if (H < 180) {
			rgb = new Color (0, C, X);
		} else if (H < 240) {
			rgb = new Color (0, X, C);
		} else if (H < 300) {
			rgb = new Color (X, 0, C);
		} else {
			rgb = new Color (C, 0, X);
		}


		rgb += new Color (m, m, m, 0);

		return (rgb);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class bitssetup : MonoBehaviour {

	public Transform[] children;

	public bool DO_IT = false;

	void Update () {
		if (DO_IT) {
			for (int i = 0; i < children.Length; i ++) {
				GameObject g = new GameObject ("placeholder");
				g.transform.position = children [i].position;
				children [i].parent = g.transform;
				g.transform.parent = transform;
			}
		}
	}
}

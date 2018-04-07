using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _WorldRotate : MonoBehaviour {
	public Vector3 r = Vector3.zero;
	public bool noInitRotate = false;

	void Start () {
		if (!noInitRotate) {
			transform.eulerAngles += Random.value * 360f * Vector3.up;
		}
	}

	void Update () {
		transform.Rotate (r * Time.deltaTime, Space.World);
	}
}

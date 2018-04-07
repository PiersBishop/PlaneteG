using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

	public Vector3 r = Vector3.zero;

	// Use this for initialization
	void Start () {
		transform.eulerAngles += Random.value * 360f * Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (r * Time.deltaTime);
	}
}

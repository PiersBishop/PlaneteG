using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movetest : MonoBehaviour {

	void FixedUpdate() {
		GetComponent<Rigidbody> ().MovePosition (transform.position +  Vector3.right * Time.deltaTime);
	}

}

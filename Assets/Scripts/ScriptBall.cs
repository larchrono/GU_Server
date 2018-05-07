using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Ground") {
			gameObject.tag = "Ball";
			GetComponent<Rigidbody> ().useGravity = false;
			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezePositionY;
		}
	}
}
